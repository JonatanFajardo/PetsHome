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
    public class PersonaService
    {
        private readonly PersonaRepository _personaRepository;
        private readonly ILogger<PersonaService> _logger;
        private readonly IMapper _mapper;
        public PersonaService(PersonaRepository personaRepository, ILogger<PersonaService> logger, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _logger = logger;
            _mapper = mapper;
        }
        //public async Task<List<PersonaViewModel>> ListAsync()
        //{
        //    try
        //    {
        //        IEnumerable<PR_General_Personas_ListResult> mappedResult = await _personaRepository.ListAsync();
        //        return _mapper.Map<List<PersonaViewModel>>(mappedResult.ToList());
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<PersonaViewModel> FindAsync(int id)
        //{
        //    try
        //    {
        //        PR_General_Personas_FindResult mappedResult = await _personaRepository.FindAsync(id);
        //        return _mapper.Map<PersonaViewModel>(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<PersonaViewModel> DetailAsync(int id)
        //{
        //    try
        //    {
        //        PR_General_Personas_DetailResult mappedResult = await _personaRepository.DetailAsync(id);
        //        return _mapper.Map<PersonaViewModel>(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<Boolean> AddAsync(PersonaViewModel model)
        //{
        //    try
        //    {
        //        tbPersonas mappedResult = _mapper.Map<tbPersonas>(model); 
        //        return await _personaRepository.AddAsync(mappedResult);
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<Boolean> UpdateAsync(PersonaViewModel model)
        //{
        //    try
        //    {
        //        tbPersonas mappedResult = _mapper.Map<tbPersonas>(model);
        //        return await _personaRepository.EditAsync(mappedResult); 
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<Boolean> RemoveAsync(int id)
        //{
        //    try
        //    {
        //        Boolean mappedResult = await _personaRepository.RemoveAsync(id);
        //        return mappedResult;
        //    }
        //    catch (Exception error)
        //    {

        //        throw;
        //    }
        //}
    }
}
