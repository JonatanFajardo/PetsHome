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
    /// Clase que representa el servicio de Entradas.
    /// </summary>
    public class EntradaService
    {
        private readonly EntradaRepository _entradaRepository;
        private readonly ILogger<EntradaService> _logger;
        private readonly IMapper _mapper;

        public EntradaService(EntradaRepository entradaRepository, ILogger<EntradaService> logger, IMapper mapper)
        {
            _entradaRepository = entradaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de Entradas de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de Entradas.</returns>
        public async Task<List<EntradaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Entradas_ListResult> mappedResult = await _entradaRepository.ListAsync();
                return _mapper.Map<List<EntradaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una Entrada por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la Entrada.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la Entrada encontrada.</returns>
        public async Task<EntradaViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _entradaRepository.FindAsync(id);
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una Entrada por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la Entrada.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles de la Entrada.</returns>
        public async Task<EntradaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Entradas_DetailResult mappedResult = await _entradaRepository.DetailAsync(id);
                return _mapper.Map<EntradaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva Entrada de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la Entrada a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó la Entrada correctamente.</returns>
        public async Task<Boolean> AddAsync(EntradaViewModel model)
        {
            try
            {
                tbEntradas mappedResult = _mapper.Map<tbEntradas>(model);
                return await _entradaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una Entrada existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la Entrada a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó la Entrada correctamente.</returns>
        public async Task<Boolean> UpdateAsync(EntradaViewModel model)
        {
            try
            {
                tbEntradas mappedResult = _mapper.Map<tbEntradas>(model);
                return await _entradaRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una Entrada por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la Entrada a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó la Entrada correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _entradaRepository.RemoveAsync(id);
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