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
    public class ViaAdministracionService
    {
        private readonly ViaAdministracionRepository _viaAdministracionRepository;
        private readonly ILogger<ViaAdministracionService> _logger;
        private readonly IMapper _mapper;

        public ViaAdministracionService(ViaAdministracionRepository viaAdministracionRepository, ILogger<ViaAdministracionService> logger, IMapper mapper)
        {
            _viaAdministracionRepository = viaAdministracionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ViaAdministracionViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_ViasAdministracion_ListResult> mappedResult = await _viaAdministracionRepository.ListAsync();
                return _mapper.Map<List<ViaAdministracionViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<ViaAdministracionViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_ViasAdministracion_FindResult mappedResult = await _viaAdministracionRepository.FindAsync(id);
                return _mapper.Map<ViaAdministracionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<ViaAdministracionViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_ViasAdministracion_DetailResult mappedResult = await _viaAdministracionRepository.DetailAsync(id);
                return _mapper.Map<ViaAdministracionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(ViaAdministracionViewModel model)
        {
            try
            {
                tbViasAdministracion mappedResult = _mapper.Map<tbViasAdministracion>(model);
                return await _viaAdministracionRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> UpdateAsync(ViaAdministracionViewModel model)
        {
            try
            {
                tbViasAdministracion mappedResult = _mapper.Map<tbViasAdministracion>(model);
                return await _viaAdministracionRepository.EditAsync(mappedResult);
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
                bool mappedResult = await _viaAdministracionRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public List<ViaAdministracionViewModel> ViaAdministracionDropdown()
        {
            try
            {
                IEnumerable<PR_Medico_ViasAdministracion_ListResult> mappedResult = _viaAdministracionRepository.ViaAdministracionDropdown();
                return _mapper.Map<List<ViaAdministracionViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
