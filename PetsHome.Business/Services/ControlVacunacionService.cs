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
    public class ControlVacunacionService
    {
        private readonly ControlVacunacionRepository _repository;
        private readonly ILogger<ControlVacunacionService> _logger;

        public ControlVacunacionService(ControlVacunacionRepository repository, ILogger<ControlVacunacionService> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<ControlVacunacionViewModel> GetDashboardAsync()
        {
            try
            {
                var dashboard        = await _repository.DashboardAsync();
                var matrizVacunacion = await _repository.MatrizVacunacionAsync();

                return new ControlVacunacionViewModel
                {
                    Dashboard        = dashboard?.ToList()        ?? new List<PR_Medico_ControlVacunacion_DashboardResult>(),
                    MatrizVacunacion = matrizVacunacion?.ToList() ?? new List<PR_Medico_ControlVacunacion_MatrizVacunacionResult>(),
                };
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new ControlVacunacionViewModel();
            }
        }

        public async Task<IEnumerable<PR_Medico_ControlVacunacion_MatrizVacunacionResult>> MatrizVacunacionAsync()
        {
            try
            {
                return await _repository.MatrizVacunacionAsync()
                    ?? Enumerable.Empty<PR_Medico_ControlVacunacion_MatrizVacunacionResult>();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return Enumerable.Empty<PR_Medico_ControlVacunacion_MatrizVacunacionResult>();
            }
        }
    }
}
