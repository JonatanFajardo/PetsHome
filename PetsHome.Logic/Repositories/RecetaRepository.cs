using Dapper;
using PetsHome.Common;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class RecetaRepository : IGenericRepository<tbRecetas>
    {
        public async Task<IEnumerable<PR_Medico_Recetas_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_List]";
            return await DbApp.Select<PR_Medico_Recetas_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_Recetas_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@receta_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Recetas_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_Recetas_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@receta_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_Recetas_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbRecetas entity)
        {
            entity.receta_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Medicamento", entity.receta_Medicamento, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoMed_Id", entity.tipoMed_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@viaAdmin_Id", entity.viaAdmin_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Dosis", entity.receta_Dosis, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Frecuencia", entity.receta_Frecuencia, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Duracion", entity.receta_Duracion, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Instrucciones", entity.receta_Instrucciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_FechaInicio", entity.receta_FechaInicio, DbType.Date, ParameterDirection.Input);
            parameter.Add("@receta_FechaFin", entity.receta_FechaFin, DbType.Date, ParameterDirection.Input);
            parameter.Add("@receta_Estado", entity.receta_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_ResurtidosAutorizados", entity.receta_ResurtidosAutorizados, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_UsuarioCrea", entity.receta_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        /// <summary>
        /// Inserta una receta y devuelve el nuevo receta_Id (el SP retorna SCOPE_IDENTITY).
        /// A diferencia de AddAsync, respeta el receta_UsuarioCrea recibido (no lo fuerza a 1).
        /// </summary>
        public async Task<int> AddReturningIdAsync(tbRecetas entity)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Medicamento", entity.receta_Medicamento, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoMed_Id", entity.tipoMed_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@viaAdmin_Id", entity.viaAdmin_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Dosis", entity.receta_Dosis, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Frecuencia", entity.receta_Frecuencia, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Duracion", entity.receta_Duracion, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Instrucciones", entity.receta_Instrucciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_FechaInicio", entity.receta_FechaInicio, DbType.Date, ParameterDirection.Input);
            parameter.Add("@receta_FechaFin", entity.receta_FechaFin, DbType.Date, ParameterDirection.Input);
            parameter.Add("@receta_Estado", entity.receta_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_ResurtidosAutorizados", entity.receta_ResurtidosAutorizados, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_UsuarioCrea", entity.receta_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<int>(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbRecetas entity)
        {
            entity.receta_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@receta_Id", entity.receta_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Medicamento", entity.receta_Medicamento, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoMed_Id", entity.tipoMed_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@viaAdmin_Id", entity.viaAdmin_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Dosis", entity.receta_Dosis, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Frecuencia", entity.receta_Frecuencia, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Duracion", entity.receta_Duracion, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_Instrucciones", entity.receta_Instrucciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_FechaInicio", entity.receta_FechaInicio, DbType.Date, ParameterDirection.Input);
            parameter.Add("@receta_FechaFin", entity.receta_FechaFin, DbType.Date, ParameterDirection.Input);
            parameter.Add("@receta_Estado", entity.receta_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@receta_ResurtidosAutorizados", entity.receta_ResurtidosAutorizados, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_UsuarioModifica", entity.receta_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@receta_Id", id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        /// <summary>
        /// Lista plana del vademecum (farmaco + dosis por especie) para la
        /// calculadora de dosis. Filtrable por especie y categoria.
        /// </summary>
        public async Task<IEnumerable<PR_Medico_Vademecum_ListResult>> VademecumListAsync(string especie, string categoria)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Vademecum_List]";
            var parameter = new DynamicParameters();
            parameter.Add("@especie", especie, DbType.String, ParameterDirection.Input);
            parameter.Add("@categoria", categoria, DbType.String, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Vademecum_ListResult>(sqlQuery, parameter);
        }

        /// <summary>
        /// Registra un resurtido de la receta (incrementa el contador y agrega
        /// una fila a la bitacora). Devuelve estado + conteos.
        /// </summary>
        public async Task<PR_Medico_Recetas_ResurtirResult> ResurtirAsync(int recetaId, int usuario)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Recetas_Resurtir]";
            var parameter = new DynamicParameters();
            parameter.Add("@receta_Id", recetaId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Recetas_ResurtirResult>(sqlQuery, parameter);
        }

        public IEnumerable<PR_Medico_Recetas_DropdownResult> Dropdown(int? masc_Id = null)
        {
            const string query = "[Medico].[PR_Medico_Recetas_Dropdown]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
                var result = db.Query<PR_Medico_Recetas_DropdownResult>(query, parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
