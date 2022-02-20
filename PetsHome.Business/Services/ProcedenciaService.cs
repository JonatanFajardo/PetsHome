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
    public class ProcedenciaService
    {
        private readonly ProcedenciaRepository _procedenciaRepository;
        private readonly ILogger<ProcedenciaService> _logger;
        private readonly IMapper _mapper;
        public ProcedenciaService(ProcedenciaRepository procedenciaRepository, ILogger<ProcedenciaService> logger, IMapper mapper)
        {
            _procedenciaRepository = procedenciaRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<ProcedenciaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Procedencias_ListResult> mappedResult = await _procedenciaRepository.ListAsync();
                return _mapper.Map<List<ProcedenciaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<ProcedenciaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Procedencias_FindResult mappedResult = await _procedenciaRepository.FindAsync(id);
                return _mapper.Map<ProcedenciaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<ProcedenciaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Procedencias_DetailResult mappedResult = await _procedenciaRepository.DetailAsync(id);
                return _mapper.Map<ProcedenciaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(ProcedenciaViewModel model)
        {
            try
            {
                tbProcedencias mappedResult = _mapper.Map<tbProcedencias>(model);
                return await _procedenciaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(ProcedenciaViewModel model)
        {
            try
            {
                tbProcedencias mappedResult = _mapper.Map<tbProcedencias>(model);
                return await _procedenciaRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _procedenciaRepository.RemoveAsync(id);
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
