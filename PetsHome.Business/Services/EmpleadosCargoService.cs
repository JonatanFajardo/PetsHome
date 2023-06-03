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
    /// Clase que representa el servicio de EmpleadosCargo.
    /// </summary>
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

        /// <summary>
        /// Obtiene una lista de EmpleadosCargo de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de EmpleadosCargo.</returns>
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

        /// <summary>
        /// Busca un EmpleadoCargo por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del EmpleadoCargo.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el EmpleadoCargo encontrado.</returns>
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

        /// <summary>
        /// Obtiene los detalles de un EmpleadoCargo por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del EmpleadoCargo.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles del EmpleadoCargo.</returns>
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

        /// <summary>
        /// Agrega un nuevo EmpleadoCargo de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del EmpleadoCargo a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó el EmpleadoCargo correctamente.</returns>
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

        /// <summary>
        /// Actualiza un EmpleadoCargo de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del EmpleadoCargo a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó el EmpleadoCargo correctamente.</returns>
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

        /// <summary>
        /// Elimina un EmpleadoCargo por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del EmpleadoCargo a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó el EmpleadoCargo correctamente.</returns>
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