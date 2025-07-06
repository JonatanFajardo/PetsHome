using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities; 
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetsHome.Business.Services
{
    public class ComportamientosService
    {
        private readonly CitaMedicaRepository _citaMedicaRepository; 
        private readonly ILogger<MascotaService> _logger;
        private readonly IMapper _mapper;

        public ComportamientosService(CitaMedicaRepository citaMedicaRepository, ILogger<MascotaService> logger, IMapper mapper)
        {
            _citaMedicaRepository = citaMedicaRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<ComportamientoViewModel> ComportamientoDropdown()
        {
            try
            {
                IEnumerable<PR_Refugio_Comportamiento_ListResult> mappedResult = _citaMedicaRepository.ComportamientoList();
                return _mapper.Map<IEnumerable<ComportamientoViewModel>>(mappedResult.ToList()); 
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
    }
}
