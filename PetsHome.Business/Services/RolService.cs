using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class RolService
    {
        private readonly RolRepository _rolRepository;
        private readonly PantallaRepository _pantallaRepository;
        private readonly IMapper _mapper;

        public RolService(RolRepository rolRepository, PantallaRepository pantallaRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _pantallaRepository = pantallaRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RolViewModel>> ListAsync()
        {
            var result = await _rolRepository.ListAsync();
            return _mapper.Map<IEnumerable<RolViewModel>>(result);
        }

        public async Task<RolViewModel> FindAsync(int id)
        {
            var result = await _rolRepository.FindAsync(id);
            return _mapper.Map<RolViewModel>(result);
        }

        public async Task<bool> CreateAsync(RolViewModel model, int usuarioCrea)
        {
            return await _rolRepository.InsertAsync(model.rol_Descripcion, model.rol_Estado, usuarioCrea);
        }

        public async Task<bool> EditAsync(RolViewModel model, int usuarioModifica)
        {
            return await _rolRepository.UpdateAsync(model.rol_Id, model.rol_Descripcion, model.rol_Estado, usuarioModifica);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _rolRepository.DeleteAsync(id);
        }

        public async Task<RolViewModel> ExistAsync(string descripcion)
        {
            var result = await _rolRepository.ExistAsync(descripcion);
            return result != null ? new RolViewModel { rol_Id = result.rol_Id, rol_Descripcion = result.rol_Descripcion } : null;
        }

        public async Task<IEnumerable<RolDropdownViewModel>> DropdownAsync()
        {
            var result = await _rolRepository.DropdownAsync();
            return _mapper.Map<IEnumerable<RolDropdownViewModel>>(result);
        }

        public async Task<IEnumerable<PantallaItemViewModel>> PantallasListAsync()
        {
            var result = await _pantallaRepository.ListAsync();
            return _mapper.Map<IEnumerable<PantallaItemViewModel>>(result);
        }

        public async Task<IEnumerable<PantallaIdViewModel>> PantallasByRolAsync(int rolId)
        {
            var result = await _pantallaRepository.ByRolAsync(rolId);
            return result.Select(r => new PantallaIdViewModel { pan_Id = r.pan_Id });
        }

        public async Task<bool> SavePantallasAsync(int rolId, string pantallaIds)
        {
            return await _pantallaRepository.SaveByRolAsync(rolId, pantallaIds);
        }

        public async Task<string> GetPantallasStringByRolAsync(int rolId)
        {
            var result = await _pantallaRepository.NombresByRolAsync(rolId);
            return string.Join(",", result.Select(r => r.pan_Descripcion));
        }
    }
}
