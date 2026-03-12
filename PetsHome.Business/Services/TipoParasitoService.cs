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
    public class TipoParasitoService
    {
        private readonly TipoParasitoRepository _tipoParasitoRepository;
        private readonly ILogger<TipoParasitoService> _logger;
        private readonly IMapper _mapper;

        public TipoParasitoService(TipoParasitoRepository tipoParasitoRepository, ILogger<TipoParasitoService> logger, IMapper mapper)
        {
            _tipoParasitoRepository = tipoParasitoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<TipoParasitoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_TiposParasito_ListResult> mappedResult = await _tipoParasitoRepository.ListAsync();
                return _mapper.Map<List<TipoParasitoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TipoParasitoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_TiposParasito_FindResult mappedResult = await _tipoParasitoRepository.FindAsync(id);
                return _mapper.Map<TipoParasitoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TipoParasitoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_TiposParasito_DetailResult mappedResult = await _tipoParasitoRepository.DetailAsync(id);
                return _mapper.Map<TipoParasitoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(TipoParasitoViewModel model)
        {
            try
            {
                tbTiposParasito mappedResult = _mapper.Map<tbTiposParasito>(model);
                return (await _tipoParasitoRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TipoParasitoViewModel model)
        {
            try
            {
                tbTiposParasito mappedResult = _mapper.Map<tbTiposParasito>(model);
                return (await _tipoParasitoRepository.EditAsync(mappedResult)).Success;
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
                var mappedResult = await _tipoParasitoRepository.RemoveAsync(id);
                return mappedResult.Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public List<TipoParasitoViewModel> TipoParasitoDropdown()
        {
            try
            {
                IEnumerable<PR_Medico_TiposParasito_DropdownResult> mappedResult = _tipoParasitoRepository.TipoParasitoDropdown();
                return _mapper.Map<List<TipoParasitoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
