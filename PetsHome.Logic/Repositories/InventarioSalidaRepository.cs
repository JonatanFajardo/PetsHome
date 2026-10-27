using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio para la salida de inventario generada al Finalizar una consulta
    /// (descuento de stock de los medicamentos entregados) y las lecturas de apoyo
    /// (existencia por item/refugio y resolucion del refugio de la mascota).
    ///
    /// NOTA IMPORTANTE: [Inventario].[PR_Inventario_SalidasDetalles_Insert] ya valida
    /// la existencia, descuenta exi_Cantidad y registra el movimiento en una transaccion.
    /// Por eso NO se invoca [Inventario].[PR_Inventario_ProcesarSalidaInventario]: hacerlo
    /// descontaria el stock por segunda vez (doble descuento).
    /// </summary>
    public class InventarioSalidaRepository
    {
        /// <summary>
        /// Devuelve el refugio (y nombre) de la mascota. refg_Id = 0 si no se encuentra.
        /// </summary>
        public async Task<PR_Refugio_Mascota_RefugioIdResult> RefugioPorMascotaAsync(int mascId)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Mascota_RefugioId]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_Refugio_Mascota_RefugioIdResult>(sqlQuery, p);
        }

        /// <summary>
        /// Refugio del usuario que atiende (via su empleado). 0 si no tiene.
        /// Se usa como ubicacion del stock cuando la mascota no tiene refugio propio
        /// (paciente de cliente externo): el medicamento sale del sitio donde atiende el vet.
        /// </summary>
        public async Task<int> RefugioPorUsuarioAsync(int usuId)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Refugio_PorUsuario]";
            var p = new DynamicParameters();
            p.Add("@usu_Id", usuId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<int>(sqlQuery, p);
        }

        /// <summary>
        /// Existencia disponible (exi_Cantidad) de un item en un refugio. 0 si no hay fila.
        /// </summary>
        public async Task<int> ExistenciaPorItemAsync(int itmId, int refgId)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencia_PorItem]";
            var p = new DynamicParameters();
            p.Add("@itm_Id", itmId, DbType.Int32, ParameterDirection.Input);
            p.Add("@refg_Id", refgId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<int>(sqlQuery, p);
        }

        /// <summary>
        /// Crea el header de la salida (tbSalidas) y devuelve el sal_Id generado. 0 si falla.
        /// </summary>
        public async Task<int> InsertSalidaAsync(string descripcion, char tipoSalida, int refgId, int usuarioCrea)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Salidas_Insert]";
            var p = new DynamicParameters();
            p.Add("@sal_Descripcion", descripcion, DbType.String, ParameterDirection.Input);
            p.Add("@sal_TipoSalida", tipoSalida.ToString(), DbType.StringFixedLength, ParameterDirection.Input, 1);
            p.Add("@refg_Id", refgId, DbType.Int32, ParameterDirection.Input);
            p.Add("@sal_Fecha", DateTime.Now, DbType.DateTime, ParameterDirection.Input);
            p.Add("@sal_UsuarioCrea", usuarioCrea, DbType.Int32, ParameterDirection.Input);

            // El SP devuelve SELECT SCOPE_IDENTITY() AS sal_Id (numeric) -> leer como decimal y castear.
            var salId = await DbApp.Select<decimal>(sqlQuery, p);
            return (int)salId;
        }

        /// <summary>
        /// Crea el detalle de la salida (tbSalidasDetalles) para la consulta.
        /// Usa PR_Inventario_Consulta_SalidaDetalle_Insert, que inserta SOLO la fila:
        /// los triggers de la tabla validan existencia, descuentan exi_Cantidad y
        /// registran el movimiento UNA vez. (No se usa PR_Inventario_SalidasDetalles_Insert
        /// porque ese SP descuenta por su cuenta y, sumado a los triggers, duplicaba el stock.)
        /// </summary>
        public async Task<bool> InsertSalidaDetalleAsync(int salId, int itmId, int cantidad, string observaciones, int usuarioCrea, int? saldetRecetaId = null)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Consulta_SalidaDetalle_Insert]";
            var p = new DynamicParameters();
            p.Add("@sal_Id", salId, DbType.Int32, ParameterDirection.Input);
            p.Add("@itm_Id", itmId, DbType.Int32, ParameterDirection.Input);
            p.Add("@saldet_Cantidad", cantidad, DbType.Int32, ParameterDirection.Input);
            p.Add("@saldet_Observaciones", observaciones, DbType.String, ParameterDirection.Input);
            p.Add("@saldet_UsuarioCrea", usuarioCrea, DbType.Int32, ParameterDirection.Input);
            // Trazabilidad receta->salida: se pasa solo si la linea dispensada genero receta.
            // El SP lo acepta opcional (default NULL); si es null la columna queda sin ligar.
            p.Add("@saldet_RecetaId", saldetRecetaId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, p);
        }
    }
}
