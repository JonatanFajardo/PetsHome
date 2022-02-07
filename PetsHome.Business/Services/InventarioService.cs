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
    public class InventarioService
    {
        private readonly InventarioRepository _inventarioRepository;
        private readonly IMapper _mapper;
        public InventarioService(InventarioRepository inventarioRepository, IMapper mapper)
        {
            _inventarioRepository = inventarioRepository;
            _mapper = mapper;
        }
        public async Task<List<InventarioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Inventarios_ListResult> mappedResult = await _inventarioRepository.ListAsync();
                return _mapper.Map<List<InventarioViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<InventarioViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Inventarios_FindResult mappedResult = await _inventarioRepository.FindAsync(id);
                return _mapper.Map<InventarioViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<InventarioViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Inventarios_DetailResult mappedResult = await _inventarioRepository.DetailAsync(id);
                return _mapper.Map<InventarioViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(InventarioViewModel model)
        {
            try
            {
                tbInventarios mappedResult = await _inventarioRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbInventarios>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(InventarioViewModel model)
        {
            try
            {
                tbInventarios mappedResult = await _inventarioRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbInventarios>(mappedResult.ToList());
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
                Boolean mappedResult = await _inventarioRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
