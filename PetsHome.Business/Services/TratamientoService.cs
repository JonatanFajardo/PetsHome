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
    public class TratamientoService
    {
        private readonly TratamientoRepository _tratamientoRepository;
        private readonly ILogger<TratamientoService> _logger;
        private readonly IMapper _mapper;

        public TratamientoService(TratamientoRepository tratamientoRepository, ILogger<TratamientoService> logger, IMapper mapper)
        {
            _tratamientoRepository = tratamientoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<TratamientoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_Tratamientos_ListResult> mappedResult = await _tratamientoRepository.ListAsync();
                return _mapper.Map<List<TratamientoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TratamientoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_Tratamientos_FindResult mappedResult = await _tratamientoRepository.FindAsync(id);
                return _mapper.Map<TratamientoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TratamientoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_Tratamientos_DetailResult mappedResult = await _tratamientoRepository.DetailAsync(id);
                return _mapper.Map<TratamientoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(TratamientoViewModel model)
        {
            try
            {
                tbTratamientos mappedResult = _mapper.Map<tbTratamientos>(model);
                return await _tratamientoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> UpdateAsync(TratamientoViewModel model)
        {
            try
            {
                tbTratamientos mappedResult = _mapper.Map<tbTratamientos>(model);
                return await _tratamientoRepository.EditAsync(mappedResult);
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
                bool mappedResult = await _tratamientoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public IEnumerable<object> TratamientoDropdown(int? masc_Id = null)
        {
            try
            {
                IEnumerable<PR_Medico_Tratamientos_DropdownResult> mappedResult = _tratamientoRepository.Dropdown(masc_Id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
