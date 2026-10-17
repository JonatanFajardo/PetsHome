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
    public class ServiciosService
    {
        private readonly ServiciosRepository _repo;
        private readonly ILogger<ServiciosService> _logger;
        private readonly IMapper _mapper;

        public ServiciosService(ServiciosRepository repo,
            ILogger<ServiciosService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ServiciosListViewModel>> ListAsync()
        {
            try
            {
                var result = await _repo.ListAsync();
                return _mapper.Map<List<ServiciosListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ServiciosFindViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _repo.FindAsync(id);
                return _mapper.Map<ServiciosFindViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ServiciosDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var result = await _repo.DetailAsync(id);
                return _mapper.Map<ServiciosDetailViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<bool> AddAsync(ServiciosFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbServicios>(model);
                entity.serv_UsuarioCrea = userId;
                return (await _repo.AddAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> UpdateAsync(ServiciosFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbServicios>(model);
                entity.serv_UsuarioModifica = userId;
                return (await _repo.EditAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return (await _repo.RemoveAsync(id)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }
    }
}
