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
    /// Clase que representa el servicio de departamento.
    /// </summary>
    public class DepartamentoService
    {
        private readonly LocalidadRepository _departamentoRepository;
        private readonly ILogger<DepartamentoService> _logger;
        private readonly IMapper _mapper;

        public DepartamentoService(LocalidadRepository departamentoRepository, ILogger<DepartamentoService> logger, IMapper mapper)
        {
            _departamentoRepository = departamentoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de departamentos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de departamentos.</returns>
        public async Task<List<DepartamentoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_General_Departamentos_ListResult> mappedResult = await _departamentoRepository.ListAsync();
                return _mapper.Map<List<DepartamentoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un departamento por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del departamento.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el departamento encontrado.</returns>
        public async Task<DepartamentoViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _departamentoRepository.FindAsync(id);
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un departamento por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del departamento.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles del departamento.</returns>
        public async Task<DepartamentoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_General_Departamentos_DetailResult mappedResult = await _departamentoRepository.DetailAsync(id);
                return _mapper.Map<DepartamentoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo departamento de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del departamento a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó el departamento correctamente.</returns>
        public async Task<Boolean> AddAsync(DepartamentoViewModel model)
        {
            try
            {
                tbDepartamentos mappedResult = _mapper.Map<tbDepartamentos>(model);
                return await _departamentoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un departamento de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del departamento a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó el departamento correctamente.</returns>
        public async Task<Boolean> UpdateAsync(DepartamentoViewModel model)
        {
            try
            {
                tbDepartamentos mappedResult = _mapper.Map<tbDepartamentos>(model);
                return await _departamentoRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un departamento por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del departamento a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó el departamento correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _departamentoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Obtiene un dropdown de departamentos.
        /// </summary>
        /// <returns>Una enumeración de DepartamentoViewModel que representa el dropdown de departamentos.</returns>
        public IEnumerable<DepartamentoViewModel> DepartamentoDropdown()
        {
            try
            {
                IEnumerable<PR_General_Departamentos_DropdownResult> mappedResult = _departamentoRepository.DepartamentoDropdown();
                return _mapper.Map<IEnumerable<DepartamentoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
