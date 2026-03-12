using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.Common;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PetsHome.Tests
{
    /// <summary>
    /// Pruebas de diagnostico para identificar errores SQL ocultos.
    /// Llaman directamente al SP sin pasar por DbApp para ver el error real.
    /// </summary>
    public class DiagnosticTests : IClassFixture<DatabaseFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly string _cs = "Data source=(localdb)\\MSSQLLocalDB;Initial Catalog=PETSHOMEDB;Integrated Security=True;Encrypt=False";

        public DiagnosticTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Diagnostico_EmpleadosCargo_DeleteSP()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            var ins = new DynamicParameters();
            ins.Add("@cag_Descripcion", "DIAG_EMPL_" + Guid.NewGuid().ToString().Substring(0, 6));
            ins.Add("@cag_Salario", 100.00m, DbType.Decimal);
            ins.Add("@cag_EsActivo", true, DbType.Boolean);
            ins.Add("@cag_UsuarioCrea", 1, DbType.Int32);

            try
            {
                var insResult = await db.ExecuteAsync("[Refugio].[PR_Refugio_EmpleadosCargos_Insert]", ins, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"INSERT result: {insResult}");
            }
            catch (Exception ex) { _output.WriteLine($"INSERT ERROR: {ex.Message}"); return; }

            var list = await db.QueryAsync("[Refugio].[PR_Refugio_EmpleadosCargos_List]", commandType: CommandType.StoredProcedure);
            dynamic last = list.OrderByDescending(x => (int)x.cag_Id).FirstOrDefault(x => ((string)x.cag_Descripcion).StartsWith("DIAG_EMPL_"));
            if (last == null) { _output.WriteLine("No insert found"); return; }
            int id = (int)last.cag_Id;
            _output.WriteLine($"Inserted ID: {id}");

            try
            {
                var del = new DynamicParameters();
                del.Add("@cag_Id", id, DbType.Int32);
                var delResult = await db.ExecuteAsync("[General].[PR_General_EmpleadosCargos_Delete]", del, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"DELETE result: {delResult}");
                Assert.True(delResult > 0 || delResult == -1, $"DELETE returned {delResult}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"DELETE ERROR: {ex.Message}");
                Assert.True(false, $"DELETE SP failed: {ex.Message}");
            }
        }

        [Fact]
        public async Task Diagnostico_Refugio_DeleteSP()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            var ins = new DynamicParameters();
            ins.Add("@refg_Nombre", "DIAG_REFG_" + Guid.NewGuid().ToString().Substring(0, 5));
            ins.Add("@refg_Ubicacion", "Test");
            ins.Add("@refg_RTN", "00000000000000");
            ins.Add("@refg_Telefono", "00000000");
            ins.Add("@refg_Correo", "test@test.com");
            ins.Add("@refg_InformacionAdicional", "Test");
            ins.Add("@depto_Id", 1, DbType.Int32);
            ins.Add("@mpio_Id", 1, DbType.Int32);
            ins.Add("@refg_EsActivo", true, DbType.Boolean);
            ins.Add("@refg_UsuarioCrea", 1, DbType.Int32);

            try
            {
                var insResult = await db.ExecuteAsync("[Refugio].[PR_Refugio_Refugios_Insert]", ins, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"INSERT result: {insResult}");
            }
            catch (Exception ex) { _output.WriteLine($"INSERT ERROR: {ex.Message}"); return; }

            var list = await db.QueryAsync("[Refugio].[PR_Refugio_Refugios_List]", commandType: CommandType.StoredProcedure);
            dynamic last = list.OrderByDescending(x => (int)x.refg_Id).FirstOrDefault(x => ((string)x.refg_Nombre).StartsWith("DIAG_REFG_"));
            if (last == null) { _output.WriteLine("No insert found"); return; }
            int id = (int)last.refg_Id;
            _output.WriteLine($"Inserted ID: {id}");

            try
            {
                var del = new DynamicParameters();
                del.Add("@refg_Id", id, DbType.Int32);
                var delResult = await db.ExecuteAsync("[General].[PR_General_Refugios_Delete]", del, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"DELETE result: {delResult}");
                Assert.True(delResult > 0 || delResult == -1, $"DELETE returned {delResult}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"DELETE ERROR: {ex.Message}");
                Assert.True(false, $"DELETE SP failed: {ex.Message}");
            }
        }

        [Fact]
        public async Task Diagnostico_Item_InsertSP()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            var cats = await db.QueryAsync("[Inventario].[PR_Inventario_Categorias_List]", commandType: CommandType.StoredProcedure);
            var firstCat = cats.FirstOrDefault();
            int catId = firstCat != null ? (int)firstCat.cat_Id : 1;
            _output.WriteLine($"Using cat_Id={catId}");

            var ins = new DynamicParameters();
            ins.Add("@itm_Codigo", "DIAG_" + Guid.NewGuid().ToString().Substring(0, 5));
            ins.Add("@itm_Descripcion", "DIAG_ITEM_" + Guid.NewGuid().ToString().Substring(0, 5));
            ins.Add("@cat_Id", catId, DbType.Int32);
            ins.Add("@itm_Precio", 10.00m, DbType.Decimal);
            ins.Add("@itm_UsuarioCrea", 1, DbType.Int32);

            try
            {
                var r = await db.ExecuteAsync("[Inventario].[PR_Inventario_Items_Insert]", ins, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"INSERT result: {r}");
                Assert.True(r > 0 || r == -1, $"INSERT failed with result={r}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"INSERT ERROR: {ex.Message}");
                Assert.True(false, $"SP failed: {ex.Message}");
            }
        }

        [Fact]
        public async Task Diagnostico_Evento_InsertSP()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            var refugios = await db.QueryAsync("[Refugio].[PR_Refugio_Refugios_List]", commandType: CommandType.StoredProcedure);
            var firstRefugio = refugios.FirstOrDefault();
            int refgId = firstRefugio != null ? (int)firstRefugio.refg_Id : 1;
            _output.WriteLine($"Using refg_Id={refgId}");

            var ins = new DynamicParameters();
            ins.Add("@eve_Descripcion", "DIAG_EVE_" + Guid.NewGuid().ToString().Substring(0, 5));
            ins.Add("@refg_Id", refgId, DbType.Int32);
            ins.Add("@eve_HoraInicio", TimeSpan.FromHours(8), DbType.Time);
            ins.Add("@eve_HoraFinal", TimeSpan.FromHours(10), DbType.Time);
            ins.Add("@eve_Fecha", DateTime.Today, DbType.DateTime);
            ins.Add("@eve_UsuarioCrea", 1, DbType.Int32);

            try
            {
                var r = await db.ExecuteAsync("[Refugio].[PR_Refugio_Eventos_Insert]", ins, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"INSERT result: {r}");
                Assert.True(r > 0 || r == -1, $"INSERT failed with result={r}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"INSERT ERROR: {ex.Message}");
                Assert.True(false, $"SP failed: {ex.Message}");
            }
        }

        [Fact]
        public async Task Diagnostico_Voluntario_InsertSP()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            var ins = new DynamicParameters();
            ins.Add("@vol_HorasTrabajadas", 10, DbType.Int32);
            ins.Add("@vol_Recurrente", true, DbType.Boolean);
            ins.Add("@per_Identidad", "0801" + Guid.NewGuid().ToString().Substring(0, 8));
            ins.Add("@per_PrimerNombre", "Juan");
            ins.Add("@per_SegundoNombre", "Test");
            ins.Add("@per_ApellidoPaterno", "Prueba");
            ins.Add("@per_ApellidoMaterno", "Test");
            ins.Add("@per_FechaNacimiento", new DateTime(1990, 1, 1), DbType.DateTime);
            ins.Add("@per_Domicilio", "Test Address");
            ins.Add("@per_Telefono", "99999999");
            ins.Add("@per_Correo", $"test{Guid.NewGuid().ToString().Substring(0, 4)}@test.com");
            ins.Add("@per_UsuarioCrea", 1, DbType.Int32);

            try
            {
                var r = await db.ExecuteAsync("[Refugio].[PR_Refugio_Voluntarios_Insert]", ins, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"INSERT result: {r}");
                Assert.True(r > 0 || r == -1, $"INSERT failed with result={r}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"INSERT ERROR: {ex.Message}");
                Assert.True(false, $"SP failed: {ex.Message}");
            }
        }

        [Fact]
        public async Task Diagnostico_RecepcionMercancia_InsertSP()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            // First get a valid refg_Id
            var refugios = await db.QueryAsync("[Refugio].[PR_Refugio_Refugios_List]", commandType: CommandType.StoredProcedure);
            var firstRefugio = refugios.FirstOrDefault();
            int refgId = firstRefugio != null ? (int)firstRefugio.refg_Id : 1;
            _output.WriteLine($"Using refg_Id={refgId}");

            var ins = new DynamicParameters();
            ins.Add("@recep_Descripcion", "DIAG_RECEP_" + Guid.NewGuid().ToString().Substring(0, 6));
            ins.Add("@recep_Fecha", DateTime.Today, DbType.DateTime);
            ins.Add("@refg_Id", refgId, DbType.Int32);
            ins.Add("@recep_TipoRecepcion", "Donacion");
            ins.Add("@recep_OrigenId", DBNull.Value, DbType.Int32);
            ins.Add("@recep_NumeroDocumento", "DOC001");
            ins.Add("@recep_UsuarioCrea", 1, DbType.Int32);

            try
            {
                var r = await db.ExecuteAsync("[Inventario].[PR_Inventario_RecepcionesMercancia_Insert]", ins, commandType: CommandType.StoredProcedure);
                _output.WriteLine($"INSERT result: {r}");
                Assert.True(r > 0 || r == -1, $"INSERT failed with result={r}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"INSERT ERROR: {ex.Message}");
                Assert.True(false, $"SP failed: {ex.Message}");
            }
        }

        [Fact]
        public async Task Diagnostico_RecepcionMercancia_ServiceAddThenDirectQuery()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            // Get a valid refg_Id
            var refugios = await db.QueryAsync("[Refugio].[PR_Refugio_Refugios_List]", commandType: CommandType.StoredProcedure);
            var firstRefugio = refugios.FirstOrDefault();
            int refgId = firstRefugio != null ? (int)firstRefugio.refg_Id : 1;
            _output.WriteLine($"Using refg_Id={refgId}");

            string uniqueDesc = "DIAG_SVC_" + Guid.NewGuid().ToString().Substring(0, 6);

            // Map manually (same as service)
            var config = new AutoMapper.MapperConfiguration(cfg =>
                cfg.AddProfile(new PetsHome.Business.Extensions.MappingProfileExtensions()));
            var mapper = config.CreateMapper();

            var model = new PetsHome.Business.Models.RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = uniqueDesc,
                recep_Fecha = DateTime.Today,
                refg_Id = refgId,
                recep_TipoRecepcion = "D",
                recep_NumeroDocumento = "DOC_DIAG"
            };

            var entity = mapper.Map<PetsHome.Common.Entities.tbRecepcionesMercancia>(model);
            _output.WriteLine($"Entity after mapping: recep_Descripcion={entity.recep_Descripcion}, refg_Id={entity.refg_Id}, UsuarioCrea={entity.recep_UsuarioCrea}");

            // Call repository directly
            var repo = new PetsHome.Logic.Repositories.RecepcionMercanciaRepository();
            var addRequestResult = await repo.AddAsync(entity);
            bool addResult = addRequestResult.Success;
            _output.WriteLine($"repo.AddAsync result: {addResult} (CodeStatus={addRequestResult.CodeStatus})");

            // Query DB directly
            var rawCheck = await db.QueryAsync("SELECT recep_Id, recep_Descripcion FROM [Inventario].[tbRecepcionesMercancia] WHERE recep_Descripcion = @desc AND recep_EsEliminado = 0",
                new { desc = uniqueDesc });
            var found = rawCheck.FirstOrDefault();
            _output.WriteLine($"Direct DB query result: {(found != null ? $"found id={found.recep_Id}" : "NOT FOUND")}");

            // Also check ListSP
            var listResult = await db.QueryAsync("[Inventario].[PR_Inventario_RecepcionesMercancia_List]", commandType: CommandType.StoredProcedure);
            var inList = listResult.FirstOrDefault(x => ((string)x.recep_Descripcion) == uniqueDesc);
            _output.WriteLine($"In list SP: {(inList != null ? "FOUND" : "NOT FOUND")}");

            // Also check including deleted records
            var rawCheckAll = await db.QueryAsync("SELECT recep_Id, recep_Descripcion, recep_EsEliminado FROM [Inventario].[tbRecepcionesMercancia] WHERE recep_Descripcion = @desc",
                new { desc = uniqueDesc });
            _output.WriteLine($"Including deleted: {(rawCheckAll.Any() ? $"found {rawCheckAll.Count()} records" : "NOT FOUND")}");

            // Check the SP return value directly to understand -1 behavior
            var spResult = await db.ExecuteAsync("[Inventario].[PR_Inventario_RecepcionesMercancia_Insert]",
                new {
                    recep_Descripcion = uniqueDesc + "_SPtest",
                    recep_Fecha = DateTime.Today,
                    refg_Id = refgId,
                    recep_TipoRecepcion = "D",
                    recep_OrigenId = (int?)null,
                    recep_NumeroDocumento = "DIAG_TEST",
                    recep_UsuarioCrea = 1
                }, commandType: CommandType.StoredProcedure);
            _output.WriteLine($"Direct SP ExecuteAsync result: {spResult}");

            var foundSp = await db.QueryAsync("SELECT recep_Id FROM [Inventario].[tbRecepcionesMercancia] WHERE recep_Descripcion = @desc",
                new { desc = uniqueDesc + "_SPtest" });
            _output.WriteLine($"Record after direct SP: {(foundSp.Any() ? "FOUND" : "NOT FOUND")}");

            Assert.True(addResult, $"repo.AddAsync returned false — SP may have failed");
            Assert.True(found != null, $"Record not in DB despite AddAsync={addResult}");
        }

        [Fact]
        public async Task Diagnostico_RecepcionMercancia_RawInsert()
        {
            // Test if a raw INSERT into the table works, bypassing the SP
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            string uniqueDesc = "DIAG_RAW_" + Guid.NewGuid().ToString().Substring(0, 6);

            try
            {
                var rawResult = await db.ExecuteAsync(
                    @"INSERT INTO [Inventario].[tbRecepcionesMercancia]
                      (recep_Descripcion, recep_Fecha, refg_Id, recep_EsEliminado, recep_UsuarioCrea, recep_FechaCrea, recep_TipoRecepcion, recep_OrigenId, recep_NumeroDocumento)
                      VALUES (@desc, @fecha, @refgId, 0, 1, GETDATE(), 'D', NULL, 'DOC_RAW_TEST')",
                    new { desc = uniqueDesc, fecha = DateTime.Today, refgId = 1 });
                _output.WriteLine($"Raw INSERT result: {rawResult}");

                var found = await db.QueryAsync("SELECT recep_Id FROM [Inventario].[tbRecepcionesMercancia] WHERE recep_Descripcion = @desc", new { desc = uniqueDesc });
                _output.WriteLine($"Raw INSERT record: {(found.Any() ? $"FOUND (id={found.First().recep_Id})" : "NOT FOUND")}");

                // Cleanup
                if (found.Any())
                    await db.ExecuteAsync("DELETE FROM [Inventario].[tbRecepcionesMercancia] WHERE recep_Descripcion = @desc", new { desc = uniqueDesc });

                Assert.True(rawResult > 0, $"Raw INSERT failed with result={rawResult}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Raw INSERT EXCEPTION: {ex.Message}");
                Assert.True(false, $"Raw INSERT threw exception: {ex.Message}");
            }
        }

        [Fact]
        public void Diagnostico_RecepcionMercancia_AutoMapperMapping()
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new PetsHome.Business.Extensions.MappingProfileExtensions());
            });
            var mapper = config.CreateMapper();

            var model = new PetsHome.Business.Models.RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = "Test Diagnostico AutoMapper",
                recep_Fecha = DateTime.Today,
                refg_Id = 1,
                recep_TipoRecepcion = "Donacion",
                recep_NumeroDocumento = "DOC001"
            };

            try
            {
                var entity = mapper.Map<PetsHome.Common.Entities.tbRecepcionesMercancia>(model);
                Assert.NotNull(entity);
                _output.WriteLine($"Mapping OK: recep_Descripcion={entity.recep_Descripcion}, refg_Id={entity.refg_Id}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"AUTOMAPPER ERROR: {ex.Message}");
                if (ex.InnerException != null)
                    _output.WriteLine($"INNER: {ex.InnerException.Message}");
                Assert.True(false, $"AutoMapper mapping RecepcionMercanciaFormViewModel -> tbRecepcionesMercancia failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Fuerza un error -5 en un SP para verificar que DbApp.ExecuteWithResult
        /// loguee el error en Serilog y reemplace el mensaje técnico.
        ///
        /// Paso previo: ejecutar en SSMS:
        /// CREATE OR ALTER PROCEDURE [Refugio].[PR_Test_Error]
        /// AS BEGIN SET NOCOUNT ON BEGIN TRY SELECT 1/0 END TRY
        /// BEGIN CATCH SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus END CATCH END
        /// </summary>
        [Fact]
        public async Task Diagnostico_DbApp_ErrorMenos5_SerilogLoguea()
        {
            // Arrange
            PetsHomeDbContext.BuildConnectionString(_cs);

            // Act - llamar SP que fuerza error -5
            var result = await DbApp.ExecuteWithResult("[Refugio].[PR_Test_Error]", new DynamicParameters());

            // Assert
            _output.WriteLine($"CodeStatus: {result.CodeStatus}");
            _output.WriteLine($"MessageStatus: {result.MessageStatus}");
            _output.WriteLine($"Success: {result.Success}");

            Assert.NotNull(result);
            Assert.Equal(-5, result.CodeStatus);
            Assert.False(result.Success);
            // El mensaje técnico debe estar oculto
            Assert.Equal("Ocurrió un error interno.", result.MessageStatus);
            _output.WriteLine("OK: Serilog debió registrar 'Divide by zero...' en el log");
        }

        [Fact]
        public async Task Diagnostico_Categoria_DetailSP()
        {
            using var db = new SqlConnection(_cs);
            await db.OpenAsync();

            var list = await db.QueryAsync("[Inventario].[PR_Inventario_Categorias_List]", commandType: CommandType.StoredProcedure);
            var first = list.FirstOrDefault();
            if (first == null) { _output.WriteLine("No categorias"); Assert.True(true); return; }
            int id = (int)first.cat_Id;
            _output.WriteLine($"Testing Detail with cat_Id={id}");

            try
            {
                var p = new DynamicParameters();
                p.Add("@cat_Id", id, DbType.Int32);
                var detail = await db.QueryFirstOrDefaultAsync("[Inventario].[PR_Inventario_Categorias_Detail]", p, commandType: CommandType.StoredProcedure);
                if (detail != null)
                {
                    _output.WriteLine("Detail returned data OK");
                    foreach (var prop in (System.Collections.Generic.IDictionary<string, object>)detail)
                        _output.WriteLine($"  Column: {prop.Key} = {prop.Value}");
                }
                else
                    _output.WriteLine("Detail returned NULL");
                Assert.NotNull(detail);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"DETAIL ERROR: {ex.Message}");
                Assert.True(false, $"SP failed: {ex.Message}");
            }
        }
    }
}
