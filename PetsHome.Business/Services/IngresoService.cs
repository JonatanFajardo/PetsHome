using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio que gestiona los ingresos de animales al refugio.
    /// </summary>
    public class IngresoService
    {
        private readonly ILogger<IngresoService> _logger;
        private readonly IMapper _mapper;
        private readonly IngresoRepository _ingresoRepository;

        public IngresoService(IngresoRepository ingresoRepository, ILogger<IngresoService> logger, IMapper mapper)
        {
            _ingresoRepository = ingresoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<IngresoListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Rescate_Ingresos_ListResult> mappedResult = await _ingresoRepository.ListAsync();
                return _mapper.Map<List<IngresoListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<IngresoFormViewModel> FindAsync(int id)
        {
            try
            {
                PR_Rescate_Ingresos_FindResult mappedResult = await _ingresoRepository.FindAsync(id);
                return _mapper.Map<IngresoFormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<IngresoDetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Rescate_Ingresos_DetailResult mappedResult = await _ingresoRepository.DetailAsync(id);
                return _mapper.Map<IngresoDetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<bool> AddAsync(IngresoFormViewModel model)
        {
            try
            {
                tbIngresos mappedResult = _mapper.Map<tbIngresos>(model);
                return await _ingresoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> UpdateAsync(IngresoFormViewModel model)
        {
            try
            {
                tbIngresos mappedResult = _mapper.Map<tbIngresos>(model);
                return await _ingresoRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return await _ingresoRepository.RemoveAsync(id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public SelectList IngresoDropdown()
        {
            try
            {
                var items = _ingresoRepository.DropdownAsync().Result;
                return new SelectList(items, "ingr_Id", "ingr_LugarRescate");
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
