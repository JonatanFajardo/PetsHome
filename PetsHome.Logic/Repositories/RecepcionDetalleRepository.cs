using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de detalles de recepciones de mercancía.
    /// </summary>
    public class RecepcionDetalleRepository
    {
        #region Consultas

        /// <summary>
        /// Obtiene la lista de detalles de una recepción específica.
        /// </summary>
        /// <param name="recepcionId">ID de la recepción.</param>
        /// <returns>Lista de detalles de la recepción.</returns>
        public async Task<IEnumerable<PR_Inventario_RecepcionesDetalles_ListResult>> ListByRecepcionAsync(int recepcionId)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_ByRecepcion]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", recepcionId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Inventario_RecepcionesDetalles_ListResult>(sqlQuery, parameter);
        }

        /// <summary>
        /// Busca un detalle de recepción por su ID.
        /// </summary>
        /// <param name="id">ID del detalle de recepción.</param>
        /// <returns>Datos del detalle encontrado.</returns>
        public async Task<PR_Inventario_RecepcionesDetalles_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@recdet_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_Inventario_RecepcionesDetalles_FindResult>(sqlQuery, parameter);
        }

        /// <summary>
        /// Inserta un nuevo detalle de recepción.
        /// </summary>
        /// <param name="entity">Entidad de detalle de recepción.</param>
        /// <returns>Verdadero si la operación fue exitosa.</returns>
        public async Task<RequestResult> AddAsync(tbRecepcionesDetalles entity)
        {
            entity.recdet_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", entity.recep_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recdet_Cantidad", entity.recdet_Cantidad, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@recdet_PrecioUnitario", entity.recdet_PrecioUnitario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@recdet_FechaVencimiento", entity.recdet_FechaVencimiento, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@recdet_NumeroLote", entity.recdet_NumeroLote, DbType.String, ParameterDirection.Input);
            parameter.Add("@recdet_UsuarioCrea", entity.recdet_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        /// <summary>
        /// Actualiza un detalle de recepción existente.
        /// </summary>
        /// <param name="entity">Entidad de detalle de recepción.</param>
        /// <returns>Verdadero si la operación fue exitosa.</returns>
        public async Task<RequestResult> EditAsync(tbRecepcionesDetalles entity)
        {
            entity.recdet_UsuarioModifica = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@recdet_Id", entity.recdet_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_Id", entity.recep_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recdet_Cantidad", entity.recdet_Cantidad, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@recdet_PrecioUnitario", entity.recdet_PrecioUnitario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@recdet_FechaVencimiento", entity.recdet_FechaVencimiento, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@recdet_NumeroLote", entity.recdet_NumeroLote, DbType.String, ParameterDirection.Input);
            parameter.Add("@recdet_UsuarioModifica", entity.recdet_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        /// <summary>
        /// Elimina (lógicamente) un detalle de recepción.
        /// </summary>
        /// <param name="id">ID del detalle de recepción.</param>
        /// <returns>Verdadero si la operación fue exitosa.</returns>
        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@recdet_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        #endregion Consultas
    }
}
