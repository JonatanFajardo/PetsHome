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
        /// <returns>Una lista de objetos solicitud que corresponden a las solicitudes encontradas.</returns>
        public async Task<List<SolicitudListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Solicitudes_ListResult> mappedResult = await _solicitudRepository.ListAsync();
                return _mapper.Map<List<SolicitudListViewModel>>(mappedResult.ToList());
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
        /// <returns>Un objeto solicitud que corresponde a la solicitud encontrada.</returns>
        public async Task<SolicitudFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_FindResult mappedResult = await _solicitudRepository.FindAsync(id);
                return _mapper.Map<SolicitudFormViewModel>(mappedResult);
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
        /// <returns>Un objeto solicitud que contiene los detalles de la solicitud encontrada.</returns>
        public async Task<SolicitudDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_DetailResult mappedResult = await _solicitudRepository.DetailAsync(id);
                return _mapper.Map<SolicitudDetailsViewModel>(mappedResult);
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
        /// <returns>True si la solicitud se agreg� correctamente, False si ocurri� un error.</returns>
        public async Task<bool> AddAsync(SolicitudFormViewModel model, int userId)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                mappedResult.sol_UsuarioCrea = userId;
                mappedResult.sol_FechaCrea = DateTime.Now;
                return (await _solicitudRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza una solicitud existente.
        /// </summary>
        /// <param name="model">Datos actualizados de la solicitud.</param>
        /// <returns>True si la solicitud se actualiz� correctamente, False si ocurri� un error.</returns>
        public async Task<bool> UpdateAsync(SolicitudFormViewModel model, int userId)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                mappedResult.sol_UsuarioModifica = userId;
                mappedResult.sol_FechaModifica = DateTime.Now;
                return (await _solicitudRepository.EditAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina una solicitud por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la solicitud a eliminar.</param>
        /// <returns>True si la solicitud se elimin� correctamente, False si ocurri� un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                var mappedResult = await _solicitudRepository.RemoveAsync(id);
                return mappedResult.Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> CambiarEstadoAsync(int id, string estado, int userId)
        {
            try
            {
                var result = await _solicitudRepository.CambiarEstadoAsync(id, estado, userId);
                return result.Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}
