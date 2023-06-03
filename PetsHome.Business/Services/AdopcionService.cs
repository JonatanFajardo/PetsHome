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
    /// Clase que representa el servicio de adopción.
    /// </summary>
    public class AdopcionService
    {
        private readonly AdopcionRepository _adopcionRepository;
        private readonly ILogger<AdopcionService> _logger;
        private readonly IMapper _mapper;

        public AdopcionService(AdopcionRepository adopcionRepository, ILogger<AdopcionService> logger, IMapper mapper)
        {
            _adopcionRepository = adopcionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de adopciones de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de adopciones.</returns>
        public async Task<List<AdopcionViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Adopciones_ListResult> mappedResult = await _adopcionRepository.ListAsync();
                return _mapper.Map<List<AdopcionViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una adopción por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la adopción.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la adopción encontrada.</returns>
        public async Task<AdopcionViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Adopciones_FindResult mappedResult = await _adopcionRepository.FindAsync(id);
                return _mapper.Map<AdopcionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una adopción por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la adopción.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles de la adopción.</returns>
        public async Task<AdopcionViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Adopciones_DetailResult mappedResult = await _adopcionRepository.DetailAsync(id);
                return _mapper.Map<AdopcionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva adopción de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la adopción a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó la adopción correctamente.</returns>
        public async Task<Boolean> AddAsync(AdopcionViewModel model)
        {
            try
            {
                tbAdopciones mappedResult = _mapper.Map<tbAdopciones>(model);
                return await _adopcionRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una adopción de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la adopción a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó la adopción correctamente.</returns>
        public async Task<Boolean> UpdateAsync(AdopcionViewModel model)
        {
            try
            {
                tbAdopciones mappedResult = _mapper.Map<tbAdopciones>(model);
                return await _adopcionRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una adopción por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la adopción a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó la adopción correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _adopcionRepository.RemoveAsync(id);
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