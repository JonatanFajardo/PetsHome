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
    public class CitaMedicaService
    {
        private readonly CitaMedicaRepository _historialmedicoRepository;
        private readonly MascotaRepository _mascotaRepository;
        private readonly ILogger<CitaMedicaService> _logger;
        private readonly IMapper _mapper;

        public CitaMedicaService(CitaMedicaRepository historialmedicoRepository,
            MascotaRepository mascotaRepository,
            ILogger<CitaMedicaService> logger, IMapper mapper)
        { 
            _historialmedicoRepository = historialmedicoRepository;
            _mascotaRepository = mascotaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los registros del historial m�dico.
        /// </summary>
        /// <returns>Una lista de objetos HistorialMedicoViewModel.</returns>
        public async Task<List<CitaMedicaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_CitaMedica_ListResult> mappedResult = await _historialmedicoRepository.ListAsync();
                return _mapper.Map<List<CitaMedicaViewModel>>(mappedResult.ToList());
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
        public async Task<CitaMedicaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_CitaMedica_FindResult mappedResult = await _historialmedicoRepository.FindAsync(id);
                return _mapper.Map<CitaMedicaViewModel>(mappedResult);
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
        public async Task<CitaMedicaDetailViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_CitaMedica_DetailResult mappedResult = await _historialmedicoRepository.DetailAsync(id);
                return _mapper.Map<CitaMedicaDetailViewModel>(mappedResult);
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
        public async Task<bool> AddAsync(CitaMedicaViewModel model)
        {
            try
            {
                tbCitaMedica mappedResult = _mapper.Map<tbCitaMedica>(model);
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
        public async Task<bool> UpdateAsync(CitaMedicaViewModel model)
        {
            try
            {
                tbCitaMedica mappedResult = _mapper.Map<tbCitaMedica>(model);
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

        public List<MascotaDropdownViewModel> MascotaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Mascotas_ListResult> mappedResult = _mascotaRepository.MascotasDropdown();
                return _mapper.Map<List<MascotaDropdownViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
