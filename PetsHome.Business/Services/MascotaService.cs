using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class MascotaService
    {
        private readonly MascotaRepository _mascotaRepository;
        private readonly IMapper _mapper;
        public MascotaService(MascotaRepository mascotaRepository, IMapper mapper)
        {
            _mascotaRepository = mascotaRepository;
            _mapper = mapper;
        }
        public async Task<List<MascotaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Mascotas_ListResult> mappedResult = await _mascotaRepository.ListAsync();
                return _mapper.Map<List<MascotaViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<MascotaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_FindResult mappedResult = await _mascotaRepository.FindAsync(id);
                return _mapper.Map<MascotaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<MascotaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_DetailResult mappedResult = await _mascotaRepository.DetailAsync(id);
                return _mapper.Map<MascotaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(MascotaViewModel model)
        {
            try
            {
                tbMascotas mappedResult = await _mascotaRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbMascotas>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(MascotaViewModel model)
        {
            try
            {
                tbMascotas mappedResult = await _mascotaRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbMascotas>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> RemoveAsync(int id)
        {
            try
            {
                Boolean mappedResult = await _mascotaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public IEnumerable<RazaViewModel> RazaDropdown()
        {
            IEnumerable<PR_Refugio_Raza_DropdownResult> mappedResult = _mascotaRepository.RazaDropdown();
            return _mapper.Map<List<MascotaViewModel>>(mappedResult.ToList());
        }

        public IEnumerable<RazaViewModel> RefugioDropdown()
        {
            IEnumerable<PR_Refugio_Refugio_DropdownResult> mappedResult = _mascotaRepository.RefugioDropdown();
            return _mapper.Map<List<MascotaViewModel>>(mappedResult.ToList());
        }

        public IEnumerable<RazaViewModel> ProcedenciaDropdown()
        {
            IEnumerable<PR_Refugio_Procedencia_DropdownResult> mappedResult = _mascotaRepository.ProcedenciaDropdown();
            return _mapper.Map<List<MascotaViewModel>>(mappedResult.ToList());
        }


    }
}
