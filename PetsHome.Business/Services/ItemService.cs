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
        public async Task<Boolean> AddAsync(ItemViewModel model)
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
            
        public async Task<Boolean> UpdateAsync(ItemViewModel model)
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
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _itemRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        #region Dropdown
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
        #endregion Dropdown
    }
}
