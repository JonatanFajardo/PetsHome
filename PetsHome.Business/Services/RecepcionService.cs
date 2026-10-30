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
    /// Orquesta el modulo Recepcion / Sala de espera.
    /// Feed, no gate: encola citas del dia y las transiciona de estado.
    /// Un dato, multiples lentes: la cola de hoy la consumen recepcion, vet y admin.
    /// </summary>
    public class RecepcionService
    {
        private readonly RecepcionRepository _repository;
        private readonly ILogger<RecepcionService> _logger;

        public RecepcionService(RecepcionRepository repository, ILogger<RecepcionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Encola una cita del dia ("En espera"). @cita_Recepcionista = usuario actual.
        /// </summary>
        public async Task<CheckInResultDto> CheckInAsync(CheckInDto dto, int recepcionista)
        {
            var resultado = new CheckInResultDto { Ok = false, CitaId = 0 };

            if (dto == null || dto.MascId <= 0)
            {
                resultado.Mensaje = "Debe seleccionar un paciente valido.";
                return resultado;
            }

            try
            {
                var res = await _repository.CheckInAsync(
                    dto.MascId, dto.TipoConsultaId, dto.Motivo,
                    string.IsNullOrWhiteSpace(dto.Prioridad) ? "Normal" : dto.Prioridad,
                    dto.VetId, recepcionista, dto.Peso);

                if (res != null && res.cita_Id.HasValue && res.cita_Id.Value > 0)
                {
                    resultado.Ok = true;
                    resultado.CitaId = res.cita_Id.Value;
                    resultado.Mensaje = "Paciente agregado a la sala de espera.";
                }
                else
                {
                    resultado.Mensaje = res?.MessageStatus ?? "No se pudo registrar el ingreso.";
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                resultado.Mensaje = "Ocurrio un error al registrar el ingreso.";
            }

            return resultado;
        }

        /// <summary>
        /// Cola de la sala de espera del dia (o de la fecha indicada).
        /// </summary>
        public async Task<List<SalaEsperaItemViewModel>> SalaEsperaAsync(DateTime? fecha = null, int? vetId = null)
        {
            try
            {
                var result = await _repository.SalaEsperaAsync(fecha, vetId)
                             ?? new List<PR_Medico_SalaEspera_ListResult>();
                return result.Select(Map).ToList();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<SalaEsperaItemViewModel>();
            }
        }

        /// <summary>Tablero Kanban del día (incluye Finalizadas) para el whiteboard.</summary>
        public async Task<List<SalaEsperaItemViewModel>> WhiteboardAsync(DateTime? fecha = null, int? vetId = null)
        {
            try
            {
                var result = await _repository.WhiteboardAsync(fecha, vetId)
                             ?? new List<PR_Medico_SalaEspera_ListResult>();
                return result.Select(Map).ToList();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<SalaEsperaItemViewModel>();
            }
        }

        /// <summary>
        /// Arma el ViewModel completo del tablero (cola + conteos).
        /// </summary>
        public async Task<RecepcionViewModel> GetTableroAsync(DateTime? fecha = null, int? vetId = null)
        {
            var model = new RecepcionViewModel { Fecha = fecha ?? DateTime.Today };
            model.Cola = await SalaEsperaAsync(fecha, vetId);
            model.TotalEnEspera = model.Cola.Count(c => c.Estado == "En espera");
            model.TotalEnAtencion = model.Cola.Count(c => c.Estado == "En atencion");
            return model;
        }

        /// <summary>
        /// Lista de veterinarios (rol 4 activos) para el selector opcional del check-in.
        /// </summary>
        public async Task<List<VeterinarioOpcionViewModel>> VeterinariosAsync()
        {
            try
            {
                var result = await _repository.VeterinariosAsync()
                             ?? new List<PR_Seguridad_Veterinarios_DropdownResult>();
                return result.Select(x => new VeterinarioOpcionViewModel { Id = x.Id, Nombre = x.Nombre }).ToList();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<VeterinarioOpcionViewModel>();
            }
        }

        /// <summary>
        /// Transiciona el estado de una visita a uno de los 7 estados. El SP valida la
        /// transicion; aqui solo se pasa el estado destino y se propaga su resultado
        /// (CodeStatus/MessageStatus) para que el front muestre el motivo si falla.
        /// </summary>
        public async Task<CambiarEstadoResultDto> CambiarEstadoAsync(int citaId, string estado, int usuario)
        {
            if (citaId <= 0 || string.IsNullOrWhiteSpace(estado))
                return new CambiarEstadoResultDto { Ok = false, Mensaje = "Datos de la transicion invalidos." };

            try
            {
                var res = await _repository.CambiarEstadoAsync(citaId, estado.Trim(), usuario);
                return new CambiarEstadoResultDto
                {
                    Ok = res != null && res.Success,
                    Mensaje = res?.MessageStatus
                };
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new CambiarEstadoResultDto { Ok = false, Mensaje = "Ocurrio un error al cambiar el estado." };
            }
        }

        private static SalaEsperaItemViewModel Map(PR_Medico_SalaEspera_ListResult x) => new SalaEsperaItemViewModel
        {
            CitaId = x.cita_Id,
            MascId = x.masc_Id,
            MascotaNombre = x.MascotaNombre,
            DuenoNombre = x.DuenoNombre,
            EsInterno = x.EsInterno,
            Motivo = x.Motivo,
            TipoConsulta = x.TipoConsulta,
            Prioridad = x.Prioridad,
            HoraLlegada = x.HoraLlegada,
            MinutosEspera = x.MinutosEspera,
            Estado = x.Estado,
            VetAsignado = x.VetAsignado
        };
    }
}
