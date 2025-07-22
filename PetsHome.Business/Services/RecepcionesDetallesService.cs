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
    /// Servicio que gestiona los detalles de las recepciones de mercancía.
    /// </summary>
    public class RecepcionesDetallesService
    {
        private readonly RecepcionesDetallesRepository _recepcionDetalleRepository;
        private readonly ILogger<RecepcionesDetallesService> _logger;
        private readonly IMapper _mapper;

        public RecepcionesDetallesService(
            RecepcionesDetallesRepository recepcionDetalleRepository,
            ILogger<RecepcionesDetallesService> logger,
            IMapper mapper)
        {
            _recepcionDetalleRepository = recepcionDetalleRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de detalles de una recepción específica.
        /// </summary>
        /// <param name="recepcionId">ID de la recepción.</param>
        /// <returns>Una lista de objetos RecepcionDetalleViewModel.</returns>
        public async Task<List<RecepcionDetalleViewModel>> ListByRecepcionAsync(int recepcionId)
        {
            try
            {
                var result = await _recepcionDetalleRepository.ListByRecepcionAsync(recepcionId);
                return _mapper.Map<List<RecepcionDetalleViewModel>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un detalle de recepción por su ID.
        /// </summary>
        /// <param name="id">ID del detalle de recepción.</param>
        /// <returns>Un objeto RecepcionDetalleViewModel o null si no se encuentra.</returns>
        public async Task<RecepcionDetalleViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _recepcionDetalleRepository.FindAsync(id);
                return _mapper.Map<RecepcionDetalleViewModel>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo detalle de recepción.
        /// </summary>
        /// <param name="model">Modelo del detalle de recepción a agregar.</param>
        /// <returns>True si se agregó correctamente, False en caso contrario.</returns>
        public async Task<bool> AddAsync(RecepcionDetalleViewModel model)
        {
            try
            {
                var entity = _mapper.Map<tbRecepcionesDetalles>(model);
                return await _recepcionDetalleRepository.AddAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza un detalle de recepción existente.
        /// </summary>
        /// <param name="model">Modelo del detalle de recepción a actualizar.</param>
        /// <returns>True si se actualizó correctamente, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RecepcionDetalleViewModel model)
        {
            try
            {
                var entity = _mapper.Map<tbRecepcionesDetalles>(model);
                return await _recepcionDetalleRepository.EditAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un detalle de recepción.
        /// </summary>
        /// <param name="id">ID del detalle de recepción a eliminar.</param>
        /// <returns>True si se eliminó correctamente, False en caso contrario.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return await _recepcionDetalleRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}