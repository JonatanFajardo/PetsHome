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
    public class DepartamentoService
    {
        private readonly DepartamentoRepository _departamentoRepository;
        private readonly IMapper _mapper;
        public DepartamentoService(DepartamentoRepository departamentoRepository, IMapper mapper)
        {
            _departamentoRepository = departamentoRepository;
            _mapper = mapper;
        }
        public async Task<List<DepartamentoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_General_Departamentos_ListResult> mappedResult = await _departamentoRepository.ListAsync();
                return _mapper.Map<List<DepartamentoViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<DepartamentoViewModel> FindAsync(int id)
        {
            try
            {
                PR_General_Departamentos_FindResult mappedResult = await _departamentoRepository.FindAsync(id);
                return _mapper.Map<DepartamentoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<DepartamentoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_General_Departamentos_DetailResult mappedResult = await _departamentoRepository.DetailAsync(id);
                return _mapper.Map<DepartamentoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(DepartamentoViewModel model)
        {
            try
            {
                tbDepartamentos mappedResult = _mapper.Map<tbDepartamentos>(model);
                return await _departamentoRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(DepartamentoViewModel model)
        {
            try
            {
                tbDepartamentos mappedResult = _mapper.Map<tbDepartamentos>(model);
                return await _departamentoRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _departamentoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
