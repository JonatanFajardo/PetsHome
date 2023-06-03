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
    /// Clase que representa el servicio de categoría.
    /// </summary>
    public class CategoriaService
    {
        private readonly CategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriaService> _logger;
        private readonly IMapper _mapper;

        public CategoriaService(CategoriaRepository categoriaRepository, ILogger<CategoriaService> logger, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de categorías de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de categorías.</returns>
        public async Task<List<CategoriaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Inventario_Categorias_ListResult> mappedResult = await _categoriaRepository.ListAsync();
                return _mapper.Map<List<CategoriaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una categoría por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la categoría.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la categoría encontrada.</returns>
        public async Task<CategoriaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Inventario_Categorias_FindResult mappedResult = await _categoriaRepository.FindAsync(id);
                return _mapper.Map<CategoriaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una categoría por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la categoría.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles de la categoría.</returns>
        public async Task<CategoriaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Inventario_Categorias_DetailResult mappedResult = await _categoriaRepository.DetailAsync(id);
                return _mapper.Map<CategoriaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva categoría de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la categoría a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó la categoría correctamente.</returns>
        public async Task<Boolean> AddAsync(CategoriaViewModel model)
        {
            try
            {
                tbCategorias mappedResult = _mapper.Map<tbCategorias>(model);
                return await _categoriaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una categoría de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la categoría a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó la categoría correctamente.</returns>
        public async Task<Boolean> UpdateAsync(CategoriaViewModel model)
        {
            try
            {
                tbCategorias mappedResult = _mapper.Map<tbCategorias>(model);
                return await _categoriaRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una categoría por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la categoría a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó la categoría correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _categoriaRepository.RemoveAsync(id);
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