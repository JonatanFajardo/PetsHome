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
    /// Servicio que gestiona los detalles de las salidas de mercancía.
    /// </summary>
    public class SalidasDetallesService
    {
        private readonly SalidasDetallesRepository _salidaDetalleRepository;
        private readonly ILogger<SalidasDetallesService> _logger;
        private readonly IMapper _mapper;

        public SalidasDetallesService(
            SalidasDetallesRepository salidaDetalleRepository,
            ILogger<SalidasDetallesService> logger,
            IMapper mapper)
        {
            _salidaDetalleRepository = salidaDetalleRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de detalles de una salida específica.
        /// </summary>
        /// <param name="salidaId">ID de la salida.</param>
        /// <returns>Una lista de objetos SalidaDetalleViewModel.</returns>
        public async Task<List<SalidaDetalleViewModel>> ListBySalidaAsync(int salidaId)
        {
            try
            {
                var result = await _salidaDetalleRepository.ListBySalidaAsync(salidaId);
                return _mapper.Map<List<SalidaDetalleViewModel>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un detalle de salida por su ID.
        /// </summary>
        /// <param name="id">ID del detalle de salida.</param>
        /// <returns>Un objeto SalidaDetalleViewModel o null si no se encuentra.</returns>
        public async Task<SalidaDetalleViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _salidaDetalleRepository.FindAsync(id);
                return _mapper.Map<SalidaDetalleViewModel>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo detalle de salida.
        /// </summary>
        /// <param name="model">Modelo del detalle de salida a agregar.</param>
        /// <returns>True si se agregó correctamente, False en caso contrario.</returns>
        public async Task<bool> AddAsync(SalidaDetalleViewModel model)
        {
            try
            {
                var entity = _mapper.Map<tbSalidasDetalles>(model);
                return await _salidaDetalleRepository.AddAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza un detalle de salida existente.
        /// </summary>
        /// <param name="model">Modelo del detalle de salida a actualizar.</param>
        /// <returns>True si se actualizó correctamente, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(SalidaDetalleViewModel model)
        {
            try
            {
                var entity = _mapper.Map<tbSalidasDetalles>(model);
                return await _salidaDetalleRepository.EditAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un detalle de salida.
        /// </summary>
        /// <param name="id">ID del detalle de salida a eliminar.</param>
        /// <returns>True si se eliminó correctamente, False en caso contrario.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return await _salidaDetalleRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}