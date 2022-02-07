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
    public class SolicitudService
    {
        private readonly SolicitudeRepository _solicitudeRepository;
        private readonly IMapper _mapper;
        public SolicitudeService(SolicitudeRepository solicitudeRepository, IMapper mapper)
        {
            _solicitudeRepository = solicitudeRepository;
            _mapper = mapper;
        }
        public async Task<List<PR_Refugio_Solicitudes_ListResult>> ListAsync()
        {
            try
            {
                IEnumerable<List> mappedResult = await _solicitudeRepository.ListAsync();
                return _mapper.Map<List<SolicitudViewModel>>(mappedResult.ToList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PR_Refugio_Solicitudes_FindResult> FindAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_FindResult mappedResult = await _solicitudeRepository.FindAsync(id);
                return _mapper.Map<SolicitudViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<PR_Refugio_Solicitudes_DetailResult> DetailAsync(int id)
        {
            try
            {
                PR_Refugio_Solicitudes_DetailResult mappedResult = await _solicitudeRepository.DetailAsync(id);
                return _mapper.Map<SolicitudViewModel>(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> AddAsync(SolicitudViewModel model)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                return await _solicitudeRepository.AddAsync(mappedResult);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<Boolean> UpdateAsync(SolicitudViewModel model)
        {
            try
            {
                tbSolicitudes mappedResult = _mapper.Map<tbSolicitudes>(model);
                return await _solicitudeRepository.EditAsync(mappedResult);
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
                Boolean mappedResult = await _solicitudeRepository.RemoveAsync(id);
                return mappedResult;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
