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
    /// Clase que representa el servicio de departamento.
    /// </summary>
    public class DepartamentoService
    {
        private readonly LocalidadRepository _departamentoRepository;
        private readonly MunicipioRepository _municipioRepository;
        private readonly ILogger<DepartamentoService> _logger;
        private readonly IMapper _mapper;

        public DepartamentoService(LocalidadRepository departamentoRepository, MunicipioRepository municipioRepository, ILogger<DepartamentoService> logger, IMapper mapper)
        {
            _departamentoRepository = departamentoRepository;
            _municipioRepository = municipioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de departamentos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de departamentos.</returns>
        public async Task<List<DepartamentoListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_General_Departamentos_ListResult> mappedResult = await _departamentoRepository.ListAsync();
                return _mapper.Map<List<DepartamentoListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un departamento por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del departamento.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el departamento encontrado.</returns>
        public async Task<DepartamentoFormViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _departamentoRepository.FindAsync(id);
                var result = _mapper.Map<DepartamentoFormViewModel>(mappedResult);
                return result;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un departamento por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del departamento.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles del departamento.</returns>
        public async Task<DepartamentoDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_General_Departamentos_DetailResult mappedResult = await _departamentoRepository.DetailAsync(id);
                var result = _mapper.Map<DepartamentoDetailsViewModel>(mappedResult);

                // Load associated municipalities
                var municipios = await _municipioRepository.ListIdAsync(id);

                // Map to MunicipioDetailsViewModel - ListResult doesn't have audit fields, so we initialize them as empty
                result.ListadoMunicipios = municipios.Select(m => new MunicipioDetailsViewModel
                {
                    mpio_Id = m.mpio_Id,
                    mpio_Codigo = m.mpio_Codigo,
                    mpio_Descripcion = m.mpio_Descripcion,
                    depto_Id = id,
                    // Audit fields will be empty since ListResult doesn't include them
                    UsuarioCreacion = "N/A",
                    mpio_FechaCrea = null,
                    UsuarioModificacion = null,
                    mpio_FechaModifica = null
                }).ToList();

                return result;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo departamento de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del departamento a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó el departamento correctamente.</returns>
        public async Task<Boolean> AddAsync(DepartamentoFormViewModel model)
        {
            try
            {
                tbDepartamentos mappedResult = _mapper.Map<tbDepartamentos>(model);
                return await _departamentoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un departamento de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo del departamento a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó el departamento correctamente.</returns>
        public async Task<Boolean> UpdateAsync(DepartamentoFormViewModel model)
        {
            try
            {
                tbDepartamentos mappedResult = _mapper.Map<tbDepartamentos>(model);
                return await _departamentoRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un departamento por su ID de forma as�ncrona.
        /// </summary>
        /// <param name="id">El ID del departamento a eliminar.</param>
        /// <returns>Una tarea que representa la operaci�n asincr�nica. El resultado indica si se elimin� el departamento correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _departamentoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Obtiene un dropdown de departamentos.
        /// </summary>
        /// <returns>Una enumeración de DepartamentoListViewModel que representa el dropdown de departamentos.</returns>
        public IEnumerable<DepartamentoListViewModel> DepartamentoDropdown()
        {
            try
            {
                IEnumerable<PR_General_Departamentos_DropdownResult> mappedResult = _departamentoRepository.DepartamentoDropdown();
                return _mapper.Map<IEnumerable<DepartamentoListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}