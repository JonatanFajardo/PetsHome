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
    public class ConsentimientoService
    {
        private readonly ConsentimientoRepository _repo;
        private readonly ILogger<ConsentimientoService> _logger;
        private readonly IMapper _mapper;

        public ConsentimientoService(ConsentimientoRepository repo,
            ILogger<ConsentimientoService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ConsentimientoListViewModel>> ListAsync(int? mascId, int? cliId, string estado)
        {
            try
            {
                var result = await _repo.ListAsync(mascId, cliId, estado);
                return _mapper.Map<List<ConsentimientoListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ConsentimientoFindViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _repo.FindAsync(id);
                return _mapper.Map<ConsentimientoFindViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ConsentimientoDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var header = await _repo.DetailAsync(id);
                if (header == null) return null;
                return _mapper.Map<ConsentimientoDetailViewModel>(header);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        /// <summary>Inserta un consentimiento y devuelve su cons_Id (0 si falla).</summary>
        public async Task<int> AddAsync(ConsentimientoFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbConsentimiento>(model);
                entity.cons_UsuarioCrea = userId;
                return await _repo.AddAsync(entity);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return 0; }
        }

        public async Task<bool> UpdateAsync(ConsentimientoFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbConsentimiento>(model);
                entity.cons_UsuarioModifica = userId;
                return (await _repo.EditAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> CambiarEstadoAsync(int consId, string estado, string firmante, int userId)
        {
            try { return (await _repo.CambiarEstadoAsync(consId, estado, firmante, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> RemoveAsync(int id, int userId)
        {
            try { return (await _repo.RemoveAsync(id, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
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
    }
}
