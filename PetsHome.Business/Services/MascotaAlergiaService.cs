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
    public class MascotaAlergiaService
    {
        private readonly MascotaAlergiaRepository _repository;
        private readonly ILogger<MascotaAlergiaService> _logger;
        private readonly IMapper _mapper;

        public MascotaAlergiaService(MascotaAlergiaRepository repository, ILogger<MascotaAlergiaService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<MascotaAlergiaViewModel>> ListAsync(int masc_Id)
        {
            try
            {
                var result = await _repository.ListAsync(masc_Id);
                return _mapper.Map<List<MascotaAlergiaViewModel>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<MascotaAlergiaViewModel>();
            }
        }

        public async Task<bool> AddAsync(MascotaAlergiaViewModel model, int usuarioCrea)
        {
            try
            {
                var entity = _mapper.Map<tbMascotaAlergia>(model);
                entity.alrg_UsuarioCrea = usuarioCrea;
                return (await _repository.AddAsync(entity)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> CorregirAsync(int alrg_Id, int usuarioModifica)
        {
            try
            {
                return (await _repository.CorregirAsync(alrg_Id, usuarioModifica)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}
