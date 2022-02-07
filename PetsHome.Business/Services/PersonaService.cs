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
    public class PersonaService
    {
        private readonly PersonaRepository _personaRepository;
        private readonly IMapper _mapper;
        public PersonaService(PersonaRepository personaRepository, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _mapper = mapper;
        }
        public async Task<List<PersonaViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_General_Personas_ListResult> mappedResult = await _personaRepository.ListAsync();
                return _mapper.Map<List<PersonaViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PersonaViewModel> FindAsync(int id)
        {
            try
            {
                PR_General_Personas_FindResult mappedResult = await _personaRepository.FindAsync(id);
                return _mapper.Map<PersonaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PersonaViewModel> DetailAsync(int id)
        {
            try
            {
                PR_General_Personas_DetailResult mappedResult = await _personaRepository.DetailAsync(id);
                return _mapper.Map<PersonaViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(PersonaViewModel model)
        {
            try
            {
                tbPersonas mappedResult = await _personaRepository.AddAsync(mappedResult);
                return await _mapper.Map<tbPersonas>(model);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(PersonaViewModel model)
        {
            try
            {
                tbPersonas mappedResult = await _personaRepository.EditAsync(mappedResult);
                return await _mapper.Map<tbPersonas>(model);
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
                Boolean mappedResult = await _personaRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
