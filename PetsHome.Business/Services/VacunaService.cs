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
    public class VacunaService
    {
        private readonly VacunaRepository _vacunaRepository;
        private readonly ILogger<VacunaService> _logger;
        private readonly IMapper _mapper;
        public VacunaService(VacunaRepository vacunaRepository, ILogger<VacunaService> logger, IMapper mapper)
        {
            _vacunaRepository = vacunaRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<VacunaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Vacunas_ListResult> mappedResult = await _vacunaRepository.ListAsync();
                return _mapper.Map<List<VacunaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<VacunaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Vacunas_FindResult mappedResult = await _vacunaRepository.FindAsync(id);
                return _mapper.Map<VacunaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<VacunaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Vacunas_DetailResult mappedResult = await _vacunaRepository.DetailAsync(id);
                return _mapper.Map<VacunaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(VacunaViewModel model)
        {
            try
            {
                tbVacunas mappedResult = _mapper.Map<tbVacunas>(model);
                return await _vacunaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(VacunaViewModel model)
        {
            try
            {
                tbVacunas mappedResult = _mapper.Map<tbVacunas>(model);
                return await _vacunaRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _vacunaRepository.RemoveAsync(id);
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
