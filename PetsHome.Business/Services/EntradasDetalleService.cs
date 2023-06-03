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
    /// Clase que representa el servicio de Detalle de Entradas.
    /// </summary>
    public class EntradasDetalleService
    {
        private readonly EntradasDetalleRepository _entradaDetalleRepository;
        private readonly ItemRepository _itemRepository;
        private readonly ILogger<EntradasDetalleService> _logger;
        private readonly IMapper _mapper;

        public EntradasDetalleService(EntradasDetalleRepository entradaDetalleRepository, ILogger<EntradasDetalleService> logger, IMapper mapper)
        {
            _entradaDetalleRepository = entradaDetalleRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de Detalles de Entradas por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la Entrada.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de Detalles de Entradas.</returns>
        public async Task<List<EntradaDetalleViewModel>> ListIdAsync(int id)
        {
            try
            {
                IEnumerable<PR_Inventario_EntradasDetalles_ListResult> mappedResult = await _entradaDetalleRepository.ListIdAsync(id);
                return _mapper.Map<List<EntradaDetalleViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un Detalle de Entrada por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del Detalle de Entrada.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el Detalle de Entrada encontrado.</returns>
        public async Task<EntradaViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _entradaDetalleRepository.FindAsync(id);
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo Detalle de Entrada de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del Detalle de Entrada a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó el Detalle de Entrada correctamente.</returns>
        public async Task<Boolean> AddAsync(EntradaDetalleViewModel model)
        {
            try
            {
                tbEntradasDetalles mappedResult = _mapper.Map<tbEntradasDetalles>(model);
                return await _entradaDetalleRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un Detalle de Entrada de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del Detalle de Entrada a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó el Detalle de Entrada correctamente.</returns>
        public async Task<Boolean> UpdateAsync(EntradaDetalleViewModel model)
        {
            try
            {
                tbEntradasDetalles mappedResult = _mapper.Map<tbEntradasDetalles>(model);
                return await _entradaDetalleRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un Detalle de Entrada por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del Detalle de Entrada a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó el Detalle de Entrada correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _entradaDetalleRepository.RemoveAsync(id);
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