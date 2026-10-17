using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio del motor de modulos configurables y la configuracion global.
    /// </summary>
    public class ConfiguracionRepository
    {
        public async Task<IEnumerable<PR_Seguridad_ModulosConfig_ListResult>> ListModulosAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_ModulosConfig_List]";
            return await DbApp.Select<PR_Seguridad_ModulosConfig_ListResult>(sqlQuery);
        }

        public async Task<IEnumerable<PR_Seguridad_ModulosConfig_ActivosResult>> ModulosActivosAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_ModulosConfig_Activos]";
            return await DbApp.Select<PR_Seguridad_ModulosConfig_ActivosResult>(sqlQuery);
        }

        public async Task<bool> SetModuloActivoAsync(string codigo, bool activo)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_ModulosConfig_SetActivo]";
            var parameter = new DynamicParameters();
            parameter.Add("@modu_Codigo", codigo, DbType.String);
            parameter.Add("@modu_EsActivo", activo, DbType.Boolean);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<PR_Seguridad_Configuracion_GetResult> GetConfiguracionAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Configuracion_Get]";
            return await DbApp.Find<PR_Seguridad_Configuracion_GetResult>(sqlQuery, new DynamicParameters());
        }

        public async Task<bool> CompletarWizardAsync(string perfil, string nombreOrg, int usuarioConfig)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Configuracion_CompletarWizard]";
            var parameter = new DynamicParameters();
            parameter.Add("@cfg_PerfilPrincipal", perfil, DbType.String);
            parameter.Add("@cfg_NombreOrganizacion", nombreOrg, DbType.String);
            parameter.Add("@cfg_UsuarioConfig", usuarioConfig, DbType.Int32);
            return await DbApp.Update(sqlQuery, parameter);
        }
    }
}
