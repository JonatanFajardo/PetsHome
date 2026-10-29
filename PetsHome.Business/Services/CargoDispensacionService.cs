using AutoMapper;
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
    public class CargoDispensacionService
    {
        private readonly CargoDispensacionRepository _repo;
        private readonly ILogger<CargoDispensacionService> _logger;
        private readonly IMapper _mapper;

        public CargoDispensacionService(CargoDispensacionRepository repo, ILogger<CargoDispensacionService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<CargoDispensacionViewModel>> ListPendientesAsync(int? cli_Id = null)
        {
            try
            {
                var result = await _repo.ListPendientesAsync(cli_Id);
                return _mapper.Map<List<CargoDispensacionViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<CargoDispensacionViewModel>(); }
        }

        /// <summary>Pendientes agrupables POR VISITA (incluye cliente/cita/fecha para agrupar en la UI).</summary>
        public async Task<List<CargoDispensacionViewModel>> ListPendientesPorVisitaAsync()
        {
            try
            {
                var result = await _repo.ListPendientesPorVisitaAsync();
                return _mapper.Map<List<CargoDispensacionViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<CargoDispensacionViewModel>(); }
        }

        /// <summary>
        /// Cobra/resuelve UNA visita según disposición ('Pagado'/'PorCobrar'/'SinCargo').
        /// Devuelve (success, factId [null si SinCargo], message).
        /// </summary>
        public async Task<(bool success, int? factId, string message)> FacturarVisitaAsync(
            int citaId, string disposicion, DateTime? vencimiento, string motivo, int usuario)
        {
            try
            {
                var r = await _repo.FacturarVisitaAsync(citaId, disposicion, vencimiento, motivo, usuario);
                if (r == null) return (false, null, "No se pudo procesar la visita.");
                return (r.CodeStatus > 0, r.fact_Id, r.MessageStatus);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return (false, null, "Error al facturar la visita."); }
        }

        /// <summary>Encola un cargo (idempotente por receta). Devuelve true si quedó pendiente/ya existía.</summary>
        public async Task<bool> EncolarAsync(tbCargoDispensacion entity)
        {
            try { return (await _repo.EncolarAsync(entity)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        /// <summary>Encola un cargo desde consulta (snapshot automático). Idempotente por receta.</summary>
        public async Task<bool> EncolarDesdeConsultaAsync(int receta_Id, int masc_Id, int cli_Id, int? cita_Id, int itm_Id, int cantidad, int usuario)
        {
            try { return (await _repo.EncolarDesdeConsultaAsync(receta_Id, masc_Id, cli_Id, cita_Id, itm_Id, cantidad, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        /// <summary>Encola un cargo de SERVICIO desde consulta (snapshot automatico). Idempotente por (cita, servicio).</summary>
        public async Task<bool> EncolarServicioDesdeConsultaAsync(int serv_Id, int masc_Id, int cli_Id, int? cita_Id, int cantidad, int usuario)
        {
            try { return (await _repo.EncolarServicioDesdeConsultaAsync(serv_Id, masc_Id, cli_Id, cita_Id, cantidad, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> FacturarAsync(int cargo_Id, int fact_Id, int fdet_Id, int usuario)
        {
            try { return (await _repo.FacturarAsync(cargo_Id, fact_Id, fdet_Id, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        /// <summary>Factura todos los cargos pendientes de un cliente en una sola factura. Devuelve el id de factura creada (0 si falló).</summary>
        public async Task<int> FacturarClienteAsync(int cli_Id, int usuario)
        {
            try
            {
                var r = await _repo.FacturarClienteAsync(cli_Id, usuario);
                return (r != null && r.CodeStatus > 0 && r.fact_Id.HasValue) ? r.fact_Id.Value : 0;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return 0; }
        }

        public async Task<bool> CondonarAsync(int cargo_Id, string motivo, int usuario)
        {
            try { return (await _repo.CondonarAsync(cargo_Id, motivo, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> ReabrirAsync(int cargo_Id, int usuario)
        {
            try { return (await _repo.ReabrirAsync(cargo_Id, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }
    }
}
