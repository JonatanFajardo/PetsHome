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
    /// Servicio que gestiona los municipios.
    /// </summary>
    public class MunicipioService
    {
        private readonly MunicipioRepository _municipioRepository;
        private readonly ILogger<MunicipioService> _logger;
        private readonly IMapper _mapper;

        public MunicipioService(MunicipioRepository municipioRepository, ILogger<MunicipioService> logger, IMapper mapper)
        {
            _municipioRepository = municipioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de municipios por el identificador de departamento.
        /// </summary>
        /// <param name="id">Identificador del departamento.</param>
        /// <returns>Una lista de objetos MunicipioViewModel que corresponden a los municipios encontrados.</returns>
        public async Task<List<MunicipioViewModel>> ListIdAsync(int id)
        {
            try
            {
                IEnumerable<PR_General_Municipios_ListResult> mappedResult = await _municipioRepository.ListIdAsync(id);
                return _mapper.Map<List<MunicipioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un municipio por su identificador.
        /// </summary>
        /// <param name="id">Identificador del municipio.</param>
        /// <returns>Un objeto DepartamentoViewModel que corresponde al municipio encontrado.</returns>
        public async Task<DepartamentoViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _municipioRepository.FindAsync(id);
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo municipio.
        /// </summary>
        /// <param name="model">Datos del municipio a agregar.</param>
        /// <returns>True si el municipio se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(MunicipioViewModel model)
        {
            try
            {
                tbMunicipios mappedResult = _mapper.Map<tbMunicipios>(model);
                return await _municipioRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un municipio existente.
        /// </summary>
        /// <param name="model">Datos actualizados del municipio.</param>
        /// <returns>True si el municipio se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(MunicipioViewModel model)
        {
            try
            {
                tbMunicipios mappedResult = _mapper.Map<tbMunicipios>(model);
                return await _municipioRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un municipio por su identificador.
        /// </summary>
        /// <param name="id">Identificador del municipio a eliminar.</param>
        /// <returns>True si el municipio se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _municipioRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Obtiene una lista de municipios para su uso en un dropdown.
        /// </summary>
        /// <returns>Una lista de objetos MunicipioViewModel para el dropdown.</returns>
        public IEnumerable<MunicipioViewModel> MunicipioDropdown()
        {
            try
            {
                IEnumerable<PR_General_Municipios_DropdownResult> mappedResult = _municipioRepository.MunicipioDropdown();
                return _mapper.Map<IEnumerable<MunicipioViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}