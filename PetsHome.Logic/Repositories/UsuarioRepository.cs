using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de usuarios y autenticación
    /// </summary>
    public class UsuarioRepository
    {
        #region CRUD

        public async Task<IEnumerable<PR_Seguridad_Usuarios_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Usuarios_List]";
            return await DbApp.Select<PR_Seguridad_Usuarios_ListResult>(sqlQuery);
        }

        public async Task<PR_Seguridad_Usuarios_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Usuarios_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_Id", id, DbType.Int32);
            return await DbApp.Find<PR_Seguridad_Usuarios_FindResult>(sqlQuery, parameter);
        }

        public async Task<bool> InsertAsync(string nombre, int empId, int rolId, int conId, bool esActivo)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Usuarios_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@Usu_Nombre", nombre, DbType.String);
            parameter.Add("@Emp_Id", empId, DbType.Int32);
            parameter.Add("@Rol_Id", rolId, DbType.Int32);
            parameter.Add("@Con_Id", conId, DbType.Int32);
            parameter.Add("@Usu_EsActivo", esActivo, DbType.Boolean);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<bool> UpdateAsync(int id, string nombre, int empId, int rolId, bool esActivo)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Usuarios_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_Id", id, DbType.Int32);
            parameter.Add("@Usu_Nombre", nombre, DbType.String);
            parameter.Add("@Emp_Id", empId, DbType.Int32);
            parameter.Add("@Rol_Id", rolId, DbType.Int32);
            parameter.Add("@Usu_EsActivo", esActivo, DbType.Boolean);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Usuarios_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_Id", id, DbType.Int32);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        public async Task<PR_Seguridad_Usuarios_ExistResult> ExistAsync(string nombre)
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Usuarios_Exist]";
            var parameter = new DynamicParameters();
            parameter.Add("@Usu_Nombre", nombre, DbType.String);
            return await DbApp.Find<PR_Seguridad_Usuarios_ExistResult>(sqlQuery, parameter);
        }

        #endregion

        #region Login y Autenticación

        /// <summary>
        /// Valida las credenciales del usuario y retorna información del usuario si son válidas
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="passwordHash">Hash de la contraseña</param>
        /// <returns>Información del usuario autenticado o null si las credenciales son inválidas</returns>
        public async Task<PR_Seguridad_Usuarios_LoginResult> LoginAsync(string username, string passwordHash)
        {
            const string sqlQuery = "[Seguridad].[UDP_Acce_tbUsuarios_Login]";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_NombreUsuario", username, DbType.String, ParameterDirection.Input);
            parameter.Add("@contrasena", passwordHash, DbType.String, ParameterDirection.Input);

            return await DbApp.Find<PR_Seguridad_Usuarios_LoginResult>(sqlQuery, parameter);
        }

        /// <summary>
        /// Marca al usuario como logueado y actualiza la información de último acceso
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Resultado de la operación (0 = éxito)</returns>
        public async Task<int> LoginInAsync(int userId)
        {
            const string sqlQuery = "[Seguridad].[UDP_Acce_tbUsuarios_LoginIn]";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_Id", userId, DbType.Int32, ParameterDirection.Input);

            var result = await DbApp.Find<PR_Seguridad_Usuarios_LoginInResult>(sqlQuery, parameter);
            return result?.Resultado ?? -1;
        }

        /// <summary>
        /// Marca al usuario como deslogueado
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Resultado de la operación (0 = éxito)</returns>
        public async Task<int> LogoutAsync(int userId)
        {
            const string sqlQuery = "[Seguridad].[UDP_Acce_tbUsuarios_Logout]";
            var parameter = new DynamicParameters();
            parameter.Add("@usu_Id", userId, DbType.Int32, ParameterDirection.Input);

            var result = await DbApp.Find<PR_Seguridad_Usuarios_LogoutResult>(sqlQuery, parameter);
            return result?.Resultado ?? -1;
        }

        #endregion
    }
}
