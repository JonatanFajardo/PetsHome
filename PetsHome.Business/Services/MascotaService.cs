using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Helpers;
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
        private readonly RefugioRepository _refugioRepository;
        private readonly ILogger<MascotaService> _logger;
        private readonly IMapper _mapper;

        public MascotaService(MascotaRepository mascotaRepository,
            RefugioRepository refugioRepository,
        ILogger<MascotaService> logger, IMapper mapper)
        {
            _mascotaRepository = mascotaRepository;
            _refugioRepository = refugioRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<MascotaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Mascotas_ListResult> mappedResult = await _mascotaRepository.ListAsync();
                return _mapper.Map<List<MascotaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<MascotaViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_FindResult mappedResult = await _mascotaRepository.FindAsync(id);
                return _mapper.Map<MascotaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<MascotaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Mascotas_DetailResult mappedResult = await _mascotaRepository.DetailAsync(id);
                return _mapper.Map<MascotaViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        public async Task<Boolean> AddAsync(MascotaViewModel model)
        {
            try
            {
                model.masc_Imagen = await model.ImageFile.GetBytesAsync();
                tbMascotas mappedResult = _mapper.Map<tbMascotas>(model);
                return await _mascotaRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        public async Task<Boolean> UpdateAsync(MascotaViewModel model)
        {
            try
            {
                model.masc_Imagen = await model.ImageFile.GetBytesAsync();
                tbMascotas mappedResult = _mapper.Map<tbMascotas>(model);
                return await _mascotaRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _mascotaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        #region Dropdown

        public IEnumerable<RazaViewModel> RazaDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Raza_DropdownResult> mappedResult = _mascotaRepository.RazaDropdown();
                return _mapper.Map<List<RazaViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        //public IEnumerable<MascotaViewModel> MascotasDropdown()
        //{
        //    try
        //    {
        //        IEnumerable<PR_Refugio_Mascotas_DropdownlResult> mappedResult = _mascotaRepository.MascotasDropdown();
        //        return _mapper.Map<List<MascotaViewModel>>(mappedResult.ToList());
        //    }
        //    catch (Exception error)
        //    {
        //        _logger.LogError(error, error.Message);
        //        return null;
        //    }
        //}

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