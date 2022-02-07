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
    public class CategoriaService
    {
        private readonly CategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        public CategoriaService(CategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }
        public async Task<List<CategoriaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Categorias_ListResult> mappedResult = await _categoriaRepository.ListAsync();
                return _mapper.Map<List<CategoriaViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<CategoriaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Categorias_FindResult mappedResult = await _categoriaRepository.FindAsync(id);
                return _mapper.Map<CategoriaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<CategoriaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Categorias_DetailResult mappedResult = await _categoriaRepository.DetailAsync(id);
                return _mapper.Map<CategoriaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(CategoriaViewModel model)
        {
            try
            {
                tbCategorias mappedResult = _mapper.Map<tbCategorias>(model);
                return await _categoriaRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(CategoriaViewModel model)
        {
            try
            {
                tbCategorias mappedResult = _categoriaRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbCategorias>(model);
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
                Boolean mappedResult = await _categoriaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
