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
    /// Clase que representa el servicio de categor�a.
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
        /// Obtiene una lista de categor�as de forma as�ncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado contiene la lista de categor�as.</returns>
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
        /// Busca una categor�a por su ID de forma as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la categor�a.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado contiene la categor�a encontrada.</returns>
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
        /// Obtiene los detalles de una categor�a por su ID de forma as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la categor�a.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado contiene los detalles de la categor�a.</returns>
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
        /// Agrega una nueva categor�a de forma as�ncrona.
        /// </summary>
        /// <param name="model">El modelo de la categor�a a agregar.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado indica si se agreg� la categor�a correctamente.</returns>
        public async Task<Boolean> AddAsync(CategoriaViewModel model, int userId)
        {
            try
            {
                tbCategorias mappedResult = _mapper.Map<tbCategorias>(model);
                mappedResult.cat_UsuarioCrea = userId;
                mappedResult.cat_FechaCrea = DateTime.Now;
                return (await _categoriaRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza una categor�a de forma as�ncrona.
        /// </summary>
        /// <param name="model">El modelo de la categor�a a actualizar.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado indica si se actualiz� la categor�a correctamente.</returns>
        public async Task<Boolean> UpdateAsync(CategoriaViewModel model, int userId)
        {
            try
            {
                tbCategorias mappedResult = _mapper.Map<tbCategorias>(model);
                mappedResult.cat_UsuarioModifica = userId;
                mappedResult.cat_FechaModifica = DateTime.Now;
                return (await _categoriaRepository.EditAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina una categor�a por su ID de forma as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la categor�a a eliminar.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado indica si se elimin� la categor�a correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                var mappedResult = await _categoriaRepository.RemoveAsync(id);
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