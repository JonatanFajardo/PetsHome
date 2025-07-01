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
    /// Servicio que gestiona los elementos del inventario.
    /// </summary>
    public class ItemService
    {
        private readonly ItemRepository _itemRepository;
        private readonly ILogger<ItemService> _logger;
        private readonly IMapper _mapper;

        public ItemService(ItemRepository itemRepository, ILogger<ItemService> logger, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los elementos del inventario.
        /// </summary>
        /// <returns>Una lista de objetos ItemViewModel.</returns>
        public async Task<List<ItemViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Items_ListResult> mappedResult = await _itemRepository.ListAsync();
                return _mapper.Map<List<ItemViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un elemento del inventario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del elemento del inventario.</param>
        /// <returns>Un objeto ItemViewModel que corresponde al elemento encontrado.</returns>
        public async Task<ItemViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Items_FindResult mappedResult = await _itemRepository.FindAsync(id);
                return _mapper.Map<ItemViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un elemento del inventario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del elemento del inventario.</param>
        /// <returns>Un objeto ItemViewModel que contiene los detalles del elemento del inventario.</returns>
        public async Task<ItemViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Items_DetailResult mappedResult = await _itemRepository.DetailAsync(id);
                return _mapper.Map<ItemViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo elemento al inventario.
        /// </summary>
        /// <param name="model">Datos del elemento del inventario a agregar.</param>
        /// <returns>True si el elemento del inventario se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(ItemViewModel model)
        {
            try
            {
                tbItems mappedResult = _mapper.Map<tbItems>(model);
                return await _itemRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un elemento existente del inventario.
        /// </summary>
        /// <param name="model">Datos actualizados del elemento del inventario.</param>
        /// <returns>True si el elemento del inventario se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(ItemViewModel model)
        {
            try
            {
                tbItems mappedResult = _mapper.Map<tbItems>(model);
                return await _itemRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un elemento del inventario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del elemento del inventario a eliminar.</param>
        /// <returns>True si el elemento del inventario se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _itemRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        #region Dropdown

        /// <summary>
        /// Obtiene una lista de categorías para su uso en un dropdown.
        /// </summary>
        /// <returns>Una lista de objetos CategoriaViewModel para el dropdown.</returns>
        public IEnumerable<CategoriaViewModel> CategoriaDropdown()
        {
            try
            {
                IEnumerable<PR_Inventario_Categorias_DropdownResult> mappedResult = _itemRepository.CategoriaDropdown();
                return _mapper.Map<List<CategoriaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene una lista de elementos para su uso en un dropdown.
        /// </summary>
        /// <returns>Una lista de objetos ItemViewModel para el dropdown.</returns>
        public IEnumerable<ItemViewModel> ItemDropdown()
        {
            try
            {
                IEnumerable<PR_Inventario_Items_DropdownResult> mappedResult = _itemRepository.ItemDropdown();
                return _mapper.Map<IEnumerable<ItemViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        #endregion Dropdown
    }
}