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

        /// <summary>
        /// Arma el certificado de vacunacion: datos de la mascota/responsable
        /// (ficha ya existente) + historial de vacunas con proximo refuerzo.
        /// </summary>
        public async Task<CertificadoVacunacionViewModel> GetCertificadoAsync(int mascId)
        {
            try
            {
                var ficha     = (await _repository.FichaMascotaAsync(mascId))?.FirstOrDefault();
                var historial = await _repository.HistorialVacunasAsync(mascId);

                var model = new CertificadoVacunacionViewModel
                {
                    FechaEmision = DateTime.Now,
                    Vacunas      = (historial ?? Enumerable.Empty<PR_Medico_Vacuna_HistorialPorMascotaResult>())
                        .Select(v => new VacunaCertificadoLinea
                        {
                            VacunaNombre    = v.VacunaNombre,
                            FechaAplicada   = v.FechaAplicada,
                            ProximoRefuerzo = v.ProximoRefuerzo,
                            Estado          = v.Estado
                        })
                        .ToList()
                };

                if (ficha != null)
                {
                    model.MascotaNombre = ficha.masc_Nombre;
                    model.Especie       = ficha.Especie;
                    model.Raza          = ficha.Raza;
                    model.Sexo          = FormatSexo(ficha.Sexo);
                    model.DuenoNombre   = !string.IsNullOrWhiteSpace(ficha.ResponsableNombre) ? ficha.ResponsableNombre
                                        : !string.IsNullOrWhiteSpace(ficha.Adoptante)         ? ficha.Adoptante
                                        : ficha.Refugio;
                }

                return model;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new CertificadoVacunacionViewModel();
            }
        }

        /// <summary>
        /// Vacunas con refuerzo proximo (<=30 dias) o vencidas, para
        /// recordatorios in-app. Si mascId es null, consulta todas.
        /// </summary>
        public async Task<List<PR_Medico_AlertaMedica_VacunasProximasResult>> GetProximosRefuerzosAsync(int? mascId)
        {
            try
            {
                var result = await _repository.VacunasProximasAsync(mascId);
                return result?.ToList() ?? new List<PR_Medico_AlertaMedica_VacunasProximasResult>();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<PR_Medico_AlertaMedica_VacunasProximasResult>();
            }
        }

        private static string FormatSexo(string sexo)
        {
            if (string.IsNullOrWhiteSpace(sexo)) return "—";
            switch (sexo.Trim().ToUpperInvariant())
            {
                case "H": return "Hembra";
                case "M": return "Macho";
                default:  return sexo;
            }
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

        // ── Pestanas nuevas vs OpenVPM (script 88) ────────────────
        var facturas = await _repository.FacturasPorMascotaAsync(mascId);
        var vitales = await _repository.VitalesPorMascotaAsync(mascId);
        var historialPeso = await _repository.PesoPorMascotaAsync(mascId);
        var laboratorios = await _repository.LaboratorioPorMascotaAsync(mascId);

                return new PerfilMedicoViewModel
                {
                    FichaMascota = fichaMascota?.ToList() ?? new List<PR_Medico_PerfilMedico_FichaMascotaResult>(),
            UltimasCitas = ultimasCitas?.ToList() ?? new List<PR_Medico_PerfilMedico_UltimasCitasResult>(),
            MedicamentosActivos = medicamentosActivos?.ToList() ?? new List<PR_Medico_PerfilMedico_MedicamentosActivosResult>(),
            TodasCitas = todasCitas?.ToList() ?? new List<PR_Medico_PerfilMedico_TodasCitasResult>(),
            Tratamientos = tratamientos?.ToList() ?? new List<PR_Medico_PerfilMedico_TratamientosResult>(),
            Vacunas = vacunas?.ToList() ?? new List<PR_Medico_PerfilMedico_VacunasResult>(),

            Facturas = (facturas ?? Enumerable.Empty<PR_Veterinaria_Facturas_PorMascotaResult>())
                .Select(f => new FichaFacturaVM
                {
                    Numero = f.Numero,
                    Fecha  = f.Fecha,
                    Total  = f.Total,
                    Saldo  = f.Saldo,
                    Estado = f.Estado
                }).ToList(),

            SignosVitales = (vitales ?? Enumerable.Empty<PR_Medico_CitaMedica_VitalesPorMascotaResult>())
                .Select(v => new FichaVitalVM
                {
                    Fecha                  = v.Fecha,
                    Peso                   = v.Peso,
                    Temperatura            = v.Temperatura,
                    FrecuenciaCardiaca     = v.FrecuenciaCardiaca,
                    FrecuenciaRespiratoria = v.FrecuenciaRespiratoria,
                    Mucosas                = v.Mucosas,
                    LlenadoCapilar         = v.LlenadoCapilar,
                    EscalaDolor            = v.EscalaDolor,
                    CondicionCorporal      = v.CondicionCorporal
                }).ToList(),

            HistorialPeso = (historialPeso ?? Enumerable.Empty<PR_Medico_CitaMedica_PesoPorMascotaResult>())
                .Select(h => new FichaPesoVM
                {
                    Fecha = h.Fecha,
                    Peso  = h.Peso
                }).ToList(),

            Laboratorios = (laboratorios ?? Enumerable.Empty<PR_Medico_Laboratorio_PorMascotaResult>())
                .Select(l => new FichaLabVM
                {
                    Examen         = l.Examen,
                    Tipo           = l.Tipo,
                    Estado         = l.Estado,
                    FechaSolicitud = l.FechaSolicitud,
                    FechaResultado = l.FechaResultado,
                    Observaciones  = l.Observaciones
                }).ToList(),
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
