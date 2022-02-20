using AutoMapper;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MunicipioService> _logger;
        private readonly IMapper _mapper;
        public MunicipioService(MunicipioRepository municipioRepository, ILogger<MunicipioService> logger, IMapper mapper)
        {
            _municipioRepository = municipioRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<MunicipioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_General_Municipios_ListResult> mappedResult = await _municipioRepository.ListAsync();
                return _mapper.Map<List<MunicipioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<MunicipioViewModel> FindAsync(int id)
        {
            try
            {
                PR_General_Municipios_FindResult mappedResult = await _municipioRepository.FindAsync(id);
                return _mapper.Map<MunicipioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        //public async Task<MunicipioViewModel> DetailAsync(int id)
        //{
        //   try
        //    {
        //        IEnumerable<PR_tbMunicipios_DetailResult> mappedResult = await _municipioRepository.DetailAsync(id);
        //        return _mapper.Map<MunicipioViewModel>(mappedResult);
        //    }
        //    catch (Exception error)
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
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(MunicipioViewModel model)
        {
            try
            {
                tbMunicipios mappedResult = _mapper.Map<tbMunicipios>(model);
                return await _municipioRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _municipioRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
    }
}
