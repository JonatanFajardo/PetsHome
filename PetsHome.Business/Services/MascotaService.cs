using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Helpers;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio que gestiona las mascotas en un refugio.
    /// </summary>
    public class MascotaService
    {
        private readonly MascotaRepository _mascotaRepository;
        private readonly ILogger<MascotaService> _logger;
        private readonly IMapper _mapper;

        public MascotaService(MascotaRepository mascotaRepository, RefugioRepository refugioRepository, ILogger<MascotaService> logger, IMapper mapper)
        {
            _mascotaRepository = mascotaRepository;
            _ = refugioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todas las mascotas en el refugio.
        /// </summary>
        /// <returns>Una lista de RefugioMascotasListDto.</returns>
        public async Task<List<Contracts.DTOs.RefugioMascotasListDto>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Mascotas_ListResult> result = await _mascotaRepository.ListAsync();
                return _mapper.Map<List<Contracts.DTOs.RefugioMascotasListDto>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca una mascota por su identificador para su edición.
        /// </summary>
        /// <param name="id">Identificador de la mascota.</param>
        /// <returns>Un objeto RefugioMascotasFindDto que corresponde a la mascota encontrada.</returns>
        public async Task<Contracts.DTOs.RefugioMascotasFindDto> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_FindResult result = await _mascotaRepository.FindAsync(id);
                return _mapper.Map<Contracts.DTOs.RefugioMascotasFindDto>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una mascota por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la mascota.</param>
        /// <returns>Un objeto RefugioMascotasDetailDto que contiene los detalles de la mascota.</returns>
        public async Task<Contracts.DTOs.RefugioMascotasDetailDto> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_DetailResult result = await _mascotaRepository.DetailAsync(id);
                return _mapper.Map<Contracts.DTOs.RefugioMascotasDetailDto>(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva mascota al refugio.
        /// </summary>
        /// <param name="dto">Datos de la mascota a agregar.</param>
        /// <param name="userId">ID del usuario que crea el registro.</param>
        /// <returns>True si la mascota se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(Contracts.DTOs.MascotasDto dto, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbMascotas>(dto);
                entity.masc_UsuarioCrea = userId;
                entity.masc_FechaCrea = DateTime.Now;
                return await _mascotaRepository.AddAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza una mascota existente en el refugio.
        /// </summary>
        /// <param name="dto">Datos actualizados de la mascota.</param>
        /// <param name="userId">ID del usuario que modifica el registro.</param>
        /// <returns>True si la mascota se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(Contracts.DTOs.MascotasDto dto, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbMascotas>(dto);
                entity.masc_UsuarioModifica = userId;
                entity.masc_FechaModifica = DateTime.Now;
                return await _mascotaRepository.EditAsync(entity);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina una mascota del refugio por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la mascota a eliminar.</param>
        /// <returns>True si la mascota se eliminó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _mascotaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        #region Dropdown

        /// <summary>
        /// Obtiene una lista de razas para su uso en un dropdown.
        /// </summary>
        /// <returns>Una lista de RefugioRazaDropdownDto.</returns>
        public IEnumerable<Contracts.DTOs.RefugioRazaDropdownDto> RazaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Raza_DropdownResult> result = _mascotaRepository.RazaDropdown();
                return _mapper.Map<List<Contracts.DTOs.RefugioRazaDropdownDto>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene una lista de procedencias para su uso en un dropdown.
        /// </summary>
        /// <returns>Una lista de RefugioProcedenciaDropdownDto.</returns>
        public IEnumerable<Contracts.DTOs.RefugioProcedenciaDropdownDto> ProcedenciaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Procedencia_DropdownResult> result = _mascotaRepository.ProcedenciaDropdown();
                return _mapper.Map<List<Contracts.DTOs.RefugioProcedenciaDropdownDto>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        #endregion Dropdown
    }
}
