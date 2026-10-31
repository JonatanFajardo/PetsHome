using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio del modulo RECORDATORIOS / RECALLS (care-reminders de OpenVPM).
    /// Un recordatorio es una tarea clinica interna: "a esta mascota le toca algo
    /// en tal fecha" (refuerzo de vacuna, seguimiento). La auto-generacion crea
    /// recordatorios pendientes desde las vacunas aplicadas y las proximas citas,
    /// sin duplicar (idempotente).
    /// </summary>
    public class RecordatorioService
    {
        private readonly RecordatorioRepository _repo;
        private readonly ILogger<RecordatorioService> _logger;
        private readonly IMapper _mapper;

        public RecordatorioService(RecordatorioRepository repo, ILogger<RecordatorioService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<RecordatorioListViewModel>> ListAsync(string estado, DateTime? desde, DateTime? hasta, int? mascId)
        {
            try
            {
                var result = await _repo.ListAsync(estado, desde, hasta, mascId);
                return _mapper.Map<List<RecordatorioListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<RecordatorioDetailViewModel> DetailAsync(int id)
        {
            try
            {
                var header = await _repo.DetailAsync(id);
                if (header == null) return null;
                return _mapper.Map<RecordatorioDetailViewModel>(header);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        /// <summary>Genera recordatorios pendientes desde la data real. Devuelve cuantos creo (-1 si error).</summary>
        public async Task<int> GenerarAsync(int userId)
        {
            try { return await _repo.GenerarAsync(userId); }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return -1; }
        }

        public async Task<bool> MarcarHechoAsync(int recId, int userId)
        {
            try { return (await _repo.MarcarHechoAsync(recId, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> DescartarAsync(int recId, int userId, string nota)
        {
            try { return (await _repo.DescartarAsync(recId, userId, nota)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> MarcarEnviadoAsync(int recId, int userId)
        {
            try { return (await _repo.MarcarEnviadoAsync(recId, userId)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }
    }
}
