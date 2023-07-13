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
    /// Servicio que gestiona las razas.
    /// </summary>
    public class RazaService
    {
        private readonly ILogger<RazaService> _logger;
        private readonly IMapper _mapper;
        private readonly RazaRepository _razaRepository;

        public RazaService(RazaRepository razaRepository, ILogger<RazaService> logger, IMapper mapper)
        {
            _razaRepository = razaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de razas.
        /// </summary>
        /// <returns>Una lista de objetos RazaViewModel que corresponden a las razas encontradas.</returns>
        public async Task<List<RazaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Razas_ListResult> mappedResult = await _razaRepository.ListAsync();
                return _mapper.Map<List<RazaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una raza por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la raza.</param>
        /// <returns>Un objeto RazaViewModel que corresponde a la raza encontrada.</returns>
        public async Task<RazaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Razas_FindResult mappedResult = await _razaRepository.FindAsync(id);
                return _mapper.Map<RazaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una raza por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la raza.</param>
        /// <returns>Un objeto RazaViewModel que contiene los detalles de la raza encontrada.</returns>
        public async Task<RazaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Razas_DetailResult mappedResult = await _razaRepository.DetailAsync(id);
                return _mapper.Map<RazaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva raza.
        /// </summary>
        /// <param name="model">Datos de la raza a agregar.</param>
        /// <returns>True si la raza se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(RazaViewModel model)
        {
            try
            {
                tbRazas mappedResult = _mapper.Map<tbRazas>(model);
                return await _razaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una raza existente.
        /// </summary>
        /// <param name="model">Datos actualizados de la raza.</param>
        /// <returns>True si la raza se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(RazaViewModel model)
        {
            try
            {
                tbRazas mappedResult = _mapper.Map<tbRazas>(model);
                return await _razaRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una raza por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la raza a eliminar.</param>
        /// <returns>True si la raza se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return await _razaRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
    }
}