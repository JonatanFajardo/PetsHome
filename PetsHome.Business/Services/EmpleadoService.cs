using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class EmpleadoService
    {
        private readonly EmpleadoRepository _empleadoRepository;
        private readonly ILogger<EmpleadoService> _logger;
        private readonly IMapper _mapper;
        public EmpleadoService(EmpleadoRepository empleadoRepository, ILogger<EmpleadoService> logger, IMapper mapper)
        {
            _empleadoRepository = empleadoRepository;
            _mapper = mapper;
            _logger = logger;

        }
        public async Task<List<EmpleadoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Empleados_ListResult> mappedResult = await _empleadoRepository.ListAsync();
                return _mapper.Map<List<EmpleadoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                throw;
            }
        }
        public async Task<EmpleadoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Empleados_FindResult mappedResult = await _empleadoRepository.FindAsync(id);
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EmpleadoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Empleados_DetailResult mappedResult = await _empleadoRepository.DetailAsync(id);
                return _mapper.Map<EmpleadoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(EmpleadoViewModel model)
        {
            try
            {
                tbEmpleados mappedResult = _mapper.Map<tbEmpleados>(model);
                return await _empleadoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(EmpleadoViewModel model)
        {
            try
            {
                tbEmpleados mappedResult = _mapper.Map<tbEmpleados>(model);
                return await _empleadoRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _empleadoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
        
        public IEnumerable<RefugioViewModel> RefugioDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Refugio_DropdownResult> mappedResult = _empleadoRepository.RefugioDropdown();
                return _mapper.Map<IEnumerable<RefugioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public IEnumerable<EmpleadoCargoViewModel> EmpleadoCargoDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_EmpleadosCargos_DropdownResult> mappedResult = _empleadoRepository.EmpleadoCargoDropdown();
                return _mapper.Map<IEnumerable<EmpleadoCargoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
