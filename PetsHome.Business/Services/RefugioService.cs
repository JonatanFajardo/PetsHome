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
    public class RefugioService
    {
        private readonly RefugioRepository _refugioRepository;
        private readonly IMapper _mapper;
        public RefugioService(RefugioRepository refugioRepository, IMapper mapper)
        {
            _refugioRepository = refugioRepository;
            _mapper = mapper;
        }
        public async Task<List<RefugioViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_Refugio_Refugios_ListResult> mappedResult = await _refugioRepository.ListAsync();
                return _mapper.Map<List<RefugioViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<RefugioViewModel> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Refugios_FindResult mappedResult = await _refugioRepository.FindAsync(id);
                return _mapper.Map<RefugioViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<RefugioViewModel> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Refugios_DetailResult mappedResult = await _refugioRepository.DetailAsync(id);
                return _mapper.Map<RefugioViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(RefugioViewModel model)
        {
            try
            {
                tbRefugios mappedResult = _mapper.Map<tbRefugios>(model);
                return await _refugioRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(RefugioViewModel model)
        {
            try
            {
                tbRefugios mappedResult = _mapper.Map<tbRefugios>(model);
                return await _refugioRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _refugioRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
