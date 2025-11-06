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
    public class TratamientoService
    {
        private readonly TratamientoRepository _tratamientoRepository;
        private readonly ILogger<TratamientoService> _logger;
        private readonly IMapper _mapper;

        public TratamientoService(TratamientoRepository tratamientoRepository, ILogger<TratamientoService> logger, IMapper mapper)
        {
            _tratamientoRepository = tratamientoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los tratamientos.
        /// </summary>
        /// <returns>Una lista de objetos TratamientoListViewModel.</returns>
        public async Task<List<TratamientoListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_Tratamientos_ListResult> mappedResult = await _tratamientoRepository.ListAsync();
                return _mapper.Map<List<TratamientoListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un tratamiento por su identificador para su edición.
        /// </summary>
        /// <param name="id">Identificador del tratamiento.</param>
        /// <returns>Un objeto TratamientoFormViewModel que corresponde al tratamiento encontrado.</returns>
        public async Task<TratamientoFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_Tratamientos_FindResult mappedResult = await _tratamientoRepository.FindAsync(id);
                return _mapper.Map<TratamientoFormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un tratamiento por su identificador.
        /// </summary>
        /// <param name="id">Identificador del tratamiento.</param>
        /// <returns>Un objeto TratamientoDetailsViewModel que contiene los detalles del tratamiento.</returns>
        public async Task<TratamientoDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_Tratamientos_DetailResult mappedResult = await _tratamientoRepository.DetailAsync(id);
                return _mapper.Map<TratamientoDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo tratamiento.
        /// </summary>
        /// <param name="model">Datos del tratamiento a agregar.</param>
        /// <returns>True si el tratamiento se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(TratamientoFormViewModel model)
        {
            try
            {
                tbTratamientos mappedResult = _mapper.Map<tbTratamientos>(model);
                return await _tratamientoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un tratamiento existente.
        /// </summary>
        /// <param name="model">Datos actualizados del tratamiento.</param>
        /// <returns>True si el tratamiento se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(TratamientoFormViewModel model)
        {
            try
            {
                tbTratamientos mappedResult = _mapper.Map<tbTratamientos>(model);
                return await _tratamientoRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un tratamiento por su identificador.
        /// </summary>
        /// <param name="id">Identificador del tratamiento a eliminar.</param>
        /// <returns>True si el tratamiento se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _tratamientoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public IEnumerable<object> TratamientoDropdown(int? masc_Id = null)
        {
            try
            {
                IEnumerable<PR_Medico_Tratamientos_DropdownResult> mappedResult = _tratamientoRepository.Dropdown(masc_Id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
