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
    public class EmpleadosCargoService
    {
        private readonly EmpleadosCargoRepository _empleadoscargoRepository;
        private readonly ILogger<EmpleadosCargoService> _logger;
        private readonly IMapper _mapper;
        public EmpleadosCargoService(EmpleadosCargoRepository empleadoscargoRepository, ILogger<EmpleadosCargoService> logger, IMapper mapper)
        {
            _empleadoscargoRepository = empleadoscargoRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<EmpleadoCargoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_EmpleadosCargos_ListResult> mappedResult = await _empleadoscargoRepository.ListAsync();
                return _mapper.Map<List<EmpleadoCargoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EmpleadoCargoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_EmpleadosCargos_FindResult mappedResult = await _empleadoscargoRepository.FindAsync(id);
                return _mapper.Map<EmpleadoCargoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EmpleadoCargoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_EmpleadosCargos_DetailResult mappedResult = await _empleadoscargoRepository.DetailAsync(id);
                return _mapper.Map<EmpleadoCargoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(EmpleadoCargoViewModel model)
        {
            try
            {
                tbEmpleadosCargos mappedResult = _mapper.Map<tbEmpleadosCargos>(model);
                return await _empleadoscargoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(EmpleadoCargoViewModel model)
        {
            try
            {
                tbEmpleadosCargos mappedResult = _mapper.Map<tbEmpleadosCargos>(model);
                return await _empleadoscargoRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _empleadoscargoRepository.RemoveAsync(id);
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
