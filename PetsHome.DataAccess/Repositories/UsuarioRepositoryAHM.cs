using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PetsHome.DataAccess.Repositories
{
    /// <summary>
    /// Repository compatible con el estilo AHM_INSTA_HELP_ADM
    /// Mantiene la misma interfaz y métodos que el UsuarioRepository de AHM
    /// </summary>
    public class UsuarioRepositoryAHM
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioRepositoryAHM(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Login compatible con AHM - retorna un objeto de usuario simple
        /// </summary>
        public UsuarioLoginResult Login(string contrasena, string usu_NombreUsuario)
        {
            const string SqlQuery = "UDP_Acce_tbUsuarios_Login";
            var result = new UsuarioLoginResult();
            var parameters = new DynamicParameters();
            parameters.Add("@usu_NombreUsuario", usu_NombreUsuario, DbType.String, ParameterDirection.Input);
            parameters.Add("@contrasena", contrasena, DbType.String, ParameterDirection.Input);

            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                result = db.QueryFirstOrDefault<UsuarioLoginResult>(SqlQuery, parameters, commandType: CommandType.StoredProcedure);
            }

            return result;
        }

        /// <summary>
        /// Marcar usuario como logueado (compatible con AHM)
        /// </summary>
        public int UsuarioLogIn(int usu_Id)
        {
            int resultado = 0;
            const string sqlQuery = @"UDP_Acce_tbUsuarios_LoginIn";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_Id", usu_Id, DbType.Int32, ParameterDirection.Input);
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        resultado = db.ExecuteScalar<int>(sqlQuery, parameter, transaction, commandType: CommandType.StoredProcedure);
                        if (resultado == 0)
                        {
                            // Éxito
                        }
                        else
                        {
                            goto errorTransaction;
                        }
                        transaction.Commit();
                        db.Close();
                        db.Dispose();
                        return 1;

                    errorTransaction:
                        transaction.Rollback();
                        db.Close();
                        db.Dispose();
                        return -1;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }
        }

        /// <summary>
        /// Marcar usuario como deslogueado (compatible con AHM)
        /// </summary>
        public int UsuarioLogOut(int usu_Id)
        {
            int resultado = 0;
            const string sqlQuery = @"UDP_Acce_tbUsuarios_Logout";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_Id", usu_Id, DbType.Int32, ParameterDirection.Input);
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        resultado = db.ExecuteScalar<int>(sqlQuery, parameter, transaction, commandType: CommandType.StoredProcedure);
                        if (resultado == 0)
                        {
                            // Éxito
                        }
                        else
                        {
                            goto errorTransaction;
                        }
                        transaction.Commit();
                        db.Close();
                        db.Dispose();
                        return 1;

                    errorTransaction:
                        transaction.Rollback();
                        db.Close();
                        db.Dispose();
                        return -1;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }
        }

        /// <summary>
        /// Obtener detalle de usuario por ID
        /// </summary>
        public UsuarioLoginResult FindDetalle(int? id)
        {
            const string query = @"UDP_Acce_tbUsuarios_FindDetalle";
            var parameters = new DynamicParameters();
            parameters.Add("@usu_Id", id, DbType.Int32, ParameterDirection.Input);

            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var resultado = db.QueryFirstOrDefault<UsuarioLoginResult>(query, parameters, commandType: CommandType.StoredProcedure);
                return resultado;
            }
        }

        /// <summary>
        /// Validación de nombre de usuario
        /// </summary>
        public UsuarioLoginResult NameValidation(string Name)
        {
            const string query = @"UDP_Acce_tbUsuarios_NameValidation";
            var parameters = new DynamicParameters();
            parameters.Add("@usu_UsuarioNombre", Name, DbType.String, ParameterDirection.Input);

            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var resultado = db.QueryFirstOrDefault<UsuarioLoginResult>(query, parameters, commandType: CommandType.StoredProcedure);
                return resultado;
            }
        }

        /// <summary>
        /// Obtener lista de pantallas por usuario
        /// </summary>
        public IEnumerable<PantallaResult> GetPantallasPorUsuario(int usu_Id)
        {
            const string query = @"UDP_Acce_PantallasXUsuario";
            var parameters = new DynamicParameters();
            parameters.Add("@usu_Id", usu_Id, DbType.Int32, ParameterDirection.Input);

            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var resultado = db.Query<PantallaResult>(query, parameters, commandType: CommandType.StoredProcedure).ToList();
                return resultado;
            }
        }

        /// <summary>
        /// Obtener string de pantallas para sesión (compatible con AHM)
        /// </summary>
        public string GetPantallasStringPorUsuario(int usu_Id)
        {
            const string query = @"SELECT [Seguridad].[FN_GetPantallasStringPorUsuario](@usu_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("@usu_Id", usu_Id, DbType.Int32, ParameterDirection.Input);

            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var resultado = db.QueryFirstOrDefault<string>(query, parameters);
                return resultado ?? "";
            }
        }
    }

    /// <summary>
    /// Resultado del login compatible con AHM
    /// </summary>
    public class UsuarioLoginResult
    {
        public int usu_Id { get; set; }
        public int Emp_Id { get; set; }
        public string usu_NombreUsuario { get; set; }
        public string usu_NombreCompleto { get; set; }
        public string per_PrimerNombre { get; set; }
        public string per_ApellidoPaterno { get; set; }
        public int rol_Id { get; set; }
        public string rol_Descripcion { get; set; }
        public bool usu_Estado { get; set; }
        public string usu_ImagenPerfil { get; set; }
        public bool? usu_Logueado { get; set; }
    }

    /// <summary>
    /// Resultado de pantallas compatible con AHM
    /// </summary>
    public class PantallaResult
    {
        public int modpt_Id { get; set; }
        public string modpt_Descripcion { get; set; }
        public string modpt_Url { get; set; }
        public string modpt_Icono { get; set; }
        public int? modpt_Orden { get; set; }
        public int mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public int? Mod_Orden { get; set; }
    }
}