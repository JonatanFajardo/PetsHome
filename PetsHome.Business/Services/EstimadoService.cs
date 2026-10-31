using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio de estimados / presupuestos. Arma el JSON de lineas y delega la
    /// escritura transaccional al SP via el repositorio. La conversion a factura
    /// la resuelve un SP propio que inserta en las tablas de factura.
    /// </summary>
    public class EstimadoService
    {
        private readonly EstimadoRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<EstimadoService> _logger;

        public EstimadoService(EstimadoRepository repo, IMapper mapper, ILogger<EstimadoService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        private static object[] LineasToSp(IEnumerable<EstimadoLineaViewModel> lineas)
        {
            return lineas.Select(l => (object)new
            {
                tipo = l.tipo,
                serv_Id = l.servId,
                itm_Id = l.itmId,
                descripcion = l.descripcion,
                cantidad = l.cantidad,
                precioUnit = l.precioUnit
            }).ToArray();
        }

        public async Task<EstimadoResultViewModel> GuardarAsync(EstimadoFormViewModel model, int userId)
        {
            try
            {
                if (model?.lineas == null || !model.lineas.Any())
                    return new EstimadoResultViewModel { success = false, message = "El presupuesto no tiene lineas." };

                string detalleJson = JsonSerializer.Serialize(LineasToSp(model.lineas));

                if (model.estId.HasValue && model.estId.Value > 0)
                {
                    // Editar (solo borrador)
                    var ru = await _repo.ActualizarAsync(model.estId.Value, model.cliId, model.mascId,
                        model.vigencia, string.IsNullOrWhiteSpace(model.notas) ? null : model.notas.Trim(),
                        userId, detalleJson);
                    return new EstimadoResultViewModel
                    {
                        success = ru != null && ru.Success,
                        estId = model.estId,
                        message = ru?.MessageStatus ?? "No se obtuvo respuesta del servidor."
                    };
                }

                var r = await _repo.CrearAsync(model.cliId, model.mascId, model.vigencia,
                    string.IsNullOrWhiteSpace(model.notas) ? null : model.notas.Trim(), userId, detalleJson);
                if (r == null)
                    return new EstimadoResultViewModel { success = false, message = "No se obtuvo respuesta del servidor." };

                return new EstimadoResultViewModel
                {
                    success = r.CodeStatus == 1,
                    estId = r.est_Id,
                    message = r.MessageStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new EstimadoResultViewModel { success = false, message = "Error al guardar el presupuesto." };
            }
        }

        public async Task<IEnumerable<EstimadoListViewModel>> ListAsync(string estado)
        {
            try
            {
                var rows = await _repo.ListAsync(estado);
                return _mapper.Map<IEnumerable<EstimadoListViewModel>>(rows ?? Enumerable.Empty<PR_Veterinaria_Estimados_ListResult>());
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<EstimadoDetailViewModel> DetalleAsync(int id)
        {
            try
            {
                var header = await _repo.FindAsync(id);
                if (header == null) return null;
                var lineas = await _repo.DetalleAsync(id);
                return new EstimadoDetailViewModel
                {
                    Header = _mapper.Map<EstimadoFindViewModel>(header),
                    Lineas = _mapper.Map<List<EstimadoDetalleViewModel>>(
                        (lineas ?? Enumerable.Empty<PR_Veterinaria_EstimadoDetalle_ListResult>()).ToList())
                };
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<EstimadoResultViewModel> CambiarEstadoAsync(int estId, string estado, int userId)
        {
            try
            {
                var r = await _repo.CambiarEstadoAsync(estId, estado, userId);
                return new EstimadoResultViewModel
                {
                    success = r != null && r.Success,
                    estId = estId,
                    message = r?.MessageStatus ?? "No se obtuvo respuesta del servidor."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new EstimadoResultViewModel { success = false, message = "Error al cambiar el estado." };
            }
        }

        public async Task<EstimadoResultViewModel> ConvertirAFacturaAsync(int estId, int userId)
        {
            try
            {
                var r = await _repo.ConvertirAFacturaAsync(estId, userId);
                if (r == null)
                    return new EstimadoResultViewModel { success = false, message = "No se obtuvo respuesta del servidor." };
                return new EstimadoResultViewModel
                {
                    success = r.CodeStatus == 1,
                    estId = estId,
                    factId = r.fact_Id,
                    message = r.MessageStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new EstimadoResultViewModel { success = false, message = "Error al convertir el presupuesto." };
            }
        }
    }
}
