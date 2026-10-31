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
    public class RecetaService
    {
        private readonly RecetaRepository _recetaRepository;
        private readonly ILogger<RecetaService> _logger;
        private readonly IMapper _mapper;

        public RecetaService(RecetaRepository recetaRepository, ILogger<RecetaService> logger, IMapper mapper)
        {
            _recetaRepository = recetaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<RecetaListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Medico_Recetas_ListResult> mappedResult = await _recetaRepository.ListAsync();
                return _mapper.Map<List<RecetaListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RecetaFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Medico_Recetas_FindResult mappedResult = await _recetaRepository.FindAsync(id);
                return _mapper.Map<RecetaFormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<RecetaDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Medico_Recetas_DetailResult mappedResult = await _recetaRepository.DetailAsync(id);
                return _mapper.Map<RecetaDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(RecetaFormViewModel model)
        {
            try
            {
                tbRecetas mappedResult = _mapper.Map<tbRecetas>(model);
                return (await _recetaRepository.AddAsync(mappedResult)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>Inserta una receta y devuelve su nuevo id (0 si falló).</summary>
        public async Task<int> AddReturningIdAsync(RecetaFormViewModel model)
        {
            try
            {
                tbRecetas mappedResult = _mapper.Map<tbRecetas>(model);
                return await _recetaRepository.AddReturningIdAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return 0;
            }
        }

        public async Task<bool> UpdateAsync(RecetaFormViewModel model)
        {
            try
            {
                tbRecetas mappedResult = _mapper.Map<tbRecetas>(model);
                return (await _recetaRepository.EditAsync(mappedResult)).Success;
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
                var mappedResult = await _recetaRepository.RemoveAsync(id);
                return mappedResult.Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        /// <summary>Lista plana del vademecum para la calculadora de dosis por peso.</summary>
        public async Task<IEnumerable<PR_Medico_Vademecum_ListResult>> VademecumListAsync(string especie = null, string categoria = null)
        {
            try
            {
                return await _recetaRepository.VademecumListAsync(especie, categoria);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<PR_Medico_Vademecum_ListResult>();
            }
        }

        /// <summary>Registra un resurtido de la receta. Devuelve estado y conteos.</summary>
        public async Task<PR_Medico_Recetas_ResurtirResult> ResurtirAsync(int recetaId, int usuario)
        {
            try
            {
                return await _recetaRepository.ResurtirAsync(recetaId, usuario);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new PR_Medico_Recetas_ResurtirResult
                {
                    CodeStatus = -5,
                    MessageStatus = "Ocurrio un error al registrar el resurtido."
                };
            }
        }

        public IEnumerable<object> RecetaDropdown(int? masc_Id = null)
        {
            try
            {
                IEnumerable<PR_Medico_Recetas_DropdownResult> mappedResult = _recetaRepository.Dropdown(masc_Id);
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
