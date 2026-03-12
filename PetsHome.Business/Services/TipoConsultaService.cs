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
    /// Servicio que gestiona los tipos de consulta médica.
    /// </summary>
    public class TipoConsultaService
    {
        private readonly TipoConsultaRepository _tipoConsultaRepository;
        private readonly ILogger<TipoConsultaService> _logger;
        private readonly IMapper _mapper;

        public TipoConsultaService(TipoConsultaRepository tipoConsultaRepository, ILogger<TipoConsultaService> logger, IMapper mapper)
        {
            _tipoConsultaRepository = tipoConsultaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de tipos de consulta.
        /// </summary>
        public async Task<List<TipoConsultaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_TiposConsulta_ListResult> mappedResult = await _tipoConsultaRepository.ListAsync();
                return _mapper.Map<List<TipoConsultaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un tipo de consulta por su identificador.
        /// </summary>
        public async Task<TipoConsultaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_TiposConsulta_FindResult mappedResult = await _tipoConsultaRepository.FindAsync(id);
                return _mapper.Map<TipoConsultaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un tipo de consulta por su identificador.
        /// </summary>
        public async Task<TipoConsultaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_TiposConsulta_DetailResult mappedResult = await _tipoConsultaRepository.DetailAsync(id);
                return _mapper.Map<TipoConsultaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo tipo de consulta.
        /// </summary>
        public async Task<bool> AddAsync(TipoConsultaViewModel model)
        {
            try
            {
                tbTiposConsulta mappedResult = _mapper.Map<tbTiposConsulta>(model);
                return (await _tipoConsultaRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza un tipo de consulta existente.
        /// </summary>
        public async Task<bool> UpdateAsync(TipoConsultaViewModel model)
        {
            try
            {
                tbTiposConsulta mappedResult = _mapper.Map<tbTiposConsulta>(model);
                return (await _tipoConsultaRepository.EditAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un tipo de consulta por su identificador.
        /// </summary>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                var mappedResult = await _tipoConsultaRepository.RemoveAsync(id);
                return mappedResult.Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}
