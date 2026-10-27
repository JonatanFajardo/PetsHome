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
    public class MascotaPadecimientoService
    {
        private readonly MascotaPadecimientoRepository _repository;
        private readonly ILogger<MascotaPadecimientoService> _logger;
        private readonly IMapper _mapper;

        public MascotaPadecimientoService(MascotaPadecimientoRepository repository, ILogger<MascotaPadecimientoService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<MascotaPadecimientoViewModel>> ListAsync(int masc_Id)
        {
            try
            {
                var result = await _repository.ListAsync(masc_Id);
                return _mapper.Map<List<MascotaPadecimientoViewModel>>(result.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new List<MascotaPadecimientoViewModel>();
            }
        }

        public async Task<bool> AddAsync(MascotaPadecimientoViewModel model, int usuarioCrea)
        {
            try
            {
                var entity = _mapper.Map<tbMascotaPadecimiento>(model);
                entity.pade_UsuarioCrea = usuarioCrea;
                return (await _repository.AddAsync(entity)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }

        public async Task<bool> CorregirAsync(int pade_Id, int usuarioModifica)
        {
            try
            {
                return (await _repository.CorregirAsync(pade_Id, usuarioModifica)).Success;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return false;
            }
        }
    }
}
