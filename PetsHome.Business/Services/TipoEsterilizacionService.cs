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
    public class TipoEsterilizacionService
    {
        private readonly TipoEsterilizacionRepository _tipoEsterilizacionRepository;
        private readonly ILogger<TipoEsterilizacionService> _logger;
        private readonly IMapper _mapper;

        public TipoEsterilizacionService(TipoEsterilizacionRepository tipoEsterilizacionRepository, ILogger<TipoEsterilizacionService> logger, IMapper mapper)
        {
            _tipoEsterilizacionRepository = tipoEsterilizacionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<TipoEsterilizacionViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_TiposEsterilizacion_ListResult> mappedResult = await _tipoEsterilizacionRepository.ListAsync();
                return _mapper.Map<List<TipoEsterilizacionViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TipoEsterilizacionViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_TiposEsterilizacion_FindResult mappedResult = await _tipoEsterilizacionRepository.FindAsync(id);
                return _mapper.Map<TipoEsterilizacionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<TipoEsterilizacionViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_TiposEsterilizacion_DetailResult mappedResult = await _tipoEsterilizacionRepository.DetailAsync(id);
                return _mapper.Map<TipoEsterilizacionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(TipoEsterilizacionViewModel model)
        {
            try
            {
                tbTiposEsterilizacion mappedResult = _mapper.Map<tbTiposEsterilizacion>(model);
                return await _tipoEsterilizacionRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TipoEsterilizacionViewModel model)
        {
            try
            {
                tbTiposEsterilizacion mappedResult = _mapper.Map<tbTiposEsterilizacion>(model);
                return await _tipoEsterilizacionRepository.EditAsync(mappedResult);
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
                bool mappedResult = await _tipoEsterilizacionRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}
