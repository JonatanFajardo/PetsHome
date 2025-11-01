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
    /// Clase que representa el servicio de adopci�n.
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
        /// Obtiene una lista de adopciones de forma as�ncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado contiene la lista de adopciones.</returns>
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
        /// Busca una adopci�n por su ID de forma as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la adopci�n.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado contiene la adopci�n encontrada.</returns>
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
        /// Obtiene los detalles de una adopci�n por su ID de forma as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la adopci�n.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado contiene los detalles de la adopci�n.</returns>
        public async Task<IEnumerable<AdopcionDetailsViewModel>> DetailAsync(int id)
        {
            try
            {
                IEnumerable<PR_Refugio_Adopciones_DetailResult> mappedResult = await _adopcionRepository.DetailAsync(id);
                return _mapper.Map<IEnumerable<AdopcionDetailsViewModel>>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva adopci�n de forma as�ncrona.
        /// </summary>
        /// <param name="model">El modelo de la adopci�n a agregar.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado indica si se agreg� la adopci�n correctamente.</returns>
        public async Task<Boolean> AddAsync(AdopcionViewModel model, int userId)
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
        /// Actualiza una adopci�n de forma as�ncrona.
        /// </summary>
        /// <param name="model">El modelo de la adopci�n a actualizar.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado indica si se actualiz� la adopci�n correctamente.</returns>
        public async Task<Boolean> UpdateAsync(AdopcionViewModel model, int userId)
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
        /// Elimina una adopci�n por su ID de forma as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la adopci�n a eliminar.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado indica si se elimin� la adopci�n correctamente.</returns>
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