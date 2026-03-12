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
    public class GravedadService
    {
        private readonly GravedadRepository _gravedadRepository;
        private readonly ILogger<GravedadService> _logger;
        private readonly IMapper _mapper;

        public GravedadService(GravedadRepository gravedadRepository, ILogger<GravedadService> logger, IMapper mapper)
        {
            _gravedadRepository = gravedadRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<GravedadViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_Gravedades_ListResult> mappedResult = await _gravedadRepository.ListAsync();
                return _mapper.Map<List<GravedadViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<GravedadViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_Gravedades_FindResult mappedResult = await _gravedadRepository.FindAsync(id);
                return _mapper.Map<GravedadViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<GravedadViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_Gravedades_DetailResult mappedResult = await _gravedadRepository.DetailAsync(id);
                return _mapper.Map<GravedadViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(GravedadViewModel model)
        {
            try
            {
                tbGravedades mappedResult = _mapper.Map<tbGravedades>(model);
                return (await _gravedadRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(GravedadViewModel model)
        {
            try
            {
                tbGravedades mappedResult = _mapper.Map<tbGravedades>(model);
                return (await _gravedadRepository.EditAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                var mappedResult = await _gravedadRepository.RemoveAsync(id);
                return mappedResult.Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}
