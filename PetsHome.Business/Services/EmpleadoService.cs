using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Clase que representa el servicio de Empleado.
    /// </summary>
    public class EmpleadoService
    {
        private readonly EmpleadoRepository _empleadoRepository;
        private readonly ILogger<EmpleadoService> _logger;
        private readonly IMapper _mapper;

        public EmpleadoService(EmpleadoRepository empleadoRepository, ILogger<EmpleadoService> logger, IMapper mapper)
        {
            _empleadoRepository = empleadoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de Empleados de forma asincrona.
        /// </summary>
        public async Task<List<EmpleadoListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Empleados_ListResult> mappedResult = await _empleadoRepository.ListAsync();
                return _mapper.Map<List<EmpleadoListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                throw;
            }
        }

        /// <summary>
        /// Busca un Empleado por su ID de forma asincrona.
        /// </summary>
        public async Task<EmpleadoFormViewModel> FindAsync(int id)
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

        /// <summary>
        /// Obtiene los detalles de un Empleado por su ID de forma asincrona.
        /// </summary>
        public async Task<EmpleadoDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Empleados_DetailResult mappedResult = await _empleadoRepository.DetailAsync(id);
                EmpleadoDetailsViewModel detail = _mapper.Map<EmpleadoDetailsViewModel>(mappedResult);
                detail.emp_FechaCrea = mappedResult.per_FechaCrea;
                detail.emp_FechaModifica = mappedResult.per_FechaModifica;
                return detail;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo Empleado de forma asincrona.
        /// </summary>
        public async Task<bool> AddAsync(EmpleadoFormViewModel model)
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

        /// <summary>
        /// Actualiza un Empleado de forma asincrona.
        /// </summary>
        public async Task<bool> UpdateAsync(EmpleadoFormViewModel model)
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

        /// <summary>
        /// Elimina un Empleado por su ID de forma asincrona.
        /// </summary>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _empleadoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Obtiene una lista de opciones de cargo de empleados de forma asincrona.
        /// </summary>
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
