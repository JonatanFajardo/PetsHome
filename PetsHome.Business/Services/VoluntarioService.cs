using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class VoluntarioService
    {

        private readonly VoluntarioRepository _voluntarioRepository;
        private readonly ILogger<VoluntarioService> _logger;
        private readonly IMapper _mapper;
        public VoluntarioService(VoluntarioRepository voluntarioRepository, ILogger<VoluntarioService> logger, IMapper mapper)
        {
            _voluntarioRepository = voluntarioRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<VoluntarioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Voluntarios_ListResult> mappedResult = await _voluntarioRepository.ListAsync();
                return _mapper.Map<List<VoluntarioViewModel>>(mappedResult.ToList());
                //return MappingCustom.Map(mappedResult).ToList();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<VoluntarioViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Voluntarios_FindResult mappedResult = await _voluntarioRepository.FindAsync(id);
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<VoluntarioViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Voluntarios_DetailResult mappedResult = await _voluntarioRepository.DetailAsync(id);
                return _mapper.Map<VoluntarioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(VoluntarioViewModel model)
        {
            try
            {
                tbVoluntarios mappedResult = _mapper.Map<tbVoluntarios>(model); 
                return await _voluntarioRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(VoluntarioViewModel model)
        {
            try
            {
                tbVoluntarios mappedResult = _mapper.Map<tbVoluntarios>(model);
                return await _voluntarioRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _voluntarioRepository.RemoveAsync(id);
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
