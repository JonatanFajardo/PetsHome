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
    /// <summary>
    /// Servicio del modulo Lista de espera POR DEMANDA (waitlist).
    /// Reusa TipoConsultaService para el dropdown de tipos de consulta.
    /// </summary>
    public class ListaEsperaService
    {
        private readonly ListaEsperaRepository _repo;
        private readonly TipoConsultaService _tipoConsultaService;
        private readonly ILogger<ListaEsperaService> _logger;
        private readonly IMapper _mapper;

        public ListaEsperaService(ListaEsperaRepository repo, TipoConsultaService tipoConsultaService,
            ILogger<ListaEsperaService> logger, IMapper mapper)
        {
            _repo = repo;
            _tipoConsultaService = tipoConsultaService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ListaEsperaListViewModel>> ListAsync(int? mascId, int? tipoConId, string estado)
        {
            try
            {
                var result = await _repo.ListAsync(mascId, tipoConId, estado);
                return _mapper.Map<List<ListaEsperaListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<List<ListaEsperaSugerenciaViewModel>> SugerenciasAsync(DateTime? fecha, int? tipoConId)
        {
            try
            {
                var result = await _repo.SugerenciasAsync(fecha, tipoConId);
                return _mapper.Map<List<ListaEsperaSugerenciaViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ListaEsperaFindViewModel> FindAsync(int id)
        {
            try
            {
                var result = await _repo.FindAsync(id);
                return _mapper.Map<ListaEsperaFindViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<ListaEsperaDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var header = await _repo.DetailAsync(id);
                if (header == null) return null;
                return _mapper.Map<ListaEsperaDetailViewModel>(header);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        /// <summary>Inserta un registro y devuelve su lesp_Id (0 si falla).</summary>
        public async Task<int> AddAsync(ListaEsperaFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbListaEspera>(model);
                entity.lesp_UsuarioCrea = userId;
                return await _repo.AddAsync(entity);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return 0; }
        }

        public async Task<bool> UpdateAsync(ListaEsperaFormViewModel model, int userId)
        {
            try
            {
                var entity = _mapper.Map<tbListaEspera>(model);
                entity.lesp_UsuarioModifica = userId;
                return (await _repo.EditAsync(entity)).Success;
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> CambiarEstadoAsync(int lespId, string estado, int userId)
        {
            try { return (await _repo.CambiarEstadoAsync(lespId, estado, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> RemoveAsync(int id, int userId)
        {
            try { return (await _repo.RemoveAsync(id, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public List<MascotaDropdownViewModel> MascotaDropdown()
        {
            try
            {
                var result = _repo.MascotasDropdown();
                return _mapper.Map<List<MascotaDropdownViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<MascotaDropdownViewModel>(); }
        }

        public async Task<List<TipoConsultaViewModel>> TipoConsultaDropdown()
        {
            try { return await _tipoConsultaService.ListAsync() ?? new List<TipoConsultaViewModel>(); }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<TipoConsultaViewModel>(); }
        }
    }
}
