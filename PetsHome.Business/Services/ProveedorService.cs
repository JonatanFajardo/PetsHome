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
    /// Servicio del catalogo de proveedores de inventario.
    /// </summary>
    public class ProveedorService
    {
        private readonly ProveedorRepository _repo;
        private readonly ILogger<ProveedorService> _logger;
        private readonly IMapper _mapper;

        public ProveedorService(ProveedorRepository repo, ILogger<ProveedorService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ProveedoresListViewModel>> ListAsync()
        {
            try
            {
                var result = await _repo.ListAsync();
                return _mapper.Map<List<ProveedoresListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<List<ProveedoresDropdownViewModel>> DropdownAsync()
        {
            try
            {
                var result = await _repo.DropdownAsync();
                return _mapper.Map<List<ProveedoresDropdownViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ProveedoresFindViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _repo.FindAsync(id);
                return _mapper.Map<ProveedoresFindViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ProveedoresDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var result = await _repo.DetailAsync(id);
                return _mapper.Map<ProveedoresDetailViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<bool> AddAsync(ProveedoresFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbProveedores>(model);
                entity.prov_UsuarioCrea = userId;
                entity.prov_FechaCrea = DateTime.Now;
                return (await _repo.AddAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> UpdateAsync(ProveedoresFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbProveedores>(model);
                entity.prov_UsuarioModifica = userId;
                entity.prov_FechaModifica = DateTime.Now;
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

        public async Task<bool> NombreExistsAsync(string nombre, int id)
        {
            try
            {
                return await _repo.NombreExistsAsync(nombre, id);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }
    }
}
