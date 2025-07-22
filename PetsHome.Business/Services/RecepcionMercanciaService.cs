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
    /// Servicio que gestiona las recepciones de mercancía.
    /// </summary>
    public class RecepcionMercanciaService
    {
        private readonly RecepcionesMercanciaRepository _recepcionRepository;
        private readonly ExistenciasRepository _existenciasRepository;
        private readonly ILogger<RecepcionMercanciaService> _logger;
        private readonly IMapper _mapper;

        public RecepcionMercanciaService(
            RecepcionesMercanciaRepository recepcionRepository,
            ExistenciasRepository existenciasRepository,
            ILogger<RecepcionMercanciaService> logger,
            IMapper mapper)
        {
            _recepcionRepository = recepcionRepository;
            _existenciasRepository = existenciasRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todas las recepciones de mercancía.
        /// </summary>
        /// <returns>Una lista de objetos RecepcionMercanciaViewModel.</returns>
        public async Task<List<RecepcionMercanciaViewModel>> ListAsync()
        {
            try
            {
                var result = await _recepcionRepository.ListAsync();
                return _mapper.Map<List<RecepcionMercanciaViewModel>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una recepción de mercancía por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la recepción.</param>
        /// <returns>Un objeto RecepcionMercanciaViewModel que corresponde al registro encontrado.</returns>
        public async Task<RecepcionMercanciaViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _recepcionRepository.FindAsync(id);
                return _mapper.Map<RecepcionMercanciaViewModel>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una recepción de mercancía por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la recepción.</param>
        /// <returns>Un objeto RecepcionMercanciaViewModel que contiene los detalles de la recepción.</returns>
        public async Task<RecepcionMercanciaViewModel> DetailAsync(int id)
        {
            try
            {
                var result = await _recepcionRepository.DetailAsync(id);
                return _mapper.Map<RecepcionMercanciaViewModel>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva recepción de mercancía.
        /// </summary>
        /// <param name="model">Datos de la recepción a agregar.</param>
        /// <returns>True si la recepción se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(RecepcionMercanciaViewModel model)
        {
            try
            {
                tbRecepcionesMercancia entity = _mapper.Map<tbRecepcionesMercancia>(model);
                return await _recepcionRepository.AddAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza una recepción de mercancía existente.
        /// </summary>
        /// <param name="model">Datos actualizados de la recepción.</param>
        /// <returns>True si la recepción se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(RecepcionMercanciaViewModel model)
        {
            try
            {
                tbRecepcionesMercancia entity = _mapper.Map<tbRecepcionesMercancia>(model);
                return await _recepcionRepository.EditAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina una recepción de mercancía por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la recepción a eliminar.</param>
        /// <returns>True si la recepción se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return await _recepcionRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Procesa una recepción completa con sus detalles y actualiza las existencias.
        /// </summary>
        /// <param name="recepcion">Datos de la recepción.</param>
        /// <param name="detalles">Lista de detalles de la recepción.</param>
        /// <returns>True si el proceso se completó correctamente.</returns>
        public async Task<bool> ProcesarRecepcionCompleta(
            RecepcionMercanciaViewModel recepcion, 
            List<RecepcionDetalleViewModel> detalles)
        {
            try
            {
                // 1. Guardar la recepción principal
                var recepcionCreada = await AddAsync(recepcion);
                if (!recepcionCreada) return false;

                // 2. Procesar cada detalle y actualizar existencias
                foreach (var detalle in detalles)
                {
                    // Actualizar o crear existencia para el ítem
                    await ActualizarExistenciaPorRecepcion(
                        detalle.itm_Id, 
                        recepcion.refg_Id, 
                        detalle.recdet_Cantidad);
                }

                return true;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error procesando recepción completa: {Message}", error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza las existencias de un ítem después de una recepción.
        /// </summary>
        private async Task<bool> ActualizarExistenciaPorRecepcion(int itemId, int refugioId, int cantidad)
        {
            try
            {
                // Buscar existencia actual
                var existenciaActual = await _existenciasRepository.GetByItemAndRefugioAsync(itemId, refugioId);
                
                if (existenciaActual != null)
                {
                    // Si existe, sumar la cantidad recibida
                    int nuevoStock = existenciaActual.exist_Stock + cantidad;
                    return await _existenciasRepository.UpdateStockAsync(itemId, refugioId, nuevoStock);
                }
                else
                {
                    // Si no existe, crear nueva existencia
                    var nuevaExistencia = new tbExistencias
                    {
                        itm_Id = itemId,
                        refg_Id = refugioId,
                        exist_Stock = cantidad,
                        exist_StockMinimo = 10, // Valor por defecto
                        exist_StockMaximo = 1000, // Valor por defecto
                        exist_FechaActualizacion = DateTime.Now,
                        exist_EsEliminado = false
                    };
                    return await _existenciasRepository.AddAsync(nuevaExistencia);
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error actualizando existencia: {Message}", error.Message);
                return false;
            }
        }
    }
}