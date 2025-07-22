using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class ExistenciasService
    {
        private readonly ExistenciasRepository _existenciasRepository;
        private readonly IMapper _mapper;

        public ExistenciasService(ExistenciasRepository existenciasRepository, IMapper mapper)
        {
            _existenciasRepository = existenciasRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<dynamic>> ListAsync()
        {
            try
            {
                return await _existenciasRepository.ListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ExistenciaViewModel> FindAsync(int id)
        {
            try
            {
                var resultado = await _existenciasRepository.FindAsync(id);
                return _mapper.Map<ExistenciaViewModel>(resultado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ExistenciaViewModel> DetailAsync(int id)
        {
            try
            {
                var resultado = await _existenciasRepository.DetailAsync(id);
                return _mapper.Map<ExistenciaViewModel>(resultado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> GetByItemAndRefugioAsync(int itemId, int refugioId)
        {
            try
            {
                return await _existenciasRepository.GetByItemAndRefugioAsync(itemId, refugioId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddAsync(ExistenciaViewModel model)
        {
            try
            {
                var entity = _mapper.Map<tbExistencias>(model);
                return await _existenciasRepository.AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ExistenciaViewModel model)
        {
            try
            {
                var entity = _mapper.Map<tbExistencias>(model);
                return await _existenciasRepository.EditAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int itemId, int refugioId, int nuevoStock)
        {
            try
            {
                return await _existenciasRepository.UpdateStockAsync(itemId, refugioId, nuevoStock);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                return await _existenciasRepository.RemoveAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}