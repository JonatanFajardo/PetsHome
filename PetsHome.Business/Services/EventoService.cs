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
    public class EventoService
    {
        private readonly EventoRepository _eventoRepository;
        private readonly IMapper _mapper;
        public EventoService(EventoRepository eventoRepository, IMapper mapper)
        {
            _eventoRepository = eventoRepository;
            _mapper = mapper;
        }
        public async Task<List<EventoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_tbEventos_ListResult> mappedResult = await _eventoRepository.ListAsync();
                return _mapper.Map<List<EventoViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<EventoViewModel> FindAsync(int id)
        {
            try
            {
                PR_tbEventos_FindResult mappedResult = await _eventoRepository.FindAsync(id);
                return _mapper.Map<EventoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<EventoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_tbEventos_DetailResult mappedResult = await _eventoRepository.DetailAsync(id);
                return _mapper.Map<EventoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(EventoViewModel model)
        {
            try
            {
                tbEventos mappedResult = await _eventoRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbEventos>(model);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(EventoViewModel model)
        {
            try
            {
                tbEventos mappedResult = await _eventoRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbEventos>(model);
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
                Boolean mappedResult = await _eventoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
