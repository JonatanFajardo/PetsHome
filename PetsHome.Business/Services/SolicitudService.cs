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
    /// Servicio que gestiona las solicitudes.
    /// </summary>
    public class SolicitudService
    {
        private readonly SolicitudRepository _solicitudRepository;
        private readonly ILogger<SolicitudService> _logger;
        private readonly IMapper _mapper;

        public SolicitudService(SolicitudRepository solicitudRepository, ILogger<SolicitudService> logger, IMapper mapper)
        {
            _solicitudRepository = solicitudRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de solicitudes.
        /// </summary>
        /// <returns>Una lista de objetos SolicitudViewModel que corresponden a las solicitudes encontradas.</returns>
        public async Task<List<SolicitudViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Solicitudes_ListResult> mappedResult = await _solicitudRepository.ListAsync();
                return _mapper.Map<List<SolicitudViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una solicitud por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la solicitud.</param>
        /// <returns>Un objeto SolicitudViewModel que corresponde a la solicitud encontrada.</returns>
        public async Task<SolicitudViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_FindResult mappedResult = await _solicitudRepository.FindAsync(id);
                return _mapper.Map<SolicitudViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una solicitud por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la solicitud.</param>
        /// <returns>Un objeto SolicitudViewModel que contiene los detalles de la solicitud encontrada.</returns>
        public async Task<SolicitudViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_DetailResult mappedResult = await _solicitudRepository.DetailAsync(id);
                return _mapper.Map<SolicitudViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva solicitud.
        /// </summary>
        /// <param name="model">Datos de la solicitud a agregar.</param>
        /// <returns>True si la solicitud se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(SolicitudViewModel model)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                return await _solicitudRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una solicitud existente.
        /// </summary>
        /// <param name="model">Datos actualizados de la solicitud.</param>
        /// <returns>True si la solicitud se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(SolicitudViewModel model)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                return await _solicitudRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una solicitud por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la solicitud a eliminar.</param>
        /// <returns>True si la solicitud se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _solicitudRepository.RemoveAsync(id);
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