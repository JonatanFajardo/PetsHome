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
    /// Servicio que gestiona el inventario de productos.
    /// </summary>
    public class InventarioService
    {
        private readonly InventarioRepository _inventarioRepository;
        private readonly ILogger<InventarioService> _logger;
        private readonly IMapper _mapper;

        public InventarioService(InventarioRepository inventarioRepository, ILogger<InventarioService> logger, IMapper mapper)
        {
            _inventarioRepository = inventarioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los registros del inventario.
        /// </summary>
        /// <returns>Una lista de objetos InventarioViewModel.</returns>
        public async Task<List<InventarioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Inventarios_ListResult> mappedResult = await _inventarioRepository.ListAsync();
                return _mapper.Map<List<InventarioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un registro del inventario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del inventario.</param>
        /// <returns>Un objeto InventarioViewModel que corresponde al registro encontrado.</returns>
        public async Task<InventarioViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Inventarios_FindResult mappedResult = await _inventarioRepository.FindAsync(id);
                return _mapper.Map<InventarioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un registro del inventario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del inventario.</param>
        /// <returns>Un objeto InventarioViewModel que contiene los detalles del registro del inventario.</returns>
        public async Task<InventarioViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Inventarios_DetailResult mappedResult = await _inventarioRepository.DetailAsync(id);
                return _mapper.Map<InventarioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo registro al inventario.
        /// </summary>
        /// <param name="model">Datos del registro del inventario a agregar.</param>
        /// <returns>True si el registro del inventario se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(InventarioViewModel model)
        {
            try
            {
                tbInventarios mappedResult = _mapper.Map<tbInventarios>(model);
                return await _inventarioRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un registro existente del inventario.
        /// </summary>
        /// <param name="model">Datos actualizados del registro del inventario.</param>
        /// <returns>True si el registro del inventario se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(InventarioViewModel model)
        {
            try
            {
                tbInventarios mappedResult = _mapper.Map<tbInventarios>(model);
                return await _inventarioRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un registro del inventario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del inventario a eliminar.</param>
        /// <returns>True si el registro del inventario se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _inventarioRepository.RemoveAsync(id);
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