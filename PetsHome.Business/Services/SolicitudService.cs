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
    public class SolicitudService
    {
        private readonly SolicitudRepository _solicitudRepository;
        private readonly ILogger<SolicitudService> _logger;
        private readonly IMapper _mapper;
        public SolicitudService(SolicitudRepository solicitudeRepository, ILogger<SolicitudService> logger, IMapper mapper)
        {
            _solicitudRepository = solicitudeRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<SolicitudViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Solicitudes_ListResult> mappedResult = await _solicitudRepository.ListAsync();
                return _mapper.Map<List<SolicitudViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<SolicitudViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_FindResult mappedResult = await _solicitudRepository.FindAsync(id);
                return _mapper.Map<SolicitudViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<SolicitudViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_DetailResult mappedResult = await _solicitudRepository.DetailAsync(id);
                return _mapper.Map<SolicitudViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(SolicitudViewModel model)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                return await _solicitudRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(SolicitudViewModel model)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                return await _solicitudRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _solicitudRepository.RemoveAsync(id);
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
