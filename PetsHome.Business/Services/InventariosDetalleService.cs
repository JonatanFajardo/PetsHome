using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business.Services
{
    public class InventariosDetalleService
    {
        private readonly InventariosDetalleRepository _inventariosdetalleRepository;
        private readonly ILogger<InventariosDetalleService> _logger;
        private readonly IMapper _mapper;

        public InventariosDetalleService(InventariosDetalleRepository inventariosdetalleRepository, ILogger<InventariosDetalleService> logger, IMapper mapper)
        {
            _inventariosdetalleRepository = inventariosdetalleRepository;
            _logger = logger;
            _mapper = mapper;
        }

        //public async Task<List<InventarioDetalleViewModel>> ListAsync()
        //{
        //    try
        //    {
        //        IEnumerable<PR_Inventario_InventariosDetalles_ListResult> mappedResult = await _inventariosdetalleRepository.ListAsync();
        //        return _mapper.Map<List<InventariosDetalleViewModel>>(mappedResult.ToList());
        //    }
        //    catch (Exception error)
        //    {
        //        throw;
        //    }
        //}
        //public async Task<InventariosDetalleViewModel> FindAsync(int id)
        //{
        //    try
        //    {
        //        PR_Inventario_InventariosDetalles_FindResult mappedResult = await _inventariosdetalleRepository.FindAsync(id);
        //        return _mapper.Map<InventariosDetalleViewModel>(mappedResult);
        //    }
        //    catch (Exception error)
        //    {
        //        throw;
        //    }
        //}
        //public async Task<InventariosDetalleViewModel> DetailAsync(int id)
        //{
        //    try
        //    {
        //        PR_Inventario_InventariosDetalles_DetailResult mappedResult = await _inventariosdetalleRepository.DetailAsync(id);
        //        return _mapper.Map < InventariosDetalleViewModel >> (mappedResult.ToList());
        //    }
        //    catch (Exception error)
        //    {
        //        throw;
        //    }
        //}
        //public async Task<Boolean> AddAsync(InventariosDetallViewModel model)
        //{
        //    try
        //    {
        //        tbInventariosDetalles mappedResult = await _inventariosdetalleRepository.AddAsync(mappedResult);
        //        return await _mapper.Map<tbInventariosDetalles>(model);
        //    }
        //    catch (Exception error)
        //    {
        //        throw;
        //    }
        //}
        //public async Task<Boolean> UpdateAsync(InventariosDetallViewModel model)
        //{
        //    try
        //    {
        //        tbInventariosDetalles mappedResult = await _inventariosdetalleRepository.EditAsync(mappedResult);
        //        return await _mapper.Map<tbInventariosDetalles>(model);
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
        //        Boolean mappedResult = await _inventariosdetalleRepository.RemoveAsync(id);
        //        return mappedResult;
        //    }
        //    catch (Exception error)
        //    {
        //        throw;
        //    }
        //}
    }
}