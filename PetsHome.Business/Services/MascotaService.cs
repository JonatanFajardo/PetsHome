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
        /// <returns>Una lista de objetos MascotaListViewModel.</returns>
        public async Task<List<MascotaListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Mascotas_ListResult> mappedResult = await _mascotaRepository.ListAsync();
                return _mapper.Map<List<MascotaListViewModel>>(mappedResult.ToList());
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
        /// <returns>Un objeto MascotaFormViewModel que corresponde a la mascota encontrada.</returns>
        public async Task<MascotaFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_FindResult mappedResult = await _mascotaRepository.FindAsync(id);
                return _mapper.Map<MascotaFormViewModel>(mappedResult);
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
        /// <returns>Un objeto MascotaDetailsViewModel que contiene los detalles de la mascota.</returns>
        public async Task<MascotaDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_DetailResult mappedResult = await _mascotaRepository.DetailAsync(id);
                return _mapper.Map<MascotaDetailsViewModel>(mappedResult);
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
        /// <param name="model">Datos de la mascota a agregar.</param>
        /// <returns>True si la mascota se agregó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> AddAsync(MascotaFormViewModel model, int userId)
        {
            try
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    model.masc_Imagen = await model.ImageFile.GetBytesAsync();
                }
                else
                {
                    model.masc_Imagen = null;
                }

                tbMascotas mappedResult = _mapper.Map<tbMascotas>(model);
                mappedResult.masc_UsuarioCrea = userId;
                mappedResult.masc_FechaCrea = DateTime.Now;
                return await _mascotaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Actualiza una mascota existente en el refugio.
        /// </summary>
        /// <param name="model">Datos actualizados de la mascota.</param>
        /// <returns>True si la mascota se actualizó correctamente, False si ocurrió un error.</returns>
        public async Task<bool> UpdateAsync(MascotaFormViewModel model, int userId)
        {
            try
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    model.masc_Imagen = await model.ImageFile.GetBytesAsync();
                }

                tbMascotas mappedResult = _mapper.Map<tbMascotas>(model);
                mappedResult.masc_UsuarioModifica = userId;
                mappedResult.masc_FechaModifica = DateTime.Now;
                return await _mascotaRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
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
                return false;
            }
        }

        #region Dropdown

        /// <summary>
        /// Obtiene una lista de mascotas para su uso en un dropdown.
        /// </summary>
        /// <returns>Una lista de objetos MascotaDropdownViewModel para el dropdown.</returns>
        public IEnumerable<MascotaDropdownViewModel> MascotaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Mascotas_ListResult> mappedResult = _mascotaRepository.MascotasDropdown();
                return _mapper.Map<List<MascotaDropdownViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene una lista de razas para su uso en un dropdown.
        /// </summary>
        /// <returns>Una lista de objetos RazaDropdownViewModel para el dropdown.</returns>
        public IEnumerable<RazaDropdownViewModel> RazaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Raza_DropdownResult> mappedResult = _mascotaRepository.RazaDropdown();
                return _mapper.Map<List<RazaDropdownViewModel>>(mappedResult.ToList());
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
        /// <returns>Una lista de objetos ProcedenciaViewModel para el dropdown.</returns>
        public IEnumerable<ProcedenciaViewModel> ProcedenciaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Procedencia_DropdownResult> mappedResult = _mascotaRepository.ProcedenciaDropdown();
                return _mapper.Map<List<ProcedenciaViewModel>>(mappedResult.ToList());
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
