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
    public class InventariosDetalleService
    {
        private readonly InventariosDetalleRepository _inventariosdetalleRepository;
        private readonly ILogger<InventariosDetalleService> _logger;
        private readonly IMapper _mapper;

        public InventariosDetalleService(InventariosDetalleRepository inventariosdetalleRepository, ILogger<InventariosDetalleService> logger, IMapper mapper)
        {
            _inventariosdetalleRepository = inventariosdetalleRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<InventarioDetalleViewModel>> ListAsync()
        {
            try
            {
                var mappedResult = await _inventariosdetalleRepository.ListAsync();
                return _mapper.Map<List<InventarioDetalleViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<InventarioDetalleViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _inventariosdetalleRepository.FindAsync(id);
                return _mapper.Map<InventarioDetalleViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<InventarioDetalleViewModel> DetailAsync(int id)
        {
            try
            {
                var mappedResult = await _inventariosdetalleRepository.DetailAsync(id);
                return _mapper.Map<InventarioDetalleViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(InventarioDetalleViewModel model)
        {
            try
            {
                tbInventariosDetalles mappedResult = _mapper.Map<tbInventariosDetalles>(model);
                return await _inventariosdetalleRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> UpdateAsync(InventarioDetalleViewModel model)
        {
            try
            {
                tbInventariosDetalles mappedResult = _mapper.Map<tbInventariosDetalles>(model);
                return await _inventariosdetalleRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _inventariosdetalleRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
    }
}