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
    /// <summary>
    /// Servicio que gestiona las procedencias.
    /// </summary>
    public class ProcedenciaService
    {
        private readonly ProcedenciaRepository _procedenciaRepository;
        private readonly ILogger<ProcedenciaService> _logger;
        private readonly IMapper _mapper;

        public ProcedenciaService(ProcedenciaRepository procedenciaRepository, ILogger<ProcedenciaService> logger, IMapper mapper)
        {
            _procedenciaRepository = procedenciaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de procedencias.
        /// </summary>
        /// <returns>Una lista de objetos ProcedenciaViewModel que corresponden a las procedencias encontradas.</returns>
        public async Task<List<ProcedenciaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Procedencias_ListResult> mappedResult = await _procedenciaRepository.ListAsync();
                return _mapper.Map<List<ProcedenciaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una procedencia por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la procedencia.</param>
        /// <returns>Un objeto ProcedenciaViewModel que corresponde a la procedencia encontrada.</returns>
        public async Task<ProcedenciaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Procedencias_FindResult mappedResult = await _procedenciaRepository.FindAsync(id);
                return _mapper.Map<ProcedenciaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una procedencia por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la procedencia.</param>
        /// <returns>Un objeto ProcedenciaViewModel que contiene los detalles de la procedencia encontrada.</returns>
        public async Task<ProcedenciaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Procedencias_DetailResult mappedResult = await _procedenciaRepository.DetailAsync(id);
                return _mapper.Map<ProcedenciaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva procedencia.
        /// </summary>
        /// <param name="model">Datos de la procedencia a agregar.</param>
        /// <returns>True si la procedencia se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(ProcedenciaViewModel model)
        {
            try
            {
                tbProcedencias mappedResult = _mapper.Map<tbProcedencias>(model);
                return await _procedenciaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una procedencia existente.
        /// </summary>
        /// <param name="model">Datos actualizados de la procedencia.</param>
        /// <returns>True si la procedencia se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(ProcedenciaViewModel model)
        {
            try
            {
                tbProcedencias mappedResult = _mapper.Map<tbProcedencias>(model);
                return await _procedenciaRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una procedencia por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la procedencia a eliminar.</param>
        /// <returns>True si la procedencia se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _procedenciaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
    }
}