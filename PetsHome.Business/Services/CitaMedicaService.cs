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
    /// Servicio que gestiona el historial médico de las mascotas.
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
        /// Obtiene una lista de todos los registros del historial médico.
        /// </summary>
        /// <returns>Una lista de objetos HistorialMedicoViewModel.</returns>
        public async Task<List<CitaMedicaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_CitaMedica_ListResult> mappedResult = await _historialmedicoRepository.ListAsyncs();
                return _mapper.Map<List<CitaMedicaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un registro del historial médico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del historial médico.</param>
        /// <returns>Un objeto HistorialMedicoViewModel que corresponde al registro encontrado.</returns>
        public async Task<CitaMedicaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_CitaMedica_FindResult mappedResult = await _historialmedicoRepository.FindAsync(id);
                return _mapper.Map<CitaMedicaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un registro del historial médico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del historial médico.</param>
        /// <returns>Un objeto HistorialMedicoViewModel que contiene los detalles del registro del historial médico.</returns>
        public async Task<CitaMedicaDetailViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_CitaMedica_DetailResult mappedResult = await _historialmedicoRepository.DetailAsync(id);
                return _mapper.Map<CitaMedicaDetailViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo registro al historial médico.
        /// </summary>
        /// <param name="model">Datos del registro del historial médico a agregar.</param>
        /// <returns>True si el registro del historial médico se agregó correctamente, False si ocurrió un error.</returns>
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
        /// Actualiza un registro existente del historial médico.
        /// </summary>
        /// <param name="model">Datos actualizados del registro del historial médico.</param>
        /// <returns>True si el registro del historial médico se actualizó correctamente, False si ocurrió un error.</returns>
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
        /// Elimina un registro del historial médico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro del historial médico a eliminar.</param>
        /// <returns>True si el registro del historial médico se eliminó correctamente, False si ocurrió un error.</returns>
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

        public List<PR_Refugio_Mascotas_DropdownResult> MascotaDropdown()
        {
            try
            {
                List<PR_Refugio_Mascotas_DropdownResult> mappedResult = _mascotaRepository.MascotasDropdown();
                return mappedResult.ToList();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}