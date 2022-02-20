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
    public class EventoService
    {
        private readonly EventoRepository _eventoRepository;
        private readonly ILogger<EventoService> _logger;
        private readonly IMapper _mapper;
        public EventoService(EventoRepository eventoRepository, ILogger<EventoService> logger, IMapper mapper)
        {
            _eventoRepository = eventoRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<EventoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Eventos_ListResult> mappedResult = await _eventoRepository.ListAsync();
                return _mapper.Map<List<EventoViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EventoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Eventos_FindResult mappedResult = await _eventoRepository.FindAsync(id);
                return _mapper.Map<EventoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<EventoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Eventos_DetailResult mappedResult = await _eventoRepository.DetailAsync(id);
                return _mapper.Map<EventoViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }
        public async Task<Boolean> AddAsync(EventoViewModel model)
        {
            try
            {
                tbEventos mappedResult = _mapper.Map<tbEventos>(model);
                return await _eventoRepository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }
            
        public async Task<Boolean> UpdateAsync(EventoViewModel model)
        {
            try
            {
                tbEventos mappedResult = _mapper.Map<tbEventos>(model); 
                return await _eventoRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _eventoRepository.RemoveAsync(id);
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
