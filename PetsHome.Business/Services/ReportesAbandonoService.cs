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
    /// Servicio que gestiona los reportes de abandono.
    /// </summary>
    public class ReportesAbandonoService
    {
        private readonly ILogger<ReportesAbandonoService> _logger;
        private readonly IMapper _mapper;
        private readonly ReportesAbandonoRepository _reportesAbandonoRepository;

        public ReportesAbandonoService(ReportesAbandonoRepository reportesAbandonoRepository, ILogger<ReportesAbandonoService> logger, IMapper mapper)
        {
            _reportesAbandonoRepository = reportesAbandonoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ReportesAbandonoListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Rescate_ReportesAbandono_ListResult> mappedResult = await _reportesAbandonoRepository.ListAsync();
                return _mapper.Map<List<ReportesAbandonoListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<ReportesAbandonoFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Rescate_ReportesAbandono_FindResult mappedResult = await _reportesAbandonoRepository.FindAsync(id);
                return _mapper.Map<ReportesAbandonoFormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<ReportesAbandonoDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Rescate_ReportesAbandono_DetailResult mappedResult = await _reportesAbandonoRepository.DetailAsync(id);
                return _mapper.Map<ReportesAbandonoDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(ReportesAbandonoFormViewModel model)
        {
            try
            {
                tbReportesAbandono mappedResult = _mapper.Map<tbReportesAbandono>(model);
                return await _reportesAbandonoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> UpdateAsync(ReportesAbandonoFormViewModel model)
        {
            try
            {
                tbReportesAbandono mappedResult = _mapper.Map<tbReportesAbandono>(model);
                return await _reportesAbandonoRepository.EditAsync(mappedResult);
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
                return await _reportesAbandonoRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public SelectList ReportesAbandonoDropdown()
        {
            try
            {
                var items = ListAsync().Result;
                return new SelectList(items, "repa_Id", "repa_UbicacionIncidente");
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
