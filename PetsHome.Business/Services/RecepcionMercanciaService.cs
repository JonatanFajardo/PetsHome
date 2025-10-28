using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Extensions;
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
    /// Servicio que gestiona las recepciones de mercancía.
    /// </summary>
    public class RecepcionMercanciaService
    {
        private readonly RecepcionMercanciaRepository _recepcionRepository;
        private readonly ILogger<RecepcionMercanciaService> _logger;
        private readonly IMapper _mapper;

        public RecepcionMercanciaService(RecepcionMercanciaRepository recepcionRepository,
                                         ILogger<RecepcionMercanciaService> logger,
                                         IMapper mapper)
        {
            _recepcionRepository = recepcionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todas las recepciones de mercancía de forma asíncrona.
        /// </summary>
        /// <returns>Una lista de RecepcionMercanciaListViewModel.</returns>
        public async Task<List<RecepcionMercanciaListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_RecepcionesMercancia_ListResult> mappedResult = await _recepcionRepository.ListAsync();
                return _mapper.Map<List<RecepcionMercanciaListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una recepción de mercancía por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>Una RecepcionMercanciaFormViewModel con los datos de la recepción.</returns>
        public async Task<RecepcionMercanciaFormViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _recepcionRepository.FindAsync(id);
                var result = _mapper.Map<RecepcionMercanciaFormViewModel>(mappedResult);
                return result;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una recepción de mercancía por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>Una RecepcionMercanciaDetailsViewModel con el detalle completo.</returns>
        public async Task<RecepcionMercanciaDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_RecepcionesMercancia_DetailResult mappedResult = await _recepcionRepository.DetailAsync(id);
                return _mapper.Map<RecepcionMercanciaDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva recepción de mercancía de forma asíncrona.
        /// </summary>
        /// <param name="model">Modelo con los datos de la recepción.</param>
        /// <returns>True si ocurrió un error, False si la operación fue exitosa.</returns>
        public async Task<Boolean> AddAsync(RecepcionMercanciaFormViewModel model)
        {
            try
            {
                tbRecepcionesMercancia mappedResult = _mapper.Map<tbRecepcionesMercancia>(model);
                return await _recepcionRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una recepción de mercancía de forma asíncrona.
        /// </summary>
        /// <param name="model">Modelo con los datos actualizados de la recepción.</param>
        /// <returns>True si ocurrió un error, False si la operación fue exitosa.</returns>
        public async Task<Boolean> UpdateAsync(RecepcionMercanciaFormViewModel model)
        {
            try
            {
                tbRecepcionesMercancia mappedResult = _mapper.Map<tbRecepcionesMercancia>(model);
                return await _recepcionRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una recepción de mercancía por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la recepción a eliminar.</param>
        /// <returns>True si ocurrió un error, False si la operación fue exitosa.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _recepcionRepository.RemoveAsync(id);
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
