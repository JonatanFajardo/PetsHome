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
    /// <summary>
    /// Logica de la bitacora de sustancias controladas: listar movimientos,
    /// registrar (con validaciones de testigo/paciente/saldo via SP) y calcular
    /// el saldo por farmaco.
    /// </summary>
    public class SustanciaControladaService
    {
        private readonly SustanciaControladaRepository _repo;
        private readonly ILogger<SustanciaControladaService> _logger;
        private readonly IMapper _mapper;

        public SustanciaControladaService(SustanciaControladaRepository repo,
            ILogger<SustanciaControladaService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<SustanciaMovimientoListViewModel>> ListAsync(int? itmId)
        {
            try
            {
                var result = await _repo.ListAsync(itmId);
                return _mapper.Map<List<SustanciaMovimientoListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<SustanciaMovimientoDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var result = await _repo.DetailAsync(id);
                return _mapper.Map<SustanciaMovimientoDetailViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<List<SustanciaSaldoViewModel>> SaldosAsync()
        {
            try
            {
                var result = await _repo.SaldosAsync();
                return _mapper.Map<List<SustanciaSaldoViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<SustanciaSaldoViewModel>(); }
        }

        /// <summary>
        /// Registra un movimiento. Devuelve (ok, mensaje): el mensaje viene del SP
        /// cuando rechaza por falta de testigo/paciente, cantidad o saldo insuficiente.
        /// </summary>
        public async Task<(bool ok, string mensaje, int scm_Id)> RegistrarAsync(SustanciaMovimientoFormViewModel model, int userId)
        {
            try
            {
                var entity = new tbSustanciaMovimiento
                {
                    itm_Id = model.itm_Id,
                    scm_Tipo = model.scm_Tipo,
                    scm_Cantidad = model.scm_Cantidad,
                    masc_Id = model.masc_Id,
                    scm_TestigoId = model.scm_TestigoId,
                    scm_Lote = model.scm_Lote,
                    scm_DeaSchedule = model.scm_DeaSchedule,
                    scm_Notas = model.scm_Notas,
                    scm_UsuarioCrea = userId
                };

                var (id, code, msg) = await _repo.RegistrarConMensajeAsync(entity);
                return (code > 0, msg, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (false, "Ocurrio un error al registrar el movimiento.", 0);
            }
        }

        public async Task<bool> RemoveAsync(int id, int userId)
        {
            try { return (await _repo.RemoveAsync(id, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        // ── Dropdowns ──────────────────────────────
        public List<FarmacoControladoOpcion> FarmacoDropdown()
        {
            try
            {
                return _repo.FarmacosDropdown()
                    .Select(x => new FarmacoControladoOpcion
                    {
                        itm_Id = x.itm_Id,
                        itm_Codigo = x.itm_Codigo,
                        itm_Descripcion = x.itm_Descripcion
                    }).ToList();
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<FarmacoControladoOpcion>(); }
        }

        public List<MascotaDropdownViewModel> MascotaDropdown()
        {
            try
            {
                var result = _repo.MascotasDropdown();
                return _mapper.Map<List<MascotaDropdownViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<MascotaDropdownViewModel>(); }
        }

        public async Task<List<TestigoOpcion>> TestigoDropdownAsync()
        {
            try
            {
                var result = await _repo.TestigosAsync();
                return result.Select(x => new TestigoOpcion { Id = x.Id, Nombre = x.Nombre }).ToList();
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<TestigoOpcion>(); }
        }
    }
}
