using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio que gestiona los tipos de reportantes.
    /// </summary>
    public class ReportantesTipoService
    {
        private readonly ILogger<ReportantesTipoService> _logger;
        private readonly IMapper _mapper;
        private readonly ReportantesTipoRepository _reportantesTipoRepository;

        public ReportantesTipoService(ReportantesTipoRepository reportantesTipoRepository, ILogger<ReportantesTipoService> logger, IMapper mapper)
        {
            _reportantesTipoRepository = reportantesTipoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ReportantesTipoListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Rescate_ReportantesTipo_ListResult> mappedResult = await _reportantesTipoRepository.ListAsync();
                return _mapper.Map<List<ReportantesTipoListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<ReportantesTipoFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Rescate_ReportantesTipo_FindResult mappedResult = await _reportantesTipoRepository.FindAsync(id);
                return _mapper.Map<ReportantesTipoFormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<ReportantesTipoDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Rescate_ReportantesTipo_DetailResult mappedResult = await _reportantesTipoRepository.DetailAsync(id);
                return _mapper.Map<ReportantesTipoDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(ReportantesTipoFormViewModel model)
        {
            try
            {
                tbReportantesTipo mappedResult = _mapper.Map<tbReportantesTipo>(model);
                return await _reportantesTipoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> UpdateAsync(ReportantesTipoFormViewModel model)
        {
            try
            {
                tbReportantesTipo mappedResult = _mapper.Map<tbReportantesTipo>(model);
                return await _reportantesTipoRepository.EditAsync(mappedResult);
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
                return await _reportantesTipoRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public SelectList ReportantesTipoDropdown()
        {
            try
            {
                var items = _reportantesTipoRepository.DropdownAsync().Result;
                return new SelectList(items, "reptip_Id", "reptip_Descripcion");
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
