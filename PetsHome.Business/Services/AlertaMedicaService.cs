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
    public class AlertaMedicaService
    {
        private readonly AlertaMedicaRepository _repository;
        private readonly ILogger<AlertaMedicaService> _logger;

        public AlertaMedicaService(AlertaMedicaRepository repository, ILogger<AlertaMedicaService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<AlertaMedicaViewModel> GetDashboardAsync()
        {
            try
            {
                var vacunas      = await _repository.VacunasVencidasAsync();
                var tratamientos = await _repository.TratamientosPorVencerAsync();
                var recetas      = await _repository.RecetasSinRevisionAsync();
                var sinConsulta  = await _repository.SinConsultaAsync();

                return new AlertaMedicaViewModel
                {
                    VacunasVencidas       = vacunas?.ToList()      ?? new List<PR_Medico_AlertaMedica_VacunasResult>(),
                    TratamientosPorVencer = tratamientos?.ToList() ?? new List<PR_Medico_AlertaMedica_TratamientosResult>(),
                    RecetasSinRevision    = recetas?.ToList()      ?? new List<PR_Medico_AlertaMedica_RecetasResult>(),
                    SinConsulta           = sinConsulta?.ToList()  ?? new List<PR_Medico_AlertaMedica_SinConsultaResult>()
                };
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new AlertaMedicaViewModel();
            }
        }
    }
}
