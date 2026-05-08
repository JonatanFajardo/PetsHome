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
    public class DashboardVeterinarioService
    {
        private readonly DashboardVeterinarioRepository _repository;
        private readonly ILogger<DashboardVeterinarioService> _logger;

        public DashboardVeterinarioService(DashboardVeterinarioRepository repository, ILogger<DashboardVeterinarioService> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<DashboardVeterinarioViewModel> GetDashboardAsync()
        {
            try
            {
                var agendaHoy = await _repository.AgendaHoyAsync();
        var tratamientosActivos = await _repository.TratamientosActivosAsync();
        var alertasVeterinario = await _repository.AlertasVeterinarioAsync();
        var resumenMes = await _repository.ResumenMesAsync();

                return new DashboardVeterinarioViewModel
                {
                    AgendaHoy = agendaHoy?.ToList() ?? new List<PR_Medico_DashboardVeterinario_AgendaHoyResult>(),
            TratamientosActivos = tratamientosActivos?.ToList() ?? new List<PR_Medico_DashboardVeterinario_TratamientosActivosResult>(),
            AlertasVeterinario = alertasVeterinario?.ToList() ?? new List<PR_Medico_DashboardVeterinario_AlertasVeterinarioResult>(),
            ResumenMes = resumenMes?.ToList() ?? new List<PR_Medico_DashboardVeterinario_ResumenMesResult>(),
                };
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new DashboardVeterinarioViewModel();
            }
        }
    }
}
