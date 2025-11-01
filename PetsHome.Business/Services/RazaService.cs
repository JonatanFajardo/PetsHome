using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio que gestiona las razas.
    /// </summary>
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

        public async Task<List<RazaListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Razas_ListResult> mappedResult = await _razaRepository.ListAsync();
                return _mapper.Map<List<RazaListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RazaFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Razas_FindResult mappedResult = await _razaRepository.FindAsync(id);
                return _mapper.Map<RazaFormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RazaDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Razas_DetailResult mappedResult = await _razaRepository.DetailAsync(id);
                return _mapper.Map<RazaDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(RazaFormViewModel model, int userId)
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

        public async Task<bool> UpdateAsync(RazaFormViewModel model, int userId)
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

        public async Task<bool> RemoveAsync(int id)
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
