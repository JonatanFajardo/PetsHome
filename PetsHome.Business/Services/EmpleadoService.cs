using AutoMapper;
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
        /// Obtiene una lista de Empleados de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de Empleados.</returns>
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

        /// <summary>
        /// Busca un Empleado por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del Empleado.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el Empleado encontrado.</returns>
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

        /// <summary>
        /// Obtiene los detalles de un Empleado por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del Empleado.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles del Empleado.</returns>
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

        /// <summary>
        /// Agrega un nuevo Empleado de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del Empleado a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó el Empleado correctamente.</returns>
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

        /// <summary>
        /// Actualiza un Empleado de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del Empleado a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó el Empleado correctamente.</returns>
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

        /// <summary>
        /// Elimina un Empleado por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del Empleado a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó el Empleado correctamente.</returns>
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

        /// <summary>
        /// Obtiene una lista de opciones de cargo de empleados de forma asíncrona.
        /// </summary>
        /// <returns>Una enumeración de las opciones de cargo de empleados.</returns>
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
