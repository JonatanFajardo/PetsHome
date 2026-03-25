using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class RolRepository
    {
        public async Task<IEnumerable<PR_Seguridad_Roles_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Roles_List]";
            return await DbApp.Select<PR_Seguridad_Roles_ListResult>(sqlQuery);
        }

        public async Task<PR_Seguridad_Roles_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Roles_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Id", id, DbType.Int32);
            return await DbApp.Find<PR_Seguridad_Roles_FindResult>(sqlQuery, parameter);
        }

        public async Task<bool> InsertAsync(string descripcion, bool estado, int usuarioCrea)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Roles_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Descripcion", descripcion, DbType.String);
            parameter.Add("@rol_Estado", estado, DbType.Boolean);
            parameter.Add("@rol_UsuarioCrea", usuarioCrea, DbType.Int32);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<bool> UpdateAsync(int id, string descripcion, bool estado, int usuarioModifica)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Roles_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Id", id, DbType.Int32);
            parameter.Add("@rol_Descripcion", descripcion, DbType.String);
            parameter.Add("@rol_Estado", estado, DbType.Boolean);
            parameter.Add("@rol_UsuarioModifica", usuarioModifica, DbType.Int32);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Roles_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Id", id, DbType.Int32);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        public async Task<PR_Seguridad_Roles_ExistResult> ExistAsync(string descripcion)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Roles_Exist]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Descripcion", descripcion, DbType.String);
            return await DbApp.Find<PR_Seguridad_Roles_ExistResult>(sqlQuery, parameter);
        }

        public async Task<IEnumerable<PR_Seguridad_Roles_DropdownResult>> DropdownAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Roles_Dropdown]";
            return await DbApp.Select<PR_Seguridad_Roles_DropdownResult>(sqlQuery);
        }
    }
}
