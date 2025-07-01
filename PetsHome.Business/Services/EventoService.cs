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
    /// Servicio que gestiona eventos relacionados con las mascotas.
    /// </summary>
    public class EventoService
    {
        private readonly EventoRepository _eventoRepository;
        private readonly ILogger<EventoService> _logger;
        private readonly IMapper _mapper;

        public EventoService(EventoRepository eventoRepository, ILogger<EventoService> logger, IMapper mapper)
        {
            _eventoRepository = eventoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los eventos.
        /// </summary>
        /// <returns>Una lista de objetos EventoViewModel.</returns>
        public async Task<List<EventoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Eventos_ListResult> mappedResult = await _eventoRepository.ListAsync();
                return _mapper.Map<List<EventoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un evento por su identificador.
        /// </summary>
        /// <param name="id">Identificador del evento.</param>
        /// <returns>Un objeto EventoViewModel que corresponde al evento encontrado.</returns>
        public async Task<EventoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Eventos_FindResult mappedResult = await _eventoRepository.FindAsync(id);
                return _mapper.Map<EventoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un evento por su identificador.
        /// </summary>
        /// <param name="id">Identificador del evento.</param>
        /// <returns>Un objeto EventoViewModel que contiene los detalles del evento.</returns>
        public async Task<EventoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Eventos_DetailResult mappedResult = await _eventoRepository.DetailAsync(id);
                return _mapper.Map<EventoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo evento.
        /// </summary>
        /// <param name="model">Datos del evento a agregar.</param>
        /// <returns>True si el evento se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(EventoViewModel model)
        {
            try
            {
                tbEventos mappedResult = _mapper.Map<tbEventos>(model);
                return await _eventoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un evento existente.
        /// </summary>
        /// <param name="model">Datos actualizados del evento.</param>
        /// <returns>True si el evento se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(EventoViewModel model)
        {
            try
            {
                tbEventos mappedResult = _mapper.Map<tbEventos>(model);
                return await _eventoRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un evento por su identificador.
        /// </summary>
        /// <param name="id">Identificador del evento a eliminar.</param>
        /// <returns>True si el evento se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _eventoRepository.RemoveAsync(id);
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