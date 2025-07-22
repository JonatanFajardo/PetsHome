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
    /// Servicio que gestiona las salidas de inventario.
    /// </summary>
    public class SalidaService
    {
        private readonly SalidasRepository _salidaRepository;
        private readonly ExistenciasRepository _existenciasRepository;
        private readonly ILogger<SalidaService> _logger;
        private readonly IMapper _mapper;

        public SalidaService(
            SalidasRepository salidaRepository,
            ExistenciasRepository existenciasRepository,
            ILogger<SalidaService> logger,
            IMapper mapper)
        {
            _salidaRepository = salidaRepository;
            _existenciasRepository = existenciasRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todas las salidas.
        /// </summary>
        /// <returns>Una lista de objetos SalidaViewModel.</returns>
        public async Task<List<SalidaViewModel>> ListAsync()
        {
            try
            {
                var result = await _salidaRepository.ListAsync();
                return _mapper.Map<List<SalidaViewModel>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una salida por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la salida.</param>
        /// <returns>Un objeto SalidaViewModel que corresponde al registro encontrado.</returns>
        public async Task<SalidaViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _salidaRepository.FindAsync(id);
                return _mapper.Map<SalidaViewModel>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una salida por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la salida.</param>
        /// <returns>Un objeto SalidaViewModel que contiene los detalles de la salida.</returns>
        public async Task<SalidaViewModel> DetailAsync(int id)
        {
            try
            {
                var result = await _salidaRepository.DetailAsync(id);
                return _mapper.Map<SalidaViewModel>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva salida.
        /// </summary>
        /// <param name="model">Datos de la salida a agregar.</param>
        /// <returns>True si la salida se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(SalidaViewModel model)
        {
            try
            {
                tbSalidas entity = _mapper.Map<tbSalidas>(model);
                return await _salidaRepository.AddAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza una salida existente.
        /// </summary>
        /// <param name="model">Datos actualizados de la salida.</param>
        /// <returns>True si la salida se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(SalidaViewModel model)
        {
            try
            {
                tbSalidas entity = _mapper.Map<tbSalidas>(model);
                return await _salidaRepository.EditAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina una salida por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la salida a eliminar.</param>
        /// <returns>True si la salida se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return await _salidaRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Procesa una salida completa con sus detalles y actualiza las existencias.
        /// </summary>
        /// <param name="salida">Datos de la salida.</param>
        /// <param name="detalles">Lista de detalles de la salida.</param>
        /// <returns>True si el proceso se completó correctamente.</returns>
        public async Task<bool> ProcesarSalidaCompleta(
            SalidaViewModel salida, 
            List<SalidaDetalleViewModel> detalles)
        {
            try
            {
                // 1. Validar disponibilidad de stock
                foreach (var detalle in detalles)
                {
                    var existencia = await _existenciasRepository.GetByItemAndRefugioAsync(
                        detalle.itm_Id, salida.refg_Id);
                    
                    if (existencia == null || existencia.exist_Stock < detalle.saldet_Cantidad)
                    {
                        _logger.LogWarning("Stock insuficiente para el ítem {ItemId}", detalle.itm_Id);
                        return false;
                    }
                }

                // 2. Guardar la salida principal
                var salidaCreada = await AddAsync(salida);
                if (!salidaCreada) return false;

                // 3. Procesar cada detalle y actualizar existencias
                foreach (var detalle in detalles)
                {
                    await ActualizarExistenciaPorSalida(
                        detalle.itm_Id, 
                        salida.refg_Id, 
                        detalle.saldet_Cantidad);
                }

                return true;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error procesando salida completa: {Message}", error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza las existencias de un ítem después de una salida.
        /// </summary>
        private async Task<bool> ActualizarExistenciaPorSalida(int itemId, int refugioId, int cantidad)
        {
            try
            {
                var existenciaActual = await _existenciasRepository.GetByItemAndRefugioAsync(itemId, refugioId);
                
                if (existenciaActual != null)
                {
                    int nuevoStock = existenciaActual.exist_Stock - cantidad;
                    if (nuevoStock < 0) nuevoStock = 0; // No permitir stock negativo
                    
                    return await _existenciasRepository.UpdateStockAsync(itemId, refugioId, nuevoStock);
                }
                
                return false; // No se puede hacer salida si no existe la existencia
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error actualizando existencia por salida: {Message}", error.Message);
                return false;
            }
        }

        /// <summary>
        /// Verifica la disponibilidad de stock para una lista de ítems.
        /// </summary>
        /// <param name="refugioId">ID del refugio.</param>
        /// <param name="itemsConCantidades">Diccionario con ItemId y cantidad solicitada.</param>
        /// <returns>True si hay stock suficiente para todos los ítems.</returns>
        public async Task<bool> VerificarDisponibilidadStock(int refugioId, Dictionary<int, int> itemsConCantidades)
        {
            try
            {
                foreach (var item in itemsConCantidades)
                {
                    var existencia = await _existenciasRepository.GetByItemAndRefugioAsync(item.Key, refugioId);
                    
                    if (existencia == null || existencia.exist_Stock < item.Value)
                    {
                        return false;
                    }
                }
                
                return true;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error verificando disponibilidad de stock: {Message}", error.Message);
                return false;
            }
        }
    }
}