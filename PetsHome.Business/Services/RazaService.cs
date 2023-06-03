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
    public class RazaService
    {
        private readonly ILogger<RazaService> _logger;
        private readonly IMapper _mapper;
        private readonly RazaRepository _razaRepository;

        public RazaService(RazaRepository razaRepository, ILogger<RazaService> logger, IMapper mapper)
        {
            _razaRepository = razaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<RazaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Razas_ListResult> mappedResult = await _razaRepository.ListAsync();
                return _mapper.Map<List<RazaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RazaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Razas_FindResult mappedResult = await _razaRepository.FindAsync(id);
                return _mapper.Map<RazaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RazaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Razas_DetailResult mappedResult = await _razaRepository.DetailAsync(id);
                return _mapper.Map<RazaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<Boolean> AddAsync(RazaViewModel model)
        {
            try
            {
                tbRazas mappedResult = _mapper.Map<tbRazas>(model);
                return await _razaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<Boolean> UpdateAsync(RazaViewModel model)
        {
            try
            {
                tbRazas mappedResult = _mapper.Map<tbRazas>(model);
                return await _razaRepository.EditAsync(mappedResult);
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
                return await _razaRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
    }
}