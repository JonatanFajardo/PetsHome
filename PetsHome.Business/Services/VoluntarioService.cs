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
    public class VoluntarioService
    {
        private readonly VoluntarioRepository _voluntarioRepository;
        private readonly ILogger<VoluntarioService> _logger;
        private readonly IMapper _mapper;

        public VoluntarioService(VoluntarioRepository voluntarioRepository, ILogger<VoluntarioService> logger, IMapper mapper)
        {
            _voluntarioRepository = voluntarioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene la lista de voluntarios
        /// </summary>
        /// <returns></returns>
        public async Task<List<VoluntarioListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Voluntarios_ListResult> mappedResult = await _voluntarioRepository.ListAsync();
                return _mapper.Map<List<VoluntarioListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<VoluntarioFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Voluntarios_FindResult mappedResult = await _voluntarioRepository.FindAsync(id);
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<VoluntarioDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Voluntarios_DetailResult mappedResult = await _voluntarioRepository.DetailAsync(id);
                return _mapper.Map<VoluntarioDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(VoluntarioFormViewModel model, int userId)
        {
            try
            {
                tbVoluntarios mappedResult = _mapper.Map<tbVoluntarios>(model);
                if (mappedResult.per == null)
                {
                    mappedResult.per = new tbPersonas();
                }
                mappedResult.per.per_UsuarioCrea = userId;
                mappedResult.per.per_FechaCrea = DateTime.Now;
                return (await _voluntarioRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(VoluntarioFormViewModel model, int userId)
        {
            try
            {
                tbVoluntarios mappedResult = _mapper.Map<tbVoluntarios>(model);
                if (mappedResult.per == null)
                {
                    mappedResult.per = new tbPersonas();
                }
                mappedResult.per.per_UsuarioModifica = userId;
                mappedResult.per.per_FechaModifica = DateTime.Now;
                return (await _voluntarioRepository.EditAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                var mappedResult = await _voluntarioRepository.RemoveAsync(id);
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
