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
    public class EmpleadoService
    {
        private readonly EmpleadoRepository _empleadoRepository;
        private readonly IMapper _mapper;
        public EmpleadoService(EmpleadoRepository empleadoRepository, IMapper mapper)
        {
            _empleadoRepository = empleadoRepository;
            _mapper = mapper;
        }
        public async Task<List<EmpleadoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Empleados_ListResult> mappedResult = await _empleadoRepository.ListAsync();
                return _mapper.Map<List<EmpleadoViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public async Task<EmpleadoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Empleados_FindResult mappedResult = await _empleadoRepository.FindAsync(id);
                return _mapper.Map<EmpleadoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<EmpleadoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Empleados_DetailResult mappedResult = await _empleadoRepository.DetailAsync(id);
                return _mapper.Map<EmpleadoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(EmpleadoViewModel model)
        {
            try
            {
                tbEmpleados mappedResult = await _mapper.Map<tbEmpleados>(mappedResult);
                return await _empleadoRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(EmpleadoViewModel model)
        {
            try
            {
                tbEmpleados mappedResult = await _mapper.Map<tbEmpleados>(mappedResult);
                return await _empleadoRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _empleadoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
