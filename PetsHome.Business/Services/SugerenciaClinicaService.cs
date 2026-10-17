using Microsoft.Extensions.Logging;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Motor de sugerencias clinicas: dado el texto de un diagnostico,
    /// devuelve las reglas (medicamento/vacuna) cuya palabra clave aparece.
    /// </summary>
    public class SugerenciaClinicaService
    {
        private readonly SugerenciaClinicaRepository _repository;
        private readonly ILogger<SugerenciaClinicaService> _logger;

        public SugerenciaClinicaService(SugerenciaClinicaRepository repository, ILogger<SugerenciaClinicaService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Reglas que coinciden con el texto. Devuelve lista vacia ante error o texto vacio.
        /// </summary>
        public async Task<List<PR_Medico_Sugerencia_PorTextoResult>> PorTextoAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new List<PR_Medico_Sugerencia_PorTextoResult>();

            try
            {
                var result = await _repository.PorTextoAsync(texto);
                return result == null
                    ? new List<PR_Medico_Sugerencia_PorTextoResult>()
                    : new List<PR_Medico_Sugerencia_PorTextoResult>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<PR_Medico_Sugerencia_PorTextoResult>();
            }
        }
    }
}
