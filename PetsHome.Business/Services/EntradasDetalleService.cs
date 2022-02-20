using AutoMapper;
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
        //private readonly EntradasDetalleRepository _entradasdetalleRepository;
        //private readonly ILogger<EntradasDetalleService> _logger;
        //private readonly IMapper _mapper;
        //public EntradasDetalleService(EntradasDetalleRepository entradasdetalleRepository, ILogger<EntradasDetalleService> logger, IMapper mapper)
        //{
        //    _entradasdetalleRepository = entradasdetalleRepository;
        //    _logger = logger;
            //_mapper = mapper;
        //}
        //public async Task<List<EntradaDetalleViewModel>> ListAsync()
        //{
        //    try
        //    {
        //        IEnumerable<PR_Inventario_Entradas_ListResult> mappedResult = await _entradasdetalleRepository.ListAsync();
        //        return _mapper.Map<List<EntradasDetalleViewModel>>(mappedResult.ToList());
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<EntradasDetalleViewModel> FindAsync(int id)
        //{
        //    try
        //    {
        //        PR_Inventario_Entradas_FindResult mappedResult = await _entradasdetalleRepository.FindAsync(id);
        //        return _mapper.Map<EntradasDetalleViewModel>(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<EntradasDetalleViewModel> DetailAsync(int id)
        //{
        //    try
        //    {
        //        PR_Inventario_Entradas_DetailResult mappedResult = await _entradasdetalleRepository.DetailAsync(id);
        //        return _mapper.Map<EntradasDetalleViewModel>(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<Boolean> AddAsync(EntradasDetallViewModel model)
        //{
        //    try
        //    {
        //        tbEntradasDetalles mappedResult = await _mapper.Map<tbEntradasDetalles>(mappedResult);
        //        return await _entradasdetalleRepository.AddAsync(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<Boolean> UpdateAsync(EntradasDetallViewModel model)
        //{
        //    try
        //    {
        //        tbEntradasDetalles mappedResult = await _mapper.Map<tbEntradasDetalles>(mappedResult);
        //        return await _entradasdetalleRepository.EditAsync(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<Boolean> RemoveAsync(int id)
        //{
        //    try
        //    {
        //        Boolean mappedResult = await _entradasdetalleRepository.RemoveAsync(id);
        //        return mappedResult;
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
    }
}
