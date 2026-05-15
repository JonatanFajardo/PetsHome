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
    public class PerfilMedicoService
    {
        private readonly PerfilMedicoRepository _repository;
        private readonly ILogger<PerfilMedicoService> _logger;

        public PerfilMedicoService(PerfilMedicoRepository repository, ILogger<PerfilMedicoService> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<int> GetRandomMascIdAsync()
        {
            try   { return await _repository.RandomMascIdAsync(); }
            catch { return 1; }
        }

        public async Task<PerfilMedicoViewModel> GetDashboardAsync(int mascId)
        {
            try
            {
                var fichaMascota = await _repository.FichaMascotaAsync(mascId);
        var ultimasCitas = await _repository.UltimasCitasAsync(mascId);
        var medicamentosActivos = await _repository.MedicamentosActivosAsync(mascId);
        var todasCitas = await _repository.TodasCitasAsync(mascId);
        var tratamientos = await _repository.TratamientosAsync(mascId);
        var vacunas = await _repository.VacunasAsync(mascId);

                return new PerfilMedicoViewModel
                {
                    FichaMascota = fichaMascota?.ToList() ?? new List<PR_Medico_PerfilMedico_FichaMascotaResult>(),
            UltimasCitas = ultimasCitas?.ToList() ?? new List<PR_Medico_PerfilMedico_UltimasCitasResult>(),
            MedicamentosActivos = medicamentosActivos?.ToList() ?? new List<PR_Medico_PerfilMedico_MedicamentosActivosResult>(),
            TodasCitas = todasCitas?.ToList() ?? new List<PR_Medico_PerfilMedico_TodasCitasResult>(),
            Tratamientos = tratamientos?.ToList() ?? new List<PR_Medico_PerfilMedico_TratamientosResult>(),
            Vacunas = vacunas?.ToList() ?? new List<PR_Medico_PerfilMedico_VacunasResult>(),
                };
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new PerfilMedicoViewModel();
            }
        }
    }
}
