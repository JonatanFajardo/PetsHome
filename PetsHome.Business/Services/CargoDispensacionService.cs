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
    public class CargoDispensacionService
    {
        private readonly CargoDispensacionRepository _repo;
        private readonly ILogger<CargoDispensacionService> _logger;
        private readonly IMapper _mapper;

        public CargoDispensacionService(CargoDispensacionRepository repo, ILogger<CargoDispensacionService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<CargoDispensacionViewModel>> ListPendientesAsync(int? cli_Id = null)
        {
            try
            {
                var result = await _repo.ListPendientesAsync(cli_Id);
                return _mapper.Map<List<CargoDispensacionViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<CargoDispensacionViewModel>(); }
        }

        /// <summary>Encola un cargo (idempotente por receta). Devuelve true si quedó pendiente/ya existía.</summary>
        public async Task<bool> EncolarAsync(tbCargoDispensacion entity)
        {
            try { return (await _repo.EncolarAsync(entity)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> FacturarAsync(int cargo_Id, int fact_Id, int fdet_Id, int usuario)
        {
            try { return (await _repo.FacturarAsync(cargo_Id, fact_Id, fdet_Id, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> CondonarAsync(int cargo_Id, string motivo, int usuario)
        {
            try { return (await _repo.CondonarAsync(cargo_Id, motivo, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }

        public async Task<bool> ReabrirAsync(int cargo_Id, int usuario)
        {
            try { return (await _repo.ReabrirAsync(cargo_Id, usuario)).Success; }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return false; }
        }
    }
}
