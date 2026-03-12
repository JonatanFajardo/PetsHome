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
    /// Servicio que gestiona los detalles de recepciones de mercancía.
    /// </summary>
    public class RecepcionDetalleService
    {
        private readonly RecepcionDetalleRepository _detalleRepository;
        private readonly ILogger<RecepcionDetalleService> _logger;
        private readonly IMapper _mapper;

        public RecepcionDetalleService(RecepcionDetalleRepository detalleRepository,
                                       ILogger<RecepcionDetalleService> logger,
                                       IMapper mapper)
        {
            _detalleRepository = detalleRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de detalles por el identificador de recepción.
        /// </summary>
        /// <param name="recepcionId">ID de la recepción.</param>
        /// <returns>Una lista de RecepcionDetalleListViewModel.</returns>
        public async Task<List<RecepcionDetalleListViewModel>> ListByRecepcionAsync(int recepcionId)
        {
            try
            {
                IEnumerable<PR_Inventario_RecepcionesDetalles_ListResult> mappedResult =
                    await _detalleRepository.ListByRecepcionAsync(recepcionId);
                return _mapper.Map<List<RecepcionDetalleListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un detalle de recepción por su ID.
        /// </summary>
        /// <param name="id">ID del detalle.</param>
        /// <returns>Una RecepcionMercanciaFormViewModel con el detalle anidado.</returns>
        public async Task<RecepcionMercanciaFormViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _detalleRepository.FindAsync(id);
                var recepcion = new RecepcionMercanciaFormViewModel();
                recepcion.Detalle = _mapper.Map<RecepcionDetalleFormViewModel>(mappedResult);
                return recepcion;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un detalle de recepción por su ID para vista de detalles (incluye descripción del item).
        /// </summary>
        /// <param name="id">ID del detalle.</param>
        /// <returns>Un RecepcionDetalleListViewModel con todos los datos incluyendo la descripción del item.</returns>
        public async Task<RecepcionDetalleListViewModel> FindForDetailAsync(int id)
        {
            try
            {
                // Primero obtenemos el detalle básico para saber a qué recepción pertenece
                var detalleBasico = await _detalleRepository.FindAsync(id);
                if (detalleBasico == null)
                    return null;

                // Luego obtenemos la lista completa de detalles de esa recepción (incluye descripción del item)
                var listaDetalles = await ListByRecepcionAsync(detalleBasico.recep_Id);

                // Filtramos el detalle específico que buscamos
                return listaDetalles?.FirstOrDefault(d => d.recdet_Id == id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo detalle de recepción.
        /// </summary>
        /// <param name="model">Modelo con los datos del detalle.</param>
        /// <returns>True si ocurrió un error, False si la operación fue exitosa.</returns>
        public async Task<bool> AddAsync(RecepcionDetalleFormViewModel model)
        {
            try
            {
                tbRecepcionesDetalles mappedResult = _mapper.Map<tbRecepcionesDetalles>(model);
                return (await _detalleRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza un detalle de recepción existente.
        /// </summary>
        /// <param name="model">Modelo con los datos actualizados del detalle.</param>
        /// <returns>True si ocurrió un error, False si la operación fue exitosa.</returns>
        public async Task<bool> UpdateAsync(RecepcionDetalleFormViewModel model)
        {
            try
            {
                tbRecepcionesDetalles mappedResult = _mapper.Map<tbRecepcionesDetalles>(model);
                return (await _detalleRepository.EditAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un detalle de recepción por su ID.
        /// </summary>
        /// <param name="id">ID del detalle a eliminar.</param>
        /// <returns>True si ocurrió un error, False si la operación fue exitosa.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                var mappedResult = await _detalleRepository.RemoveAsync(id);
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
