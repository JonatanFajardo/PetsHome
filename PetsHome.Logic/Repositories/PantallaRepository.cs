using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class PantallaRepository
    {
        public async Task<IEnumerable<PR_Seguridad_Pantallas_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Pantallas_List]";
            return await DbApp.Select<PR_Seguridad_Pantallas_ListResult>(sqlQuery);
        }

        public async Task<IEnumerable<PR_Seguridad_Pantallas_ByRolResult>> ByRolAsync(int rolId)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Pantallas_ByRol]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Id", rolId, DbType.Int32);
            return await DbApp.SelectById<PR_Seguridad_Pantallas_ByRolResult>(sqlQuery, parameter);
        }

        public async Task<IEnumerable<PR_Seguridad_Pantallas_NombresByRolResult>> NombresByRolAsync(int rolId)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Pantallas_NombresByRol]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Id", rolId, DbType.Int32);
            return await DbApp.SelectById<PR_Seguridad_Pantallas_NombresByRolResult>(sqlQuery, parameter);
        }

        public async Task<bool> SaveByRolAsync(int rolId, string permisosJson)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_RolesPantallas_Save]";
            var parameter = new DynamicParameters();
            parameter.Add("@rol_Id", rolId, DbType.Int32);
            parameter.Add("@permisosJson", permisosJson, DbType.String);
            return await DbApp.Insert(sqlQuery, parameter);
        }
    }
}
