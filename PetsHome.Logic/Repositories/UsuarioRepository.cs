using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de usuarios y autenticación
    /// </summary>
    public class UsuarioRepository
    {
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
