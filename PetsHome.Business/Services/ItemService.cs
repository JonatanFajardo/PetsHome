using AutoMapper;
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
        private readonly IMapper _mapper;
        public ItemService(ItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<List<ItemViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Items_ListResult> mappedResult = await _itemRepository.ListAsync();
                return _mapper.Map<List<ItemViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<ItemViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Items_FindResult mappedResult = await _itemRepository.FindAsync(id);
                return _mapper.Map<ItemViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<ItemViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Items_DetailResult mappedResult = await _itemRepository.DetailAsync(id);
                return _mapper.Map<ItemViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(ItemViewModel model)
        {
            try
            {
                tbItems mappedResult = await _itemRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbItems>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(ItemViewModel model)
        {
            try
            {
                tbItems mappedResult = await _itemRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbItems>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _itemRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
