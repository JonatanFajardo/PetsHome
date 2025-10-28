using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de recepciones de mercancía.
    /// </summary>
    public class RecepcionMercanciaRepository
    {
        #region Consultas

        /// <summary>
        /// Obtiene la lista de todas las recepciones de mercancía.
        /// </summary>
        /// <returns>Lista de recepciones de mercancía.</returns>
        public async Task<IEnumerable<PR_Inventario_RecepcionesMercancia_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_List]";
            return await DbApp.Select<PR_Inventario_RecepcionesMercancia_ListResult>(sqlQuery);
        }

        /// <summary>
        /// Busca una recepción de mercancía por su ID.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>Datos de la recepción encontrada.</returns>
        public async Task<PR_Inventario_RecepcionesMercancia_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_Inventario_RecepcionesMercancia_FindResult>(sqlQuery, parameter);
        }

        /// <summary>
        /// Obtiene el detalle completo de una recepción de mercancía.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>Detalle de la recepción.</returns>
        public async Task<PR_Inventario_RecepcionesMercancia_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Inventario_RecepcionesMercancia_DetailResult>(sqlQuery, parameter);
        }

        /// <summary>
        /// Inserta una nueva recepción de mercancía.
        /// </summary>
        /// <param name="entity">Entidad de recepción de mercancía.</param>
        /// <returns>Verdadero si la operación fue exitosa.</returns>
        public async Task<Boolean> AddAsync(tbRecepcionesMercancia entity)
        {
            entity.recep_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Descripcion", entity.recep_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_Fecha", entity.recep_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_TipoRecepcion", entity.recep_TipoRecepcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_OrigenId", entity.recep_OrigenId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_NumeroDocumento", entity.recep_NumeroDocumento, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_UsuarioCrea", entity.recep_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        /// <summary>
        /// Actualiza una recepción de mercancía existente.
        /// </summary>
        /// <param name="entity">Entidad de recepción de mercancía.</param>
        /// <returns>Verdadero si la operación fue exitosa.</returns>
        public async Task<Boolean> EditAsync(tbRecepcionesMercancia entity)
        {
            entity.recep_UsuarioModifica = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", entity.recep_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_Descripcion", entity.recep_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_Fecha", entity.recep_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_TipoRecepcion", entity.recep_TipoRecepcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_OrigenId", entity.recep_OrigenId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_NumeroDocumento", entity.recep_NumeroDocumento, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_UsuarioModifica", entity.recep_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        /// <summary>
        /// Elimina (lógicamente) una recepción de mercancía.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>Verdadero si la operación fue exitosa.</returns>
        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        #endregion Consultas
    }
}
