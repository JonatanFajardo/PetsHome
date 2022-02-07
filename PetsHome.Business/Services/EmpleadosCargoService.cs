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
    public class EmpleadosCargoService
    {
        private readonly EmpleadosCargoRepository _empleadoscargoRepository;
        private readonly IMapper _mapper;
        public EmpleadosCargoService(EmpleadosCargoRepository empleadoscargoRepository, IMapper mapper)
        {
            _empleadoscargoRepository = empleadoscargoRepository;
            _mapper = mapper;
        }
        public async Task<List<EmpleadoCargoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_EmpleadosCargos_ListResult> mappedResult = await _empleadoscargoRepository.ListAsync();
                return _mapper.Map<List<EmpleadoCargoViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<EmpleadoCargoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_EmpleadosCargos_FindResult mappedResult = await _empleadoscargoRepository.FindAsync(id);
                return _mapper.Map<EmpleadoCargoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<EmpleadoCargoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_EmpleadosCargos_DetailResult mappedResult = await _empleadoscargoRepository.DetailAsync(id);
                return _mapper.Map<EmpleadoCargoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(EmpleadoCargoViewModel model)
        {
            try
            {
                tbEmpleadosCargos mappedResult = _mapper.Map<tbEmpleadosCargos>(model);
                return await _empleadoscargoRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(EmpleadoCargoViewModel model)
        {
            try
            {
                tbEmpleadosCargos mappedResult = _mapper.Map<tbEmpleadosCargos>(model);
                return await _empleadoscargoRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _empleadoscargoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
