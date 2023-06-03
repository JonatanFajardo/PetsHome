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
    public class HistorialMedicoService
    {
        private readonly HistorialMedicoRepository _historialmedicoRepository;
        private readonly ILogger<HistorialMedicoService> _logger;
        private readonly IMapper _mapper;

        public HistorialMedicoService(HistorialMedicoRepository historialmedicoRepository, ILogger<HistorialMedicoService> logger, IMapper mapper)
        {
            _historialmedicoRepository = historialmedicoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<HistorialMedicoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_HistorialMedico_ListResult> mappedResult = await _historialmedicoRepository.ListAsync();
                return _mapper.Map<List<HistorialMedicoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<HistorialMedicoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_HistorialMedico_FindResult mappedResult = await _historialmedicoRepository.FindAsync(id);
                return _mapper.Map<HistorialMedicoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<HistorialMedicoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_HistorialMedico_DetailResult mappedResult = await _historialmedicoRepository.DetailAsync(id);
                return _mapper.Map<HistorialMedicoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<Boolean> AddAsync(HistorialMedicoViewModel model)
        {
            try
            {
                tbHistorialMedico mappedResult = _mapper.Map<tbHistorialMedico>(model);
                return await _historialmedicoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<Boolean> UpdateAsync(HistorialMedicoViewModel model)
        {
            try
            {
                tbHistorialMedico mappedResult = _mapper.Map<tbHistorialMedico>(model);
                return await _historialmedicoRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _historialmedicoRepository.RemoveAsync(id);
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