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
    public class HistorialMedicoService
    {
        private readonly HistorialMedicoRepository _historialmedicoRepository;
        private readonly IMapper _mapper;
        public HistorialMedicoService(HistorialMedicoRepository historialmedicoRepository, IMapper mapper)
        {
            _historialmedicoRepository = historialmedicoRepository;
            _mapper = mapper;
        }
        public async Task<List<HistorialMedicoViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_HistorialMedico_ListResult> mappedResult = await _historialmedicoRepository.ListAsync();
                return _mapper.Map<List<HistorialMedicoViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<HistorialMedicoViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_HistorialMedico_FindResult mappedResult = await _historialmedicoRepository.FindAsync(id);
                return _mapper.Map<HistorialMedicoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<HistorialMedicoViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_HistorialMedico_DetailResult mappedResult = await _historialmedicoRepository.DetailAsync(id);
                return _mapper.Map<HistorialMedicoViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(HistorialMedicoViewModel model)
        {
            try
            {
                tbHistorialMedicos mappedResult = await _historialmedicoRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbHistorialMedicos>(model);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(HistorialMedicoViewModel model)
        {
            try
            {
                tbHistorialMedicos mappedResult = await _historialmedicoRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbHistorialMedicos>(model);
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
                Boolean mappedResult = await _historialmedicoRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
