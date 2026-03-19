using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class RazaRepository
    {
        public virtual async Task<IEnumerable<PR_Refugio_Razas_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_List]";
            return await DbApp.Select<PR_Refugio_Razas_ListResult>(sqlQuery);
        }

        public virtual async Task<PR_Refugio_Razas_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Razas_FindResult>(sqlQuery, parameter);
        }

        public virtual async Task<PR_Refugio_Razas_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Razas_DetailResult>(sqlQuery, parameter);
        }

        public virtual async Task<RequestResult> AddAsync(tbRazas entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Descripcion", entity.raza_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_Tamano", entity.raza_Tamano, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_TipoAnimal", entity.raza_TipoAnimal, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_TipoPelaje", entity.raza_TipoPelaje, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_ImagenUrl", entity.raza_ImagenUrl, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_EsActivo", entity.raza_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@raza_UsuarioCrea", entity.raza_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public virtual async Task<RequestResult> EditAsync(tbRazas entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", entity.raza_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@raza_Descripcion", entity.raza_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_Tamano", entity.raza_Tamano, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_TipoAnimal", entity.raza_TipoAnimal, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_TipoPelaje", entity.raza_TipoPelaje, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_ImagenUrl", entity.raza_ImagenUrl, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_EsActivo", entity.raza_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@raza_UsuarioModifica", entity.raza_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public virtual async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public virtual async Task<bool> DescripcionExistsAsync(string descripcion, int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Existe]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Descripcion", descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<bool>(sqlQuery, parameter);
        }
    }
}