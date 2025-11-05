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
    public class RecetaService
    {
        private readonly RecetaRepository _recetaRepository;
        private readonly ILogger<RecetaService> _logger;
        private readonly IMapper _mapper;

        public RecetaService(RecetaRepository recetaRepository, ILogger<RecetaService> logger, IMapper mapper)
        {
            _recetaRepository = recetaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<RecetaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_Recetas_ListResult> mappedResult = await _recetaRepository.ListAsync();
                return _mapper.Map<List<RecetaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RecetaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_Recetas_FindResult mappedResult = await _recetaRepository.FindAsync(id);
                return _mapper.Map<RecetaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RecetaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_Recetas_DetailResult mappedResult = await _recetaRepository.DetailAsync(id);
                return _mapper.Map<RecetaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(RecetaViewModel model)
        {
            try
            {
                tbRecetas mappedResult = _mapper.Map<tbRecetas>(model);
                return await _recetaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> UpdateAsync(RecetaViewModel model)
        {
            try
            {
                tbRecetas mappedResult = _mapper.Map<tbRecetas>(model);
                return await _recetaRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _recetaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public IEnumerable<object> RecetaDropdown(int? masc_Id = null)
        {
            try
            {
                IEnumerable<PR_Medico_Recetas_DropdownResult> mappedResult = _recetaRepository.Dropdown(masc_Id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
