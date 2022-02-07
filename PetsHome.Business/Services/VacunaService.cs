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
    public class VacunaService
    {
        private readonly VacunaRepository _vacunaRepository;
        private readonly IMapper _mapper;
        public VacunaService(VacunaRepository vacunaRepository, IMapper mapper)
        {
            _vacunaRepository = vacunaRepository;
            _mapper = mapper;
        }
        public async Task<List<PR_Refugio_Vacunas_ListResult>> ListAsync()
        {
            try
            {
                IEnumerable<List> mappedResult = await _vacunaRepository.ListAsync();
                return _mapper.Map<List<VacunaViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PR_Refugio_Vacunas_FindResult> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Vacunas_FindResult mappedResult = await _vacunaRepository.FindAsync(id);
                return _mapper.Map<VacunaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PR_Refugio_Vacunas_DetailResult> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Vacunas_DetailResult mappedResult = await _vacunaRepository.DetailAsync(id);
                return _mapper.Map<VacunaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(VacunaViewModel model)
        {
            try
            {
                tbVacunas mappedResult = _mapper.Map<tbVacunas>(model);
                return await _vacunaRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(VacunaViewModel model)
        {
            try
            {
                tbVacunas mappedResult = _mapper.Map<tbVacunas>(model);
                return await _vacunaRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _vacunaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
