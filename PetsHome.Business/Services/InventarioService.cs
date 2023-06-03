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
    public class InventarioService
    {
        private readonly InventarioRepository _inventarioRepository;
        private readonly ILogger<InventarioService> _logger;
        private readonly IMapper _mapper;

        public InventarioService(InventarioRepository inventarioRepository, ILogger<InventarioService> logger, IMapper mapper)
        {
            _inventarioRepository = inventarioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<InventarioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Inventarios_ListResult> mappedResult = await _inventarioRepository.ListAsync();
                return _mapper.Map<List<InventarioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<InventarioViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Inventarios_FindResult mappedResult = await _inventarioRepository.FindAsync(id);
                return _mapper.Map<InventarioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<InventarioViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Inventarios_DetailResult mappedResult = await _inventarioRepository.DetailAsync(id);
                return _mapper.Map<InventarioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<Boolean> AddAsync(InventarioViewModel model)
        {
            try
            {
                tbInventarios mappedResult = _mapper.Map<tbInventarios>(model);
                return await _inventarioRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<Boolean> UpdateAsync(InventarioViewModel model)
        {
            try
            {
                tbInventarios mappedResult = _mapper.Map<tbInventarios>(model);
                return await _inventarioRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _inventarioRepository.RemoveAsync(id);
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