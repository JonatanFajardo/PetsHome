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
    /// Servicio de facturacion / POS. Arma el JSON de lineas y delega la
    /// creacion transaccional al SP via el repositorio.
    /// </summary>
    public class FacturacionService
    {
        private readonly FacturacionRepository _repo;
        private readonly PagoRepository _pagoRepo;
        private readonly ILogger<FacturacionService> _logger;
        private const decimal ISV = 15.00m;

        public FacturacionService(FacturacionRepository repo, PagoRepository pagoRepo, ILogger<FacturacionService> logger)
        {
            _repo = repo;
            _pagoRepo = pagoRepo;
            _logger = logger;
        }

        /// <summary>
        /// Registra un pago/abono sobre una factura. Rechaza sobrepago.
        /// Devuelve (ok, saldo, estado, mensaje).
        /// </summary>
        public async Task<(bool ok, decimal saldo, string estado, string mensaje)> RegistrarPagoAsync(
            int factId, decimal monto, string metodo, string referencia, string notas, int usuario)
        {
            try
            {
                var r = await _pagoRepo.RegistrarAsync(factId, monto, metodo,
                    string.IsNullOrWhiteSpace(referencia) ? null : referencia.Trim(),
                    string.IsNullOrWhiteSpace(notas) ? null : notas.Trim(), usuario);
                if (r == null) return (false, 0m, null, "No se obtuvo respuesta del servidor.");
                return (r.CodeStatus == 1, r.nuevo_saldo, r.fact_Estado, r.MessageStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (false, 0m, null, "Error al registrar el pago.");
            }
        }

        /// <summary>Historial de pagos/abonos de una factura.</summary>
        public async Task<IEnumerable<PR_Veterinaria_Pago_ListPorFacturaResult>> PagosPorFacturaAsync(int factId)
        {
            try { return await _pagoRepo.ListPorFacturaAsync(factId); }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<FacturaResultViewModel> CrearAsync(FacturaCrearViewModel model, int userId)
        {
            try
            {
                if (model?.lineas == null || !model.lineas.Any())
                    return new FacturaResultViewModel { success = false, message = "La factura no tiene lineas." };

                // Mapear al shape que espera OPENJSON del SP (serv_Id / itm_Id)
                var lineasSp = model.lineas.Select(l => new
                {
                    tipo = l.tipo,
                    serv_Id = l.servId,
                    itm_Id = l.itmId,
                    descripcion = l.descripcion,
                    cantidad = l.cantidad,
                    precioUnit = l.precioUnit
                });
                string detalleJson = JsonSerializer.Serialize(lineasSp);

                var r = await _repo.CrearAsync(model.cliId, model.mascId, null, null, ISV, userId, detalleJson);

                if (r == null)
                    return new FacturaResultViewModel { success = false, message = "No se obtuvo respuesta del servidor." };

                return new FacturaResultViewModel
                {
                    success = r.CodeStatus == 1,
                    factId = r.fact_Id,
                    message = r.MessageStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new FacturaResultViewModel { success = false, message = "Error al crear la factura." };
            }
        }

        public async Task<IEnumerable<PR_Veterinaria_Facturas_ListResult>> ListAsync()
        {
            try { return await _repo.ListAsync(); }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<object> DetalleAsync(int id)
        {
            try
            {
                var header = await _repo.FindAsync(id);
                var lineas = await _repo.DetalleAsync(id);
                return new { header, lineas };
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        /// <summary>Comprobante imprimible (encabezado + líneas tipado) de una factura.</summary>
        public async Task<ComprobanteViewModel> ComprobanteAsync(int id)
        {
            try
            {
                var header = await _repo.FindAsync(id);
                if (header == null) return null;
                var lineas = (await _repo.DetalleAsync(id))?.ToList() ?? new List<PR_Veterinaria_FacturaDetalle_ListResult>();
                var pagos = (await _pagoRepo.ListPorFacturaAsync(id))?.ToList() ?? new List<PR_Veterinaria_Pago_ListPorFacturaResult>();
                return new ComprobanteViewModel
                {
                    Header = header,
                    Lineas = lineas,
                    MontoPagado = header.fact_MontoPagado,
                    Saldo = header.Saldo,
                    Pagos = pagos.Select(p => new PagoComprobanteVM
                    {
                        Monto = p.pago_Monto,
                        Metodo = p.pago_Metodo,
                        Fecha = p.pago_Fecha,
                        RecibidoPor = p.RecibidoPorNombre,
                        Referencia = p.pago_Referencia
                    }).ToList()
                };
            }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }

        public async Task<IEnumerable<PR_Veterinaria_ProductosVendibles_DropdownResult>> ProductosVendiblesAsync()
        {
            try { return await _repo.ProductosVendiblesAsync(); }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }
    }
}
