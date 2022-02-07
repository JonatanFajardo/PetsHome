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
    public class VoluntarioService
    {

        private readonly VoluntarioRepository _voluntarioRepository;
        private readonly IMapper _mapper;
        public VoluntarioService(VoluntarioRepository voluntarioRepository, IMapper mapper)
        {
            _voluntarioRepository = voluntarioRepository;
            _mapper = mapper;
        }
        public async Task<List<PR_Refugio_Voluntarios_ListResult>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Voluntarios_ListResult> mappedResult = await _voluntarioRepository.ListAsync();
                return _mapper.Map<List<VoluntarioViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PR_Refugio_Voluntarios_FindResult> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Voluntarios_FindResult mappedResult = await _voluntarioRepository.FindAsync(id);
                return _mapper.Map<VoluntarioViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PR_Refugio_Voluntarios_DetailResult> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Voluntarios_DetailResult mappedResult = await _voluntarioRepository.DetailAsync(id);
                return _mapper.Map<VoluntarioViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(VoluntarioViewModel model)
        {
            try
            {
                tbVoluntarios mappedResult = await _voluntarioRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbVoluntarios>(model);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(VoluntarioViewModel model)
        {
            try
            {
                tbVoluntarios mappedResult = _mapper.Map<tbVoluntarios>(model);
                return await _voluntarioRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _voluntarioRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
