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
    public class ClientesService
    {
        private readonly ClientesRepository _repo;
        private readonly ILogger<ClientesService> _logger;
        private readonly IMapper _mapper;

        public ClientesService(ClientesRepository repo,
            ILogger<ClientesService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ClientesListViewModel>> ListAsync()
        {
            try
            {
                var result = await _repo.ListAsync();
                return _mapper.Map<List<ClientesListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ClientesFindViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _repo.FindAsync(id);
                return _mapper.Map<ClientesFindViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ClientesDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var result = await _repo.DetailAsync(id);
                return _mapper.Map<ClientesDetailViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<bool> AddAsync(ClientesFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbClientes>(model);
                entity.cli_UsuarioCrea = userId;
                return (await _repo.AddAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> UpdateAsync(ClientesFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbClientes>(model);
                entity.cli_UsuarioModifica = userId;
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
