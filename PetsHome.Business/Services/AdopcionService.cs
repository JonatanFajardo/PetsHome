using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PetsHome.Business.Services
{
    public class AdopcionService
    {
        private readonly AdopcionRepository _adopcionRepository;
        private readonly ILogger<AdopcionService> _logger;
        private readonly IMapper _mapper;
        public AdopcionService(AdopcionRepository adopcionRepository, ILogger<AdopcionService> logger, IMapper mapper)
        {
            _adopcionRepository = adopcionRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<AdopcionViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Adopciones_ListResult> mappedResult = await _adopcionRepository.ListAsync();
                return _mapper.Map<List<AdopcionViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<AdopcionViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Adopciones_FindResult mappedResult = await _adopcionRepository.FindAsync(id);
                return _mapper.Map<AdopcionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<AdopcionViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Adopciones_DetailResult mappedResult = await _adopcionRepository.DetailAsync(id);
                return _mapper.Map<AdopcionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(AdopcionViewModel model)
        {
            try
            {
                tbAdopciones mappedResult = _mapper.Map<tbAdopciones>(model);
                return await _adopcionRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(AdopcionViewModel model)
        {
            try
            {

                tbAdopciones mappedResult = _mapper.Map<tbAdopciones>(model);
                return await _adopcionRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _adopcionRepository.RemoveAsync(id);
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
