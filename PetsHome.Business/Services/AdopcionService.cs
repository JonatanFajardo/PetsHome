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
    public class AdopcionService
    {
        private readonly AdopcionRepository _adopcionRepository;
        private readonly IMapper _mapper;
        public AdopcionService(AdopcionRepository adopcionRepository, IMapper mapper)
        {
            _adopcionRepository = adopcionRepository;
            _mapper = mapper;
        }
        public async Task<List<AdopcionViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Adopciones_List_Result> mappedResult = await _adopcionRepository.ListAsync();
                return _mapper.Map<List<AdopcionViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<AdopcionViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Adopciones_Find_Result > mappedResult = await _adopcionRepository.FindAsync(id);
                return _mapper.Map<AdopcionViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<AdopcionViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Adopciones_Detail_Result > mappedResult = await _adopcionRepository.DetailAsync(id);
                return _mapper.Map<AdopcionViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(AdopcionViewModel model)
        {
            try
            {
                tbAdopciones mappedResult = _mapper.Map<tbAdopciones>(model);
                return await _adopcionRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(AdopcionViewModel model)
        {
            try
            {
                tbAdopciones mappedResult = await _adopcionRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbAdopciones>(model);
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
                Boolean mappedResult = await _adopcionRepository.RemoveAsync(id);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
