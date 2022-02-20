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
    public class EntradaService
    {
        private readonly EntradaRepository _entradaRepository;
        private readonly ILogger<EntradaService> _logger;
        private readonly IMapper _mapper;
        public EntradaService(EntradaRepository entradaRepository, ILogger<EntradaService> logger, IMapper mapper)
        {
            _entradaRepository = entradaRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<EntradaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Entradas_ListResult> mappedResult = await _entradaRepository.ListAsync();
                return _mapper.Map<List<EntradaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EntradaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Entradas_FindResult mappedResult = await _entradaRepository.FindAsync(id);
                return _mapper.Map<EntradaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EntradaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Entradas_DetailResult mappedResult = await _entradaRepository.DetailAsync(id);
                return _mapper.Map<EntradaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(EntradaViewModel model)
        {
            try
            {
                tbEntradas mappedResult = _mapper.Map<tbEntradas>(model);
                return await _entradaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<Boolean> UpdateAsync(EntradaViewModel model)
        {
            try
            {
                tbEntradas mappedResult = _mapper.Map<tbEntradas>(model);
                return await _entradaRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _entradaRepository.RemoveAsync(id);
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
