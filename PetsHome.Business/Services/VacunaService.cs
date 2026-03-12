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
    public class VacunaService
    {
        private readonly VacunaRepository _vacunaRepository;
        private readonly ILogger<VacunaService> _logger;
        private readonly IMapper _mapper;

        public VacunaService(VacunaRepository vacunaRepository, ILogger<VacunaService> logger, IMapper mapper)
        {
            _vacunaRepository = vacunaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<VacunaListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Vacunas_ListResult> mappedResult = await _vacunaRepository.ListAsync();
                return _mapper.Map<List<VacunaListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<VacunaFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Vacunas_FindResult mappedResult = await _vacunaRepository.FindAsync(id);
                return _mapper.Map<VacunaFormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<VacunaDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Vacunas_DetailResult mappedResult = await _vacunaRepository.DetailAsync(id);
                return _mapper.Map<VacunaDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(VacunaFormViewModel model, int userId)
        {
            try
            {
                tbVacunas mappedResult = _mapper.Map<tbVacunas>(model);
                mappedResult.vac_UsuarioCrea = userId;
                mappedResult.vac_FechaCrea = DateTime.Now;
                return (await _vacunaRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(VacunaFormViewModel model, int userId)
        {
            try
            {
                tbVacunas mappedResult = _mapper.Map<tbVacunas>(model);
                mappedResult.vac_UsuarioModifica = userId;
                mappedResult.vac_FechaModifica = DateTime.Now;
                return (await _vacunaRepository.EditAsync(mappedResult)).Success;
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
                var mappedResult = await _vacunaRepository.RemoveAsync(id);
                return mappedResult.Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }


        public IEnumerable<VacunaDropdownViewModel> VacunaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Vacunas_ListResult> mappedResult = _vacunaRepository.Dropdown();
                return _mapper.Map<List<VacunaDropdownViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<VacunaDropdownViewModel>();
            }
        }
    }
}
