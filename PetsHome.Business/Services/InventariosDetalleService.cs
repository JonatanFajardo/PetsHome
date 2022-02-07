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
    public class InventariosDetalleService
    {
        private readonly InventariosDetalleRepository _inventariosdetalleRepository;
        private readonly IMapper _mapper;
        public InventariosDetalleService(InventariosDetalleRepository inventariosdetalleRepository, IMapper mapper)
        {
            _inventariosdetalleRepository = inventariosdetalleRepository;
            _mapper = mapper;
        }
        public async Task<List<InventarioDetalleViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_InventariosDetalles_ListResult> mappedResult = await _inventariosdetalleRepository.ListAsync();
                return _mapper.Map<List<InventariosDetalleViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<InventariosDetalleViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_InventariosDetalles_FindResult mappedResult = await _inventariosdetalleRepository.FindAsync(id);
                return _mapper.Map<InventariosDetalleViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<InventariosDetalleViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_InventariosDetalles_DetailResult mappedResult = await _inventariosdetalleRepository.DetailAsync(id);
                return _mapper.Map < InventariosDetalleViewModel >> (mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(InventariosDetallViewModel model)
        {
            try
            {
                tbInventariosDetalles mappedResult = await _inventariosdetalleRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbInventariosDetalles>(model);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(InventariosDetallViewModel model)
        {
            try
            {
                tbInventariosDetalles mappedResult = await _inventariosdetalleRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbInventariosDetalles>(model);
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
                Boolean mappedResult = await _inventariosdetalleRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
