using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class LaboratorioService
    {
        private readonly LaboratorioRepository _repo;
        private readonly ILogger<LaboratorioService> _logger;
        private readonly IMapper _mapper;

        public LaboratorioService(LaboratorioRepository repo,
            ILogger<LaboratorioService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        // ── Encabezado ─────────────────────────────
        public async Task<List<LaboratorioListViewModel>> ListAsync(int? mascId, int? vetId)
        {
            try
            {
                var result = await _repo.ListAsync(mascId, vetId);
                return _mapper.Map<List<LaboratorioListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<LaboratorioFindViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _repo.FindAsync(id);
                return _mapper.Map<LaboratorioFindViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<LaboratorioDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var header = await _repo.DetailAsync(id);
                if (header == null) return null;
                var vm = _mapper.Map<LaboratorioDetailViewModel>(header);
                var detalle = await _repo.DetalleListAsync(id);
                vm.Parametros = _mapper.Map<List<LaboratorioDetalleViewModel>>(detalle.ToList());
                return vm;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        /// <summary>Inserta un examen y devuelve su lab_Id (0 si falla).</summary>
        public async Task<int> AddAsync(LaboratorioFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbLaboratorio>(model);
                entity.lab_UsuarioCrea = userId;
                return await _repo.AddAsync(entity);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return 0; }
        }

        public async Task<bool> UpdateAsync(LaboratorioFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbLaboratorio>(model);
                entity.lab_UsuarioModifica = userId;
                return (await _repo.EditAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> CambiarEstadoAsync(int labId, string estado, int userId)
        {
            try { return (await _repo.CambiarEstadoAsync(labId, estado, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> RemoveAsync(int id, int userId)
        {
            try { return (await _repo.RemoveAsync(id, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        // ── Parametros ─────────────────────────────
        public async Task<bool> DetalleAddAsync(LaboratorioDetalleViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbLaboratorioDetalle>(model);
                entity.labd_UsuarioCrea = userId;
                return (await _repo.DetalleAddAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> DetalleRemoveAsync(int labdId, int userId)
        {
            try { return (await _repo.DetalleRemoveAsync(labdId, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        // ── Dropdown de mascotas ───────────────────
        public List<MascotaDropdownViewModel> MascotaDropdown()
        {
            try
            {
                var result = _repo.MascotasDropdown();
                return _mapper.Map<List<MascotaDropdownViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<MascotaDropdownViewModel>(); }
        }
    }
}
