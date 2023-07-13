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
    /// Servicio que gestiona los refugios.
    /// </summary>
    public class RefugioService
    {
        private readonly RefugioRepository _refugioRepository;
        private readonly ILogger<RefugioService> _logger;
        private readonly IMapper _mapper;

        public RefugioService(RefugioRepository refugioRepository, ILogger<RefugioService> logger, IMapper mapper)
        {
            _refugioRepository = refugioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de refugios.
        /// </summary>
        /// <returns>Una lista de objetos RefugioViewModel que corresponden a los refugios encontrados.</returns>
        public async Task<List<RefugioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Refugios_ListResult> mappedResult = await _refugioRepository.ListAsync();
                return _mapper.Map<List<RefugioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un refugio por su identificador.
        /// </summary>
        /// <param name="id">Identificador del refugio.</param>
        /// <returns>Un objeto RefugioViewModel que corresponde al refugio encontrado.</returns>
        public async Task<RefugioViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Refugios_FindResult mappedResult = await _refugioRepository.FindAsync(id);
                return _mapper.Map<RefugioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un refugio por su identificador.
        /// </summary>
        /// <param name="id">Identificador del refugio.</param>
        /// <returns>Un objeto RefugioViewModel que contiene los detalles del refugio encontrado.</returns>
        public async Task<RefugioViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Refugios_DetailResult mappedResult = await _refugioRepository.DetailAsync(id);
                return _mapper.Map<RefugioViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo refugio.
        /// </summary>
        /// <param name="model">Datos del refugio a agregar.</param>
        /// <returns>True si el refugio se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(RefugioViewModel model)
        {
            try
            {
                tbRefugios mappedResult = _mapper.Map<tbRefugios>(model);
                return await _refugioRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un refugio existente.
        /// </summary>
        /// <param name="model">Datos actualizados del refugio.</param>
        /// <returns>True si el refugio se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(RefugioViewModel model)
        {
            try
            {
                tbRefugios mappedResult = _mapper.Map<tbRefugios>(model);
                return await _refugioRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un refugio por su identificador.
        /// </summary>
        /// <param name="id">Identificador del refugio a eliminar.</param>
        /// <returns>True si el refugio se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _refugioRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Obtiene una lista de refugios para usar en un dropdown.
        /// </summary>
        /// <returns>Una lista de objetos RefugioViewModel que corresponden a los refugios encontrados.</returns>
        public IEnumerable<RefugioViewModel> RefugioDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Refugio_DropdownResult> mappedResult = _refugioRepository.RefugioDropdown();
                return _mapper.Map<IEnumerable<RefugioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}