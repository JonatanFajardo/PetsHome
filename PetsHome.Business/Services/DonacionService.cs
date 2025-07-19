using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    /// Clase que representa el servicio de donaciones.
    /// </summary>
    public class DonacionService
    {
        private readonly DonacionRepository _donacionRepository;
        private readonly RefugioService _refugioService;
        private readonly ILogger<DonacionService> _logger;
        private readonly IMapper _mapper;

        public DonacionService(DonacionRepository donacionRepository, RefugioService refugioService, ILogger<DonacionService> logger, IMapper mapper)
        {
            _donacionRepository = donacionRepository;
            _refugioService = refugioService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de donaciones de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de donaciones.</returns>
        public async Task<List<DonacionViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Donaciones_ListResult> mappedResult = await _donacionRepository.ListAsync();
                return _mapper.Map<List<DonacionViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una donación por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la donación.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la donación encontrada.</returns>
        public async Task<DonacionViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Donaciones_FindResult mappedResult = await _donacionRepository.FindAsync(id);
                return _mapper.Map<DonacionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una donación por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la donación.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene los detalles de la donación.</returns>
        public async Task<DonacionViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Donaciones_DetailResult mappedResult = await _donacionRepository.DetailAsync(id);
                return _mapper.Map<DonacionViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva donación de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la donación a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se agregó la donación correctamente.</returns>
        public async Task<Boolean> AddAsync(DonacionViewModel model)
        {
            try
            {
                tbDonaciones mappedResult = _mapper.Map<tbDonaciones>(model);
                return await _donacionRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una donación de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de la donación a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se actualizó la donación correctamente.</returns>
        public async Task<Boolean> UpdateAsync(DonacionViewModel model)
        {
            try
            {
                tbDonaciones mappedResult = _mapper.Map<tbDonaciones>(model);
                return await _donacionRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una donación por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la donación a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó la donación correctamente.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _donacionRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Obtiene la lista de refugios para dropdown.
        /// </summary>
        /// <returns>Lista de refugios para dropdown</returns>
        public List<SelectListItem> RefugioDropdown()
        {
            var refugios = _refugioService.RefugioDropdown();
            return refugios?.Select(r => new SelectListItem
            {
                Value = r.refg_Id.ToString(),
                Text = r.refg_Nombre
            }).ToList() ?? new List<SelectListItem>();
        }

        /// <summary>
        /// Obtiene la lista de tipos de donación para dropdown.
        /// </summary>
        /// <returns>Lista de tipos de donación para dropdown</returns>
        public List<SelectListItem> TiposDonacionDropdown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Monetaria", Text = "Monetaria" },
                new SelectListItem { Value = "Artículos", Text = "Artículos" },
                new SelectListItem { Value = "Mixta", Text = "Mixta" },
                new SelectListItem { Value = "Servicios", Text = "Servicios" }
            };
        }

        /// <summary>
        /// Obtiene la lista de estados para dropdown.
        /// </summary>
        /// <returns>Lista de estados para dropdown</returns>
        public List<SelectListItem> EstadosDropdown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Recibida", Text = "Recibida" },
                new SelectListItem { Value = "En Proceso", Text = "En Proceso" },
                new SelectListItem { Value = "Procesada", Text = "Procesada" },
                new SelectListItem { Value = "Rechazada", Text = "Rechazada" }
            };
        }
    }
}