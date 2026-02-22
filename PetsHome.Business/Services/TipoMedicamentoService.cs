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
    public class TipoMedicamentoService
    {
        private readonly TipoMedicamentoRepository _tipoMedicamentoRepository;
        private readonly ILogger<TipoMedicamentoService> _logger;
        private readonly IMapper _mapper;

        public TipoMedicamentoService(TipoMedicamentoRepository tipoMedicamentoRepository, ILogger<TipoMedicamentoService> logger, IMapper mapper)
        {
            _tipoMedicamentoRepository = tipoMedicamentoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<TipoMedicamentoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_TiposMedicamento_ListResult> mappedResult = await _tipoMedicamentoRepository.ListAsync();
                return _mapper.Map<List<TipoMedicamentoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TipoMedicamentoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_TiposMedicamento_FindResult mappedResult = await _tipoMedicamentoRepository.FindAsync(id);
                return _mapper.Map<TipoMedicamentoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TipoMedicamentoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_TiposMedicamento_DetailResult mappedResult = await _tipoMedicamentoRepository.DetailAsync(id);
                return _mapper.Map<TipoMedicamentoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(TipoMedicamentoViewModel model)
        {
            try
            {
                tbTiposMedicamento mappedResult = _mapper.Map<tbTiposMedicamento>(model);
                return await _tipoMedicamentoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TipoMedicamentoViewModel model)
        {
            try
            {
                tbTiposMedicamento mappedResult = _mapper.Map<tbTiposMedicamento>(model);
                return await _tipoMedicamentoRepository.EditAsync(mappedResult);
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
                bool mappedResult = await _tipoMedicamentoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public List<TipoMedicamentoViewModel> TipoMedicamentoDropdown()
        {
            try
            {
                IEnumerable<PR_Medico_TiposMedicamento_ListResult> mappedResult = _tipoMedicamentoRepository.TipoMedicamentoDropdown();
                return _mapper.Map<List<TipoMedicamentoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
