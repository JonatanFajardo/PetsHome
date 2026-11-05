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
    /// <summary>
    /// Procedimientos clinicos estructurados (adaptado de OpenVPM "procedures").
    /// Al guardar un procedimiento con serv_Id (servicio facturable) para una
    /// mascota de cliente externo, ENCOLA el cargo reutilizando el SP/servicio
    /// existente (CargoDispensacionService.EncolarServicioDesdeConsultaAsync).
    /// Es un "feed, no gate": si el cobro falla, el procedimiento igual queda guardado.
    /// </summary>
    public class ProcedimientoService
    {
        private readonly ProcedimientoRepository _repo;
        private readonly CargoDispensacionService _cargoService;
        private readonly InventarioSalidaRepository _inventarioSalidaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProcedimientoService> _logger;

        public ProcedimientoService(
            ProcedimientoRepository repo,
            CargoDispensacionService cargoService,
            InventarioSalidaRepository inventarioSalidaRepository,
            IMapper mapper,
            ILogger<ProcedimientoService> logger)
        {
            _repo = repo;
            _cargoService = cargoService;
            _inventarioSalidaRepository = inventarioSalidaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProcedimientoListViewModel>> ListPorMascotaAsync(int mascId)
        {
            try
            {
                var result = await _repo.ListPorMascotaAsync(mascId);
                return _mapper.Map<List<ProcedimientoListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<ProcedimientoListViewModel>(); }
        }

        public async Task<List<ProcedimientoListViewModel>> ListPorCitaAsync(int citaId)
        {
            try
            {
                var result = await _repo.ListPorCitaAsync(citaId);
                return _mapper.Map<List<ProcedimientoListViewModel>>(result.ToList());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return new List<ProcedimientoListViewModel>(); }
        }

        public async Task<ProcedimientoDetailViewModel> DetalleAsync(int procId)
        {
            try
            {
                var result = await _repo.DetailAsync(procId);
                return result == null ? null : _mapper.Map<ProcedimientoDetailViewModel>(result);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        /// <summary>
        /// Guarda (alta o edicion) un procedimiento. Si trae serv_Id y la mascota
        /// es de cliente externo, encola el cargo (cobro) reutilizando lo existente.
        /// usuarioActual = usuario logueado; en alta queda como "realizado por" por defecto.
        /// </summary>
        public async Task<ProcedimientoGuardarResultViewModel> GuardarAsync(ProcedimientoGuardarDto dto, int usuarioActual)
        {
            var res = new ProcedimientoGuardarResultViewModel { Ok = false, ProcId = dto?.ProcId ?? 0 };

            if (dto == null || dto.MascId <= 0)
            {
                res.Mensaje = "Datos del procedimiento invalidos.";
                return res;
            }
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                res.Mensaje = "El nombre del procedimiento es obligatorio.";
                return res;
            }

            try
            {
                int? servId = (dto.ServId.HasValue && dto.ServId.Value > 0) ? dto.ServId : (int?)null;
                int? citaId = (dto.CitaId.HasValue && dto.CitaId.Value > 0) ? dto.CitaId : (int?)null;

                var entity = new tbProcedimientos
                {
                    proc_Id = dto.ProcId,
                    masc_Id = dto.MascId,
                    cita_Id = citaId,
                    proc_Nombre = dto.Nombre.Trim(),
                    proc_Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                    proc_Anestesia = string.IsNullOrWhiteSpace(dto.Anestesia) ? null : dto.Anestesia.Trim(),
                    proc_DuracionMinutos = (dto.DuracionMinutos.HasValue && dto.DuracionMinutos.Value > 0) ? dto.DuracionMinutos : (int?)null,
                    proc_Notas = string.IsNullOrWhiteSpace(dto.Notas) ? null : dto.Notas.Trim(),
                    serv_Id = servId
                };

                if (dto.ProcId > 0)
                {
                    // ── Edicion ──
                    entity.proc_UsuarioModifica = usuarioActual;
                    var upd = await _repo.EditAsync(entity);
                    if (!upd.Success)
                    {
                        res.Mensaje = upd.MessageStatus ?? "No se pudo actualizar el procedimiento.";
                        return res;
                    }
                    res.ProcId = dto.ProcId;
                    res.Mensaje = "Procedimiento actualizado.";
                }
                else
                {
                    // ── Alta: "realizado por" = usuario logueado por defecto ──
                    entity.proc_RealizadoPor = usuarioActual;
                    entity.proc_UsuarioCrea = usuarioActual;
                    var (procId, mensaje) = await _repo.AddReturningIdAsync(entity);
                    if (procId <= 0)
                    {
                        res.Mensaje = mensaje ?? "No se pudo registrar el procedimiento.";
                        return res;
                    }
                    res.ProcId = procId;
                    res.Mensaje = "Procedimiento registrado.";
                }

                res.Ok = true;

                // ── COBRO (feed, no gate): si el procedimiento tiene servicio facturable
                //    y la mascota es de cliente externo, encolar el cargo reutilizando
                //    el SP/servicio que YA existe. Idempotente por (cita, servicio).
                if (servId.HasValue)
                {
                    try
                    {
                        int? cliId = null;
                        var info = await _inventarioSalidaRepository.RefugioPorMascotaAsync(dto.MascId);
                        if (info != null && info.cli_Id.HasValue && info.cli_Id.Value > 0)
                            cliId = info.cli_Id;

                        if (cliId.HasValue)
                        {
                            bool encolado = await _cargoService.EncolarServicioDesdeConsultaAsync(
                                servId.Value, dto.MascId, cliId.Value, citaId, 1, usuarioActual);
                            res.CargoEncolado = encolado;
                        }
                    }
                    catch (Exception cargoError)
                    {
                        // El procedimiento ya quedo guardado; el cobro es best-effort.
                        _logger.LogWarning(cargoError, "No se pudo encolar el cargo del servicio {ServId} del procedimiento {ProcId}", servId, res.ProcId);
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                res.Ok = false;
                res.Mensaje = "Ocurrio un error al guardar el procedimiento.";
                return res;
            }
        }

        public async Task<(bool ok, string mensaje)> EliminarAsync(int procId, int usuario)
        {
            try
            {
                var r = await _repo.RemoveAsync(procId, usuario);
                return (r.Success, r.MessageStatus);
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return (false, "No se pudo eliminar el procedimiento."); }
        }
    }
}
