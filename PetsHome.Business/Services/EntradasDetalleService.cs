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
    public class EntradasDetalleService
    {
        private readonly EntradasDetalleRepository _entradaDetalleRepository;
        private readonly ItemRepository _itemRepository;
        private readonly ILogger<EntradasDetalleService> _logger;
        private readonly IMapper _mapper;
        public EntradasDetalleService(EntradasDetalleRepository municipioRepository, 
            ILogger<EntradasDetalleService> logger, 
            IMapper mapper)
        {
            _entradaDetalleRepository = municipioRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<EntradaDetalleViewModel>> ListIdAsync(int id)
        {
            try
            {
                IEnumerable<PR_Inventario_EntradasDetalles_ListResult> mappedResult = await _entradaDetalleRepository.ListIdAsync(id);
                return _mapper.Map<List<EntradaDetalleViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EntradaViewModel> FindAsync(int id)
        {
            try
            {
                var mappedResult = await _entradaDetalleRepository.FindAsync(id);
                //mappedResult.
                return MappingCustom.Map(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        //public async Task<MunicipioViewModel> DetailAsync(int id)
        //{
        //   try
        //    {
        //        IEnumerable<PR_tbMunicipios_DetailResult> mappedResult = await _entradaDetalleRepository.DetailAsync(id);
        //        return _mapper.Map<MunicipioViewModel>(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        public async Task<Boolean> AddAsync(EntradaDetalleViewModel model)
        {
            try
            {
                tbEntradasDetalles mappedResult = _mapper.Map<tbEntradasDetalles>(model);
                return await _entradaDetalleRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<Boolean> UpdateAsync(EntradaDetalleViewModel model)
        {
            try
            {
                tbEntradasDetalles mappedResult = _mapper.Map<tbEntradasDetalles>(model);
                return await _entradaDetalleRepository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _entradaDetalleRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
    }
}
