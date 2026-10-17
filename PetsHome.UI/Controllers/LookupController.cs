using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// API genérica reutilizable que alimenta el componente JS "SearchSelect".
    /// Expone búsqueda (typeahead) y creación al vuelo de catálogos, parametrizada por "tipo".
    ///
    /// Agregar un catálogo nuevo = una sola línea en BuildRegistry().
    /// </summary>
    [Authorize]
    public class LookupController : BaseController
    {
        // ---- Servicios de catálogo (ya registrados en DI por otros controllers) ----
        private readonly VacunaService _vacuna;
        private readonly RazaService _raza;
        private readonly TipoConsultaService _tipoConsulta;
        private readonly TipoMedicamentoService _tipoMedicamento;
        private readonly GravedadService _gravedad;
        private readonly ViaAdministracionService _viaAdministracion;
        private readonly TipoParasitoService _tipoParasito;
        private readonly TipoEsterilizacionService _tipoEsterilizacion;
        private readonly ProcedenciaService _procedencia;
        private readonly ItemService _item;
        private readonly ConsultaService _consulta;
        private readonly SugerenciaClinicaService _sugerencia;
        private readonly ClientesService _clientes;

        private readonly Dictionary<string, LookupDef> _registry;

        // Categorias de Inventario.tbItems que cuentan como "medicamentos" para el lookup.
        private static readonly string[] CategoriasMedicina = { "Medicamentos", "Insumos Medicos" };

        public LookupController(
            VacunaService vacuna,
            RazaService raza,
            TipoConsultaService tipoConsulta,
            TipoMedicamentoService tipoMedicamento,
            GravedadService gravedad,
            ViaAdministracionService viaAdministracion,
            TipoParasitoService tipoParasito,
            TipoEsterilizacionService tipoEsterilizacion,
            ProcedenciaService procedencia,
            ItemService item,
            ConsultaService consulta,
            SugerenciaClinicaService sugerencia,
            ClientesService clientes)
        {
            _vacuna = vacuna;
            _raza = raza;
            _tipoConsulta = tipoConsulta;
            _tipoMedicamento = tipoMedicamento;
            _gravedad = gravedad;
            _viaAdministracion = viaAdministracion;
            _tipoParasito = tipoParasito;
            _tipoEsterilizacion = tipoEsterilizacion;
            _procedencia = procedencia;
            _item = item;
            _consulta = consulta;
            _sugerencia = sugerencia;
            _clientes = clientes;

            _registry = BuildRegistry();
        }

        // =====================================================================
        //  Registro de tipos
        // =====================================================================

        /// <summary>
        /// Mapa tipo -> definición. Cada entrada declara: pantalla (permisos),
        /// cómo listar todos los ítems y, opcionalmente, cómo crear uno nuevo.
        /// Si Create es null, el quick-create NO está permitido para ese tipo.
        /// </summary>
        private Dictionary<string, LookupDef> BuildRegistry()
        {
            var r = new Dictionary<string, LookupDef>(StringComparer.OrdinalIgnoreCase);

            // ---- Búsqueda + creación al vuelo (listas que crecen con el uso) ----
            r["raza"] = new LookupDef(
                "Listado de razas",
                async () => (await _raza.ListAsync() ?? new List<RazaListViewModel>())
                    .Select(x => new LookupItem(x.raza_Id, x.raza_Descripcion, x.raza_TipoAnimal)).ToList(),
                async (text, userId) => await _raza.AddAsync(
                    new RazaFormViewModel { raza_Descripcion = text, raza_EsActivo = true }, userId));

            r["tipoconsulta"] = new LookupDef(
                "Listado de tipos de consulta",
                async () => (await _tipoConsulta.ListAsync() ?? new List<TipoConsultaViewModel>())
                    .Select(x => new LookupItem(x.tipoCon_Id, x.tipoCon_Descripcion, null)).ToList(),
                async (text, userId) => await _tipoConsulta.AddAsync(
                    new TipoConsultaViewModel { tipoCon_Descripcion = text, tipoCon_EsActivoBool = true, tipoCon_UsuarioCrea = userId }));

            r["tipomedicamento"] = new LookupDef(
                "Listado de tipos de medicamento",
                async () => (await _tipoMedicamento.ListAsync() ?? new List<TipoMedicamentoViewModel>())
                    .Select(x => new LookupItem(x.tipoMed_Id, x.tipoMed_Descripcion, null)).ToList(),
                async (text, userId) => await _tipoMedicamento.AddAsync(
                    new TipoMedicamentoViewModel { tipoMed_Descripcion = text, tipoMed_EsActivoBool = true, tipoMed_UsuarioCrea = userId }));

            // ---- Solo búsqueda (crear al vuelo NO permitido) ----
            r["vacuna"] = new LookupDef(
                "Listado de vacunas",
                async () => (await _vacuna.ListAsync() ?? new List<VacunaListViewModel>())
                    .Select(x => new LookupItem(x.vac_Id, x.vac_Descripcion, x.vacu_Especie)).ToList(),
                null);

            r["gravedad"] = new LookupDef(
                "Listado de gravedades",
                async () => (await _gravedad.ListAsync() ?? new List<GravedadViewModel>())
                    .Select(x => new LookupItem(x.grav_Id, x.grav_Descripcion, null)).ToList(),
                null);

            r["viaadministracion"] = new LookupDef(
                "Listado de vias de administracion",
                async () => (await _viaAdministracion.ListAsync() ?? new List<ViaAdministracionViewModel>())
                    .Select(x => new LookupItem(x.viaAdmin_Id, x.viaAdmin_Descripcion, null)).ToList(),
                null);

            r["tipoparasito"] = new LookupDef(
                "Listado de tipos de parasito",
                async () => (await _tipoParasito.ListAsync() ?? new List<TipoParasitoViewModel>())
                    .Select(x => new LookupItem(x.tipoPar_Id, x.tipoPar_Descripcion, x.tipoPar_Categoria)).ToList(),
                null);

            r["tipoesterilizacion"] = new LookupDef(
                "Listado de tipos de esterilizacion",
                async () => (await _tipoEsterilizacion.ListAsync() ?? new List<TipoEsterilizacionViewModel>())
                    .Select(x => new LookupItem(x.tipoEst_Id, x.tipoEst_Descripcion, x.tipoEst_Sexo)).ToList(),
                null);

            // ---- Clientes (duenos de mascotas externas): busqueda + alta minima ----
            // Create minimo: parte el texto en nombres/apellidos (primer espacio).
            r["cliente"] = new LookupDef(
                "Listado de clientes",
                async () => (await _clientes.ListAsync() ?? new List<ClientesListViewModel>())
                    .Select(x => new LookupItem(
                        x.cli_Id,
                        System.String.Join(" ", new[] { x.cli_Nombres, x.cli_Apellidos }
                            .Where(s => !string.IsNullOrWhiteSpace(s))).Trim(),
                        x.cli_Telefono)).ToList(),
                async (text, userId) =>
                {
                    var partes = (text ?? string.Empty).Trim().Split(new[] { ' ' }, 2);
                    var nombres = partes.Length > 0 ? partes[0] : text;
                    var apellidos = partes.Length > 1 ? partes[1] : string.Empty;
                    return await _clientes.AddAsync(new ClientesFormViewModel
                    {
                        cli_Nombres = nombres,
                        cli_Apellidos = apellidos
                    }, userId);
                });

            r["procedencia"] = new LookupDef(
                "Listado de procedencias",
                async () => (await _procedencia.ListAsync() ?? new List<ProcedenciaViewModel>())
                    .Select(x => new LookupItem(x.proc_Id, x.proc_Descripcion, null)).ToList(),
                null);

            // ---- Medicamentos: inventario (cat 3/8) + historial de texto libre ----
            // Create = null: el "recetar texto libre" lo resuelve el frontend sin tocar servidor.
            r["medicamento"] = new LookupDef(
                "En consulta",
                BuildMedicamentosAsync,
                null);

            return r;
        }

        /// <summary>
        /// Lista mezclada para el lookup de medicamentos: primero los items de
        /// inventario de categorias de medicina (enStock=true, con stock), luego
        /// el historial de medicamentos ya recetados que NO coincidan con un item
        /// (enStock=false, stock=null).
        /// </summary>
        private async Task<List<LookupItem>> BuildMedicamentosAsync()
        {
            var list = await BuildMedicamentosInventarioAsync();

            var enInventario = new HashSet<string>(
                list.Select(x => x.Text ?? string.Empty),
                StringComparer.OrdinalIgnoreCase);

            var historial = await _consulta.MedicamentosUsadosAsync() ?? new List<string>();
            foreach (var nombre in historial)
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    continue;

                var limpio = nombre.Trim();
                if (enInventario.Contains(limpio))
                    continue;

                enInventario.Add(limpio);
                list.Add(new LookupItem(0, limpio, "Recetado antes", false, null));
            }

            return list;
        }

        /// <summary>
        /// Items de inventario de categorias de medicina (cat 3 Medicamentos / 8 Insumos Medicos).
        /// </summary>
        private async Task<List<LookupItem>> BuildMedicamentosInventarioAsync()
        {
            var items = await _item.ListAsync() ?? new List<ItemViewModel>();
            return items
                .Where(x => x.cat_Descripcion != null
                            && CategoriasMedicina.Contains(x.cat_Descripcion, StringComparer.OrdinalIgnoreCase))
                .Select(x => new LookupItem(
                    x.itm_Id,
                    x.itm_Descripcion,
                    x.cat_Descripcion,
                    true,
                    (int)x.itm_StockActual,
                    (int)x.itm_StockMinimo))
                .ToList();
        }

        // =====================================================================
        //  Endpoints
        // =====================================================================

        /// <summary>
        /// GET /Lookup/Search?tipo={tipo}&term={term}
        /// Devuelve hasta 20 ítems [{ id, text, meta }] filtrados en memoria por text.Contains(term).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string tipo, string term)
        {
            if (string.IsNullOrWhiteSpace(tipo) || !_registry.TryGetValue(tipo, out var def))
                return Json(Array.Empty<object>());

            var all = await def.GetAll();
            IEnumerable<LookupItem> query = all;

            if (!string.IsNullOrWhiteSpace(term))
            {
                var t = term.Trim().ToLowerInvariant();
                query = query.Where(x => x.Text != null && x.Text.ToLowerInvariant().Contains(t));
            }

            var data = query
                .Take(20)
                .Select(x => new { id = x.Id, text = x.Text, meta = x.Meta, enStock = x.EnStock, stock = x.Stock, stockMin = x.StockMin })
                .ToList();

            return Json(data);
        }

        /// <summary>
        /// GET /Lookup/Sugerencias?texto={texto}
        /// Motor de sugerencias por diagnostico. Combina las reglas clinicas
        /// (tbSugerenciaClinica) con coincidencias directas contra el inventario
        /// de medicina y el catalogo de vacunas.
        /// Devuelve { medicamentos:[{id,text,enStock,stock}], vacunas:[{id,text}] }.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Sugerencias(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return Json(new { medicamentos = Array.Empty<object>(), vacunas = Array.Empty<object>() });

            var textoLower = texto.ToLowerInvariant();

            var inventario = await BuildMedicamentosInventarioAsync();
            var vacunas = (await _vacuna.ListAsync() ?? new List<VacunaListViewModel>());

            // Acumuladores con dedupe por texto (case-insensitive).
            var medsMap = new Dictionary<string, LookupItem>(StringComparer.OrdinalIgnoreCase);
            var vacsMap = new Dictionary<string, LookupItem>(StringComparer.OrdinalIgnoreCase);

            // (1) Reglas clinicas que coinciden con el texto.
            var reglas = await _sugerencia.PorTextoAsync(texto) ?? new List<PetsHome.Common.Entities.PR_Medico_Sugerencia_PorTextoResult>();
            foreach (var regla in reglas)
            {
                if (string.IsNullOrWhiteSpace(regla.sug_Termino))
                    continue;

                var termino = regla.sug_Termino.Trim();
                if (string.Equals(regla.sug_TipoSugerencia, "vacuna", StringComparison.OrdinalIgnoreCase))
                {
                    var vac = vacunas.FirstOrDefault(v => v.vac_Descripcion != null
                        && v.vac_Descripcion.ToLowerInvariant().Contains(termino.ToLowerInvariant()));
                    AgregarVacuna(vacsMap, vac != null ? vac.vac_Id : 0, vac != null ? vac.vac_Descripcion : termino);
                }
                else
                {
                    var item = inventario.FirstOrDefault(i => i.Text != null
                        && i.Text.ToLowerInvariant().Contains(termino.ToLowerInvariant()));
                    if (item != null)
                        AgregarMed(medsMap, item.Id, item.Text, item.EnStock, item.Stock);
                    else
                        AgregarMed(medsMap, 0, termino, false, null);
                }
            }

            // (2) Coincidencia directa: el texto contiene el nombre del item / vacuna.
            foreach (var item in inventario)
            {
                if (!string.IsNullOrWhiteSpace(item.Text) && textoLower.Contains(item.Text.ToLowerInvariant()))
                    AgregarMed(medsMap, item.Id, item.Text, item.EnStock, item.Stock);
            }
            foreach (var vac in vacunas)
            {
                if (!string.IsNullOrWhiteSpace(vac.vac_Descripcion) && textoLower.Contains(vac.vac_Descripcion.ToLowerInvariant()))
                    AgregarVacuna(vacsMap, vac.vac_Id, vac.vac_Descripcion);
            }

            var medicamentos = medsMap.Values
                .Take(6)
                .Select(x => new { id = x.Id, text = x.Text, enStock = x.EnStock, stock = x.Stock })
                .ToList();

            var vacunasOut = vacsMap.Values
                .Take(4)
                .Select(x => new { id = x.Id, text = x.Text })
                .ToList();

            return Json(new { medicamentos, vacunas = vacunasOut });
        }

        private static void AgregarMed(Dictionary<string, LookupItem> map, int id, string text, bool enStock, int? stock)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var key = text.Trim();
            if (map.ContainsKey(key)) return;
            map[key] = new LookupItem(id, key, null, enStock, stock);
        }

        private static void AgregarVacuna(Dictionary<string, LookupItem> map, int id, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var key = text.Trim();
            if (map.ContainsKey(key)) return;
            map[key] = new LookupItem(id, key, null);
        }

        /// <summary>
        /// POST /Lookup/Create  (body: tipo, text)
        /// Crea el registro con campos mínimos usando el servicio del catálogo.
        /// Respeta permisos: requiere "insertar" sobre la pantalla del catálogo.
        /// Devuelve { id, text } o { error }.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] string tipo, [FromForm] string text)
        {
            if (string.IsNullOrWhiteSpace(tipo) || !_registry.TryGetValue(tipo, out var def))
                return Json(new { success = false, error = "Tipo de catálogo no soportado." });

            if (string.IsNullOrWhiteSpace(text))
                return Json(new { success = false, error = "El texto es obligatorio." });

            if (def.Create == null)
                return Json(new { success = false, error = "La creación al vuelo no está permitida para este catálogo." });

            if (!CurrentUserId.HasValue)
            {
                Response.StatusCode = 401;
                return Json(new { success = false, error = "Sesión expirada. Inicie sesión nuevamente." });
            }

            if (!PantallaAuthorizeAttribute.TienePermiso(User, def.Pantalla, "insertar"))
            {
                Response.StatusCode = 403;
                return Json(new { success = false, error = "No tiene permiso para crear en este catálogo." });
            }

            var nuevo = text.Trim();

            // Evita duplicados exactos (case-insensitive): si ya existe, devuelve el existente.
            var existentes = await def.GetAll();
            var yaExiste = existentes
                .Where(x => string.Equals(x.Text, nuevo, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            if (yaExiste != null)
                return Json(new { success = true, id = yaExiste.Id, text = yaExiste.Text });

            bool ok = await def.Create(nuevo, CurrentUserId.Value);
            if (!ok)
                return Json(new { success = false, error = "No se pudo crear el registro." });

            // El servicio AddAsync devuelve bool; recuperamos el id re-consultando por texto.
            var creado = (await def.GetAll())
                .Where(x => string.Equals(x.Text, nuevo, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            return Json(new
            {
                success = true,
                id = creado?.Id ?? 0,
                text = creado?.Text ?? nuevo
            });
        }

        /// <summary>
        /// Página de demostración manual del componente SearchSelect.
        /// </summary>
        [HttpGet]
        public IActionResult Demo()
        {
            return View();
        }

        // =====================================================================
        //  Tipos internos
        // =====================================================================

        private sealed class LookupItem
        {
            public int Id { get; }
            public string Text { get; }
            public string Meta { get; }

            /// <summary>True si el item proviene del inventario (solo aplica a medicamentos).</summary>
            public bool EnStock { get; }

            /// <summary>Stock disponible (solo aplica a medicamentos de inventario).</summary>
            public int? Stock { get; }

            /// <summary>Stock minimo configurado del item (para badges "bajo"; solo medicamentos de inventario).</summary>
            public int? StockMin { get; }

            public LookupItem(int id, string text, string meta)
                : this(id, text, meta, false, null, null)
            {
            }

            public LookupItem(int id, string text, string meta, bool enStock, int? stock)
                : this(id, text, meta, enStock, stock, null)
            {
            }

            public LookupItem(int id, string text, string meta, bool enStock, int? stock, int? stockMin)
            {
                Id = id;
                Text = text;
                Meta = meta;
                EnStock = enStock;
                Stock = stock;
                StockMin = stockMin;
            }
        }

        private sealed class LookupDef
        {
            /// <summary>Nombre de pantalla usado para validar permisos "insertar".</summary>
            public string Pantalla { get; }

            /// <summary>Lista completa del catálogo (proyectada a id/text/meta).</summary>
            public Func<Task<List<LookupItem>>> GetAll { get; }

            /// <summary>Crea un registro mínimo (text, userId) -> éxito. Null = no permitido.</summary>
            public Func<string, int, Task<bool>> Create { get; }

            public LookupDef(string pantalla,
                Func<Task<List<LookupItem>>> getAll,
                Func<string, int, Task<bool>> create)
            {
                Pantalla = pantalla;
                GetAll = getAll;
                Create = create;
            }
        }
    }
}
