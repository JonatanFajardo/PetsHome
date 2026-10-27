using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Orquesta la pantalla "En consulta" (cabina de trabajo del veterinario).
    /// REUTILIZA:
    ///   - PerfilMedicoService : ficha, historial, meds activos, vacunas (banderas rojas).
    ///   - TipoConsultaService : dropdown de tipos de consulta.
    ///   - CitaMedicaRepository: SP existente [Medico].[PR_Medico_CitaMedica_Insert] al Finalizar.
    ///   - ConsultaRepository  : peso vivo + id de la ultima cita (SPs auxiliares nuevos).
    /// </summary>
    public class ConsultaService
    {
        private readonly PerfilMedicoService _perfilService;
        private readonly TipoConsultaService _tipoConsultaService;
        private readonly CitaMedicaRepository _citaRepository;
        private readonly ConsultaRepository _consultaRepository;
        private readonly RecetaService _recetaService;
        private readonly InventarioSalidaRepository _inventarioSalidaRepository;
        private readonly RecepcionRepository _recepcionRepository;
        private readonly RecepcionService _recepcionService;
        private readonly CargoDispensacionService _cargoService;
        private readonly ILogger<ConsultaService> _logger;

        // Tipo de salida de inventario usado al entregar medicamentos en consulta.
        // CHK_TipoSalida permite D (Donacion), T (Traslado), V (Venta), P (Perdida), U (Uso).
        // 'U' = uso/consumo interno: la mascota del refugio consume el medicamento.
        private const char TipoSalidaConsulta = 'U';

        public ConsultaService(
            PerfilMedicoService perfilService,
            TipoConsultaService tipoConsultaService,
            CitaMedicaRepository citaRepository,
            ConsultaRepository consultaRepository,
            RecetaService recetaService,
            InventarioSalidaRepository inventarioSalidaRepository,
            RecepcionRepository recepcionRepository,
            RecepcionService recepcionService,
            CargoDispensacionService cargoService,
            ILogger<ConsultaService> logger)
        {
            _perfilService = perfilService;
            _tipoConsultaService = tipoConsultaService;
            _citaRepository = citaRepository;
            _consultaRepository = consultaRepository;
            _recetaService = recetaService;
            _inventarioSalidaRepository = inventarioSalidaRepository;
            _recepcionRepository = recepcionRepository;
            _recepcionService = recepcionService;
            _cargoService = cargoService;
            _logger = logger;
        }

        /// <summary>
        /// Historial de nombres de medicamento ya recetados/aplicados (texto libre),
        /// para alimentar el lookup de medicamentos.
        /// </summary>
        public async Task<List<string>> MedicamentosUsadosAsync()
        {
            try
            {
                var result = await _consultaRepository.MedicamentosUsadosAsync();
                return result == null
                    ? new List<string>()
                    : result.Select(x => x.Medicamento).ToList();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<string>();
            }
        }

        public Task<int> GetRandomMascIdAsync() => _perfilService.GetRandomMascIdAsync();

        /// <summary>
        /// Arma el ViewModel de la pantalla reutilizando el dashboard del perfil medico.
        /// Si mascId &lt;= 0 NO carga paciente: devuelve el ESTADO VACIO (SinPaciente=true)
        /// con la sala de espera del dia para que el vet elija a quien atender.
        /// </summary>
        public async Task<ConsultaViewModel> GetPantallaAsync(int mascId, int citaId = 0)
        {
            var model = new ConsultaViewModel { MascId = mascId, CitaId = citaId };
            try
            {
                // La cola del dia se expone siempre (estado vacio y panel lateral).
                model.SalaEspera = await _recepcionService.SalaEsperaAsync(null, null)
                                   ?? new List<SalaEsperaItemViewModel>();

                if (mascId <= 0)
                {
                    // Estado vacio: no cargar ficha, el front muestra el selector.
                    model.SinPaciente = true;
                    model.MascId = 0;
                    model.TiposConsulta = await _tipoConsultaService.ListAsync() ?? new List<TipoConsultaViewModel>();
                    return model;
                }

                model.SinPaciente = false;
                model.Perfil = await _perfilService.GetDashboardAsync(mascId) ?? new PerfilMedicoViewModel();
                model.TiposConsulta = await _tipoConsultaService.ListAsync() ?? new List<TipoConsultaViewModel>();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
            }
            return model;
        }

        /// <summary>
        /// El vet toma una visita de la sala de espera: la pasa a "En atencion".
        /// Devuelve (ok, mascId) para que el front abra la cabina con esa mascota.
        /// El mascId se resuelve de la cola del dia (la visita sigue listada tras el cambio).
        /// </summary>
        public async Task<(bool ok, int mascId)> AtenderAsync(int citaId, int usuario)
        {
            if (citaId <= 0)
                return (false, 0);

            try
            {
                // Resolver mascId desde la cola del dia ANTES de cambiar el estado
                // (la fila esta "En espera" y sigue apareciendo).
                int mascId = 0;
                var cola = await _recepcionRepository.SalaEsperaAsync(null, null);
                if (cola != null)
                {
                    foreach (var fila in cola)
                    {
                        if (fila.cita_Id == citaId) { mascId = fila.masc_Id; break; }
                    }
                }

                var res = await _recepcionRepository.CambiarEstadoAsync(citaId, "En atencion", usuario);
                bool ok = res != null && res.Success;
                return (ok, mascId);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return (false, 0);
            }
        }

        /// <summary>
        /// Persiste la consulta al Finalizar. Crea la cita medica (reutilizando el SP
        /// existente), actualiza el peso vivo de la mascota si se capturo y devuelve el id.
        /// </summary>
        public async Task<GuardarConsultaResultDto> GuardarAsync(GuardarConsultaDto dto, int usuarioCrea)
        {
            var resultado = new GuardarConsultaResultDto { Ok = false, CitaId = 0 };

            if (dto == null || dto.MascId <= 0)
            {
                resultado.Mensaje = "Datos de la consulta invalidos.";
                return resultado;
            }

            try
            {
                // ── 0. Validacion de stock EN VIVO (antes de crear nada) ──
                // Lineas a entregar del inventario: marcadas "Entregar" y con item real.
                var lineasEntrega = (dto.Medicamentos ?? new List<MedicamentoLineaDto>())
                    .Where(l => l != null && l.Entregar && l.Id > 0 && !string.IsNullOrWhiteSpace(l.Nombre))
                    .ToList();

                int refgId = 0;
                int? cliId = null;
                string nombreMascota = "Mascota";

                if (lineasEntrega.Count > 0)
                {
                    // Resolver el refugio de la mascota (el stock es por item y por refugio).
                    try
                    {
                        var info = await _inventarioSalidaRepository.RefugioPorMascotaAsync(dto.MascId);
                        if (info != null)
                        {
                            refgId = info.refg_Id;
                            cliId = info.cli_Id;
                            if (!string.IsNullOrWhiteSpace(info.masc_Nombre))
                                nombreMascota = info.masc_Nombre;
                        }
                    }
                    catch (Exception refgError)
                    {
                        _logger.LogWarning(refgError, "No se pudo resolver el refugio de la mascota {MascId}", dto.MascId);
                    }

                    // Si la mascota no tiene refugio (paciente de cliente externo), el stock
                    // sale del SITIO DONDE ATIENDE el vet (su refugio asignado), no de la mascota.
                    if (refgId <= 0)
                    {
                        try { refgId = await _inventarioSalidaRepository.RefugioPorUsuarioAsync(usuarioCrea); }
                        catch (Exception usuError)
                        {
                            _logger.LogWarning(usuError, "No se pudo resolver el refugio del usuario {Usuario}", usuarioCrea);
                        }
                    }

                    // Solo validamos/descontamos si hay un refugio (misma cantidad que luego se descuenta).
                    if (refgId > 0)
                    {
                        var faltantes = new List<FaltanteStockDto>();
                        foreach (var linea in lineasEntrega)
                        {
                            int pedido = linea.Cantidad <= 0 ? 1 : linea.Cantidad;
                            int disponible = 0;
                            try { disponible = await _inventarioSalidaRepository.ExistenciaPorItemAsync(linea.Id, refgId); }
                            catch (Exception exiError)
                            {
                                _logger.LogWarning(exiError, "No se pudo leer existencia del item {ItmId} en refugio {RefgId}", linea.Id, refgId);
                            }

                            if (pedido > disponible)
                                faltantes.Add(new FaltanteStockDto { Nombre = linea.Nombre.Trim(), Disponible = disponible, Pedido = pedido });
                        }

                        if (faltantes.Count > 0)
                        {
                            // Bloquea TODO el Finalizar: no se crea cita, ni recetas, ni salidas.
                            resultado.Ok = false;
                            resultado.Faltantes = faltantes;
                            resultado.Mensaje = "Sin stock suficiente para entregar algunos medicamentos.";
                            return resultado;
                        }
                    }
                }

                // ── 1. Cita medica ──
                var entity = new tbCitaMedica
                {
                    masc_Id = dto.MascId,
                    cita_FechaConsulta = DateTime.Now,
                    tipoCon_Id = dto.TipoConsultaId,
                    cita_MotivoConsulta = dto.Motivo,
                    cita_Diagnostico = dto.Diagnostico,
                    cita_Peso = dto.Peso,
                    cita_Temperatura = dto.Temperatura,
                    cita_FrecuenciaCardiaca = dto.FrecuenciaCardiaca,
                    cita_FrecuenciaRespiratoria = dto.FrecuenciaRespiratoria,
                    cita_Mucosas = string.IsNullOrWhiteSpace(dto.Mucosas) ? null : dto.Mucosas.Trim(),
                    cita_LlenadoCapilar = dto.LlenadoCapilar,
                    cita_EscalaDolor = dto.EscalaDolor,
                    cita_CondicionCorporal = dto.CondicionCorporal,
                    vac_Id = dto.VacunaId,
                    cita_ProximaCita = dto.ProximaCita,
                    cita_MotivoProximaCita = dto.MotivoProximaCita,
                    cita_UsuarioCrea = usuarioCrea
                };

                if (dto.CitaId > 0)
                {
                    // ── 1a. Finalizar una visita existente (venia de la sala de espera) ──
                    // En vez de crear una cita nueva, se actualizan sus datos clinicos
                    // (diagnostico/peso/etc.) y se cierra el estado a "Finalizada".
                    // Las recetas y salidas se ligan a ESTE cita_Id (que ya existe).
                    entity.cita_Id = dto.CitaId;
                    try
                    {
                        var updResult = await _citaRepository.EditAsync(entity);
                        if (!updResult.Success)
                            _logger.LogWarning("No se pudo actualizar los datos clinicos de la cita {CitaId}: {Msg}", dto.CitaId, updResult.MessageStatus);
                    }
                    catch (Exception updError)
                    {
                        _logger.LogWarning(updError, "Error al actualizar los datos clinicos de la cita {CitaId}", dto.CitaId);
                    }

                    // Cerrar la visita (transicion de estado). Esto es lo minimo garantizado.
                    try { await _recepcionRepository.CambiarEstadoAsync(dto.CitaId, "Finalizada", usuarioCrea); }
                    catch (Exception estError) { _logger.LogWarning(estError, "No se pudo finalizar la visita {CitaId}", dto.CitaId); }

                    resultado.Ok = true;
                    resultado.CitaId = dto.CitaId;
                }
                else
                {
                    // ── 1b. Crear una cita nueva (consulta directa, sin visita encolada) ──
                    var citaResult = await _citaRepository.AddAsync(entity);
                    if (!citaResult.Success)
                    {
                        resultado.Mensaje = citaResult.MessageStatus ?? "No se pudo guardar la cita.";
                        return resultado;
                    }

                    resultado.Ok = true;
                    resultado.CitaId = await _consultaRepository.UltimaCitaIdAsync(dto.MascId);
                }

                // ── 2. Actualizar peso vivo de la mascota (best-effort) ──
                if (dto.Peso.HasValue && dto.Peso.Value > 0)
                {
                    try { await _consultaRepository.ActualizarPesoAsync(dto.MascId, dto.Peso.Value, usuarioCrea); }
                    catch (Exception pesoError) { _logger.LogWarning(pesoError, "No se pudo actualizar el peso de la mascota {MascId}", dto.MascId); }
                }

                // ── 3. Medicamentos/recetas por linea ──
                // Persiste cada medicamento como una receta reutilizando RecetaService.
                // INVARIANTE: itm_Id (afirma que salio del inventario) se guarda SOLO si la
                // linea de verdad se descuenta: marcada Entregar + en stock + con ubicacion
                // resuelta (refgId > 0). Si no hay ubicacion, NO se vincula item: queda como
                // receta de texto libre (no mentir diciendo que se entrego del inventario).
                // Mapa linea->recetaId: liga cada linea entregada con la receta que genero,
                // para poblar saldet_RecetaId en el paso 4 (trazabilidad receta->salida).
                // Clave por referencia: las entradas de lineasEntrega son los MISMOS objetos
                // que las de dto.Medicamentos (Where sin copia), asi el lookup en paso 4 acierta.
                var recetaPorLinea = new Dictionary<MedicamentoLineaDto, int>();

                if (resultado.CitaId > 0 && dto.Medicamentos != null)
                {
                    foreach (var linea in dto.Medicamentos)
                    {
                        if (linea == null || string.IsNullOrWhiteSpace(linea.Nombre))
                            continue;

                        try
                        {
                            bool entregaItem = linea.Entregar && linea.EnStock && linea.Id > 0 && refgId > 0;
                            var receta = new RecetaFormViewModel
                            {
                                cita_Id = resultado.CitaId,
                                masc_Id = dto.MascId,
                                receta_Medicamento = linea.Nombre.Trim(),
                                itm_Id = entregaItem ? linea.Id : (int?)null,
                                receta_Dosis = linea.Dosis,
                                receta_Frecuencia = linea.Frecuencia,
                                receta_Duracion = linea.Duracion,
                                receta_Estado = "Activa",
                                receta_FechaInicio = DateTime.Now,
                                receta_UsuarioCrea = usuarioCrea
                            };

                            // Lineas que SE DISPENSAN del inventario (internas y de cliente):
                            // capturamos el receta_Id para ligarlo a la salida (paso 4) y, si es
                            // cliente externo, encolar el cargo con el MISMO receta_Id.
                            if (entregaItem)
                            {
                                int recetaId = await _recetaService.AddReturningIdAsync(receta);
                                if (recetaId > 0)
                                {
                                    recetaPorLinea[linea] = recetaId;

                                    // Dispensación a CLIENTE EXTERNO (cli_Id set): encolar el cargo
                                    // (cola dispensar→cobrar). El stock ya se descuenta en el paso 4;
                                    // la cola es solo el cobro pendiente (feed, no gate).
                                    if (cliId.HasValue)
                                    {
                                        int cant = linea.Cantidad <= 0 ? 1 : linea.Cantidad;
                                        try
                                        {
                                            await _cargoService.EncolarDesdeConsultaAsync(
                                                recetaId, dto.MascId, cliId.Value,
                                                resultado.CitaId > 0 ? resultado.CitaId : (int?)null,
                                                linea.Id, cant, usuarioCrea);
                                        }
                                        catch (Exception cargoError)
                                        {
                                            _logger.LogWarning(cargoError, "No se pudo encolar el cargo de dispensación (receta {RecetaId})", recetaId);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await _recetaService.AddAsync(receta);
                            }
                        }
                        catch (Exception recetaError)
                        {
                            _logger.LogWarning(recetaError, "No se pudo guardar la receta '{Medicamento}' de la mascota {MascId}", linea.Nombre, dto.MascId);
                        }
                    }
                }

                // ── 4. Salida de inventario (descuento de stock) ──
                // Ya se valido el stock en vivo (paso 0). Un header por consulta y un
                // detalle por cada linea entregada; el SP de detalle descuenta exi_Cantidad.
                if (lineasEntrega.Count > 0 && refgId > 0)
                {
                    try
                    {
                        // Paciente de cliente externo (cli_Id set) => Venta ('V'); interno => Uso ('U').
                        // El movimiento queda correctamente clasificado; la factura es el paso siguiente.
                        char tipoSalida = cliId.HasValue ? 'V' : TipoSalidaConsulta;
                        int salId = await _inventarioSalidaRepository.InsertSalidaAsync(
                            "Consulta medica - " + nombreMascota, tipoSalida, refgId, usuarioCrea);

                        if (salId > 0)
                        {
                            foreach (var linea in lineasEntrega)
                            {
                                int pedido = linea.Cantidad <= 0 ? 1 : linea.Cantidad;
                                try
                                {
                                    // Si la linea genero receta (paso 3), ligarla a la salida.
                                    int? recetaIdLinea = recetaPorLinea.TryGetValue(linea, out int rid) && rid > 0
                                        ? rid : (int?)null;
                                    bool ok = await _inventarioSalidaRepository.InsertSalidaDetalleAsync(
                                        salId, linea.Id, pedido, "Entregado en consulta", usuarioCrea, recetaIdLinea);
                                    if (!ok)
                                        _logger.LogWarning("No se pudo descontar el item {ItmId} (x{Cantidad}) de la salida {SalId}", linea.Id, pedido, salId);
                                }
                                catch (Exception detError)
                                {
                                    _logger.LogWarning(detError, "Error al descontar item {ItmId} (x{Cantidad}) de la salida {SalId}", linea.Id, pedido, salId);
                                }
                            }
                        }
                        else
                        {
                            _logger.LogWarning("No se pudo crear el header de salida de inventario para la mascota {MascId} (refugio {RefgId}).", dto.MascId, refgId);
                        }
                    }
                    catch (Exception salError)
                    {
                        _logger.LogWarning(salError, "No se pudo generar la salida de inventario de la consulta (mascota {MascId}).", dto.MascId);
                    }
                }

                resultado.Mensaje = "Consulta finalizada correctamente.";
                return resultado;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                resultado.Ok = false;
                resultado.Mensaje = "Ocurrio un error al guardar la consulta.";
                return resultado;
            }
        }
    }
}
