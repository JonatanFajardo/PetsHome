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
        private readonly ILogger<FacturacionService> _logger;
        private const decimal ISV = 15.00m;

        public FacturacionService(FacturacionRepository repo, ILogger<FacturacionService> logger)
        {
            _repo = repo;
            _logger = logger;
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

        public async Task<IEnumerable<PR_Veterinaria_ProductosVendibles_DropdownResult>> ProductosVendiblesAsync()
        {
            try { return await _repo.ProductosVendiblesAsync(); }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); return null; }
        }
    }
}
