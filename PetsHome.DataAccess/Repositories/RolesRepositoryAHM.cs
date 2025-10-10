using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PetsHome.DataAccess.Repositories
{
    /// <summary>
    /// Repository de roles compatible con el estilo AHM_INSTA_HELP_ADM
    /// </summary>
    public class RolesRepositoryAHM
    {
        /// <summary>
        /// Obtener pantallas por rol (compatible con AHM)
        /// </summary>
        public IEnumerable<PantallaResult> ListPantallas(int rolid)
        {
            const string query = @"UDP_Acce_PantallasXRol";
            var parameters = new DynamicParameters();
            parameters.Add("@rol_Id", rolid, DbType.Int32, ParameterDirection.Input);

            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var resultado = db.Query<PantallaResult>(query, parameters, commandType: CommandType.StoredProcedure).ToList();
                return resultado;
            }
        }

        /// <summary>
        /// Obtener string de pantallas para sesión por rol (compatible con AHM)
        /// </summary>
        public string GetPantallasStringPorRol(int rol_Id)
        {
            const string query = @"SELECT [Seguridad].[FN_GetPantallasStringPorRol](@rol_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("@rol_Id", rol_Id, DbType.Int32, ParameterDirection.Input);

            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var resultado = db.QueryFirstOrDefault<string>(query, parameters);
                return resultado ?? "";
            }
        }

        /// <summary>
        /// Listar roles para usuarios
        /// </summary>
        public IEnumerable<RolResult> ListforUsers()
        {
            const string sqlQuery = "PR_Seguridad_Roles_List";
            var parameters = new DynamicParameters();
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var resultado = db.Query<RolResult>(sqlQuery, parameters, commandType: CommandType.StoredProcedure).ToList();
                return resultado;
            }
        }

        /// <summary>
        /// Buscar rol por ID
        /// </summary>
        public RolResult Find(int rol_Id)
        {
            const string sqlQuery = "PR_Seguridad_Roles_Detail";
            var parameters = new DynamicParameters();
            parameters.Add("@rol_Id", rol_Id, DbType.Int32, ParameterDirection.Input);
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.QueryFirstOrDefault<RolResult>(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }

    /// <summary>
    /// Resultado de rol compatible con AHM
    /// </summary>
    public class RolResult
    {
        public int Rol_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public bool Rol_EsActivo { get; set; }
        public DateTime Rol_FechaCreacion { get; set; }
    }
}