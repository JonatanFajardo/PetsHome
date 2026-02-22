using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;

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
        public async Task<List<RefugioListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Refugios_ListResult> mappedResult = await _refugioRepository.ListAsync();
                return _mapper.Map<List<RefugioListViewModel>>(mappedResult.ToList());
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
        public async Task<RefugioFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Refugios_FindResult mappedResult = await _refugioRepository.FindAsync(id);
                return _mapper.Map<RefugioFormViewModel>(mappedResult);
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
        public async Task<RefugioDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Refugios_DetailResult mappedResult = await _refugioRepository.DetailAsync(id);
                return _mapper.Map<RefugioDetailsViewModel>(mappedResult);
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
        public async Task<bool> AddAsync(RefugioFormViewModel model, int userId)
        {
            try
            {
                tbRefugios mappedResult = _mapper.Map<tbRefugios>(model);
                mappedResult.refg_UsuarioCrea = userId;
                mappedResult.refg_FechaCrea = DateTime.Now;
                return await _refugioRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza un refugio existente.
        /// </summary>
        public async Task<bool> UpdateAsync(RefugioFormViewModel model, int userId)
        {
            try
            {
                tbRefugios mappedResult = _mapper.Map<tbRefugios>(model);
                mappedResult.refg_UsuarioModifica = userId;
                mappedResult.refg_FechaModifica = DateTime.Now;
                return await _refugioRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un refugio por su identificador.
        /// </summary>
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
                return false;
            }
        }

        /// <summary>
        /// Obtiene una lista de refugios para dropdowns.
        /// </summary>
        public IEnumerable<RefugioDropdownViewModel> RefugioDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Refugio_DropdownResult> mappedResult = _refugioRepository.RefugioDropdown();
                return _mapper.Map<IEnumerable<RefugioDropdownViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
