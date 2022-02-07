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
    public class MunicipioService
    {
        private readonly MunicipioRepository _municipioRepository;
        private readonly IMapper _mapper;
        public MunicipioService(MunicipioRepository municipioRepository, IMapper mapper)
        {
            _municipioRepository = municipioRepository;
            _mapper = mapper;
        }
        public async Task<List<MunicipioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_General_Municipios_ListResult> mappedResult = await _municipioRepository.ListAsync();
                return _mapper.Map<List<MunicipioViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<MunicipioViewModel> FindAsync(int id)
        {
            try
            {
                PR_General_Municipios_FindResult mappedResult = await _municipioRepository.FindAsync(id);
                return _mapper.Map<MunicipioViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        //public async Task<MunicipioViewModel> DetailAsync(int id)
        //{
        //   try
        //    {
        //        IEnumerable<PR_tbMunicipios_DetailResult> mappedResult = await _municipioRepository.DetailAsync(id);
        //        return _mapper.Map<MunicipioViewModel>(mappedResult);
        //    }
        //    catch (System.Exception)
        //    {

        //        throw;
        //    }
        //}
        public async Task<Boolean> AddAsync(MunicipioViewModel model)
        {
            try
            {
                tbMunicipios mappedResult = _mapper.Map<tbMunicipios>(model);
                return await _municipioRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(MunicipioViewModel model)
        {
            try
            {
                tbMunicipios mappedResult = _mapper.Map<tbMunicipios>(model);
                return await _municipioRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _municipioRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
