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
    /// Servicio que gestiona el historial m�dico de las mascotas.
    /// </summary>
    public class HistorialMedicoService
    {
        private readonly HistorialMedicoRepository _historialmedicoRepository;
        private readonly ILogger<HistorialMedicoService> _logger;
        private readonly IMapper _mapper;

        public HistorialMedicoService(HistorialMedicoRepository historialmedicoRepository, ILogger<HistorialMedicoService> logger, IMapper mapper)
        {
            _historialmedicoRepository = historialmedicoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los registros del historial m�dico.
        /// </summary>
        /// <returns>Una lista de objetos HistorialMedicoViewModel.</returns>
        public async Task<List<HistorialMedicoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_HistorialMedico_ListResult> mappedResult = await _historialmedicoRepository.ListAsync();
                return _mapper.Map<List<HistorialMedicoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un registro del historial m�dico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del historial m�dico.</param>
        /// <returns>Un objeto HistorialMedicoViewModel que corresponde al registro encontrado.</returns>
        public async Task<HistorialMedicoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_HistorialMedico_FindResult mappedResult = await _historialmedicoRepository.FindAsync(id);
                return _mapper.Map<HistorialMedicoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un registro del historial m�dico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del historial m�dico.</param>
        /// <returns>Un objeto HistorialMedicoViewModel que contiene los detalles del registro del historial m�dico.</returns>
        public async Task<HistorialMedicoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_HistorialMedico_DetailResult mappedResult = await _historialmedicoRepository.DetailAsync(id);
                return _mapper.Map<HistorialMedicoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo registro al historial m�dico.
        /// </summary>
        /// <param name="model">Datos del registro del historial m�dico a agregar.</param>
        /// <returns>True si el registro del historial m�dico se agreg� correctamente, False si ocurri� un error.</returns>
        public async Task<bool> AddAsync(HistorialMedicoViewModel model, int userId)
        {
            try
            {
                tbHistorialMedico mappedResult = _mapper.Map<tbHistorialMedico>(model);
                mappedResult.medic_UsuarioCrea = userId;
                mappedResult.medic_FechaCrea = DateTime.Now;
                return await _historialmedicoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un registro existente del historial m�dico.
        /// </summary>
        /// <param name="model">Datos actualizados del registro del historial m�dico.</param>
        /// <returns>True si el registro del historial m�dico se actualiz� correctamente, False si ocurri� un error.</returns>
        public async Task<bool> UpdateAsync(HistorialMedicoViewModel model, int userId)
        {
            try
            {
                tbHistorialMedico mappedResult = _mapper.Map<tbHistorialMedico>(model);
                mappedResult.medic_UsuarioModifica = userId;
                mappedResult.medic_FechaModifica = DateTime.Now;
                return await _historialmedicoRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un registro del historial m�dico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del historial m�dico a eliminar.</param>
        /// <returns>True si el registro del historial m�dico se elimin� correctamente, False si ocurri� un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _historialmedicoRepository.RemoveAsync(id);
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