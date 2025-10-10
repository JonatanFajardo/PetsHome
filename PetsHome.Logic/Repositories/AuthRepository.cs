using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces.Especific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class AuthRepository : IAuthRepository
    {

        public async Task<PR_Seguridad_Usuarios_LoginResult> LoginAsync(string usuario, string hashContrasena)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Usu_Nombre", usuario);
                parameters.Add("@Con_Hash", hashContrasena);

                return await DbApp.Select<PR_Seguridad_Usuarios_LoginResult>("Seguridad.PR_Seguridad_Usuarios_Login", parameters);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<PR_Seguridad_Usuarios_DetailResult> GetUsuarioDetailAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                return await DbApp.Select<PR_Seguridad_Usuarios_DetailResult>("PR_Seguridad_Usuarios_Detail", parameters);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PR_Seguridad_Roles_ListResult>> GetRolesAsync()
        {
            try
            {
                var result = await DbApp.Select<PR_Seguridad_Roles_ListResult>("PR_Seguridad_Roles_List");
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_Roles_ListResult>();
            }
        }

        public async Task<int> CreateContrasenaAsync(string hash, string salt, int usuarioCreacion)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Con_Hash", hash);
                parameters.Add("@Con_Salt", salt);
                parameters.Add("@Usu_UsuarioCreacion", usuarioCreacion);

                var result = await DbApp.Insert("PR_Seguridad_Contrasenas_Insert", parameters);
                return result ? 1 : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> CreateUsuarioAsync(int empId, string usuario, string passwordHash, int rolId, string ip, int usuarioCreacion)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Emp_Id", empId);
                parameters.Add("@Usu_Nombre", usuario);
                parameters.Add("@Usu_PasswordHash", passwordHash);
                parameters.Add("@Rol_Id", rolId);
                parameters.Add("@Usu_Ip", ip ?? "");
                parameters.Add("@Usu_UsuarioCreacion", usuarioCreacion);

                var result = await DbApp.Insert("PR_Seguridad_Usuarios_Insert", parameters);
                return result ? 1 : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdateLastAccessAsync(int usuarioId, string ip)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);
                parameters.Add("@Usu_Ip", ip ?? "");

                return await DbApp.Update("PR_Seguridad_Usuarios_UpdateLastAccess", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int usuarioId, string passwordHash)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);
                parameters.Add("@Usu_PasswordHash", passwordHash);

                return await DbApp.Update("PR_Seguridad_Usuarios_ChangePassword", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UsuarioExistsAsync(string usuario)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Usu_Nombre", usuario);

                var result = await DbApp.Select<int>("PR_Seguridad_Usuarios_Exists", parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EmpleadoTieneUsuarioAsync(int empId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Emp_Id", empId);

                var result = await DbApp.Select<int>("PR_Seguridad_Empleados_TieneUsuario", parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PR_Seguridad_Usuarios_GetPermissionsResult>> GetUsuarioPermissionsAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                var result = await DbApp.SelectById<PR_Seguridad_Usuarios_GetPermissionsResult>("PR_Seguridad_Usuarios_GetPermissions", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_Usuarios_GetPermissionsResult>();
            }
        }

        public async Task<List<PR_Seguridad_Roles_GetPermissionsResult>> GetRolePermissionsAsync(int rolId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Rol_Id", rolId);

                var result = await DbApp.SelectById<PR_Seguridad_Roles_GetPermissionsResult>("PR_Seguridad_Roles_GetPermissions", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_Roles_GetPermissionsResult>();
            }
        }

        public async Task<bool> CheckPermissionAsync(int usuarioId, string modulo, string permiso)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);
                parameters.Add("@Mod_Nombre", modulo);
                parameters.Add("@Per_Nombre", permiso);

                var result = await DbApp.Select<PR_Seguridad_CheckPermissionResult>("PR_Seguridad_CheckPermission", parameters);

                return result?.TienePermiso > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PR_Seguridad_PantallasPorUsuario_ListResult>> GetPantallasUsuarioAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                var result = await DbApp.SelectById<PR_Seguridad_PantallasPorUsuario_ListResult>("Seguridad.PR_Seguridad_PantallasPorUsuario", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_PantallasPorUsuario_ListResult>();
            }
        }

        public async Task<List<PR_Seguridad_GetUserPermissionsResult>> GetUserPermissionsAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                var result = await DbApp.SelectById<PR_Seguridad_GetUserPermissionsResult>("PR_Seguridad_GetUserPermissions", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_GetUserPermissionsResult>();
            }
        }

        public async Task<List<PR_Seguridad_GetUserPantallasResult>> GetUserPantallasAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                var result = await DbApp.SelectById<PR_Seguridad_GetUserPantallasResult>("PR_Seguridad_GetUserPantallas", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_GetUserPantallasResult>();
            }
        }

        // Nuevos métodos para el sistema de seguridad mejorado

        public async Task<(PR_Seguridad_Usuarios_Login_V2Result usuario, List<PR_Seguridad_Usuarios_Login_V2RolesResult> roles)> LoginV2Async(string usuario, string hashContrasena, string userAgent = null, string direccionIP = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Usu_Nombre", usuario);
                parameters.Add("@Con_Hash", hashContrasena);
                parameters.Add("@UserAgent", userAgent);
                parameters.Add("@DireccionIP", direccionIP);

                return await DbApp.ExecuteMultipleAsync("Seguridad.PR_Seguridad_Usuarios_Login_V2", parameters, async (multi) =>
                {
                    var usuarioResult = await multi.ReadFirstOrDefaultAsync<PR_Seguridad_Usuarios_Login_V2Result>();
                    var rolesResult = await multi.ReadAsync<PR_Seguridad_Usuarios_Login_V2RolesResult>();

                    return (usuarioResult, rolesResult.ToList());
                });
            }
            catch (Exception ex)
            {
                return (null, new List<PR_Seguridad_Usuarios_Login_V2RolesResult>());
            }
        }

        public async Task<bool> LogoutV2Async(int usuarioId, string userAgent = null, string direccionIP = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);
                parameters.Add("@UserAgent", userAgent);
                parameters.Add("@DireccionIP", direccionIP);

                var result = await DbApp.Select<PR_Seguridad_Usuarios_LogoutResult>("Seguridad.PR_Seguridad_Usuarios_Logout", parameters);
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(List<PR_Seguridad_PantallasPorRol_ComponentesResult> componentes, 
                          List<PR_Seguridad_PantallasPorRol_ModulosResult> modulos, 
                          List<PR_Seguridad_PantallasPorRol_PantallasResult> pantallas)> GetPantallasPorRolAsync(int rolId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@rol_Id", rolId);

                return await DbApp.ExecuteMultipleAsync("Seguridad.PR_Seguridad_PantallasPorRol", parameters, async (multi) =>
                {
                    var componentes = await multi.ReadAsync<PR_Seguridad_PantallasPorRol_ComponentesResult>();
                    var modulos = await multi.ReadAsync<PR_Seguridad_PantallasPorRol_ModulosResult>();
                    var pantallas = await multi.ReadAsync<PR_Seguridad_PantallasPorRol_PantallasResult>();

                    return (componentes.ToList(), modulos.ToList(), pantallas.ToList());
                });
            }
            catch
            {
                return (new List<PR_Seguridad_PantallasPorRol_ComponentesResult>(),
                        new List<PR_Seguridad_PantallasPorRol_ModulosResult>(),
                        new List<PR_Seguridad_PantallasPorRol_PantallasResult>());
            }
        }

        public async Task<(List<PR_Seguridad_PantallasPorRol_ComponentesResult> componentes, 
                          List<PR_Seguridad_PantallasPorRol_ModulosResult> modulos, 
                          List<PR_Seguridad_PantallasPorRol_PantallasResult> pantallas)> GetPantallasPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                return await DbApp.ExecuteMultipleAsync("Seguridad.PR_Seguridad_PantallasPorUsuario", parameters, async (multi) =>
                {
                    var componentes = await multi.ReadAsync<PR_Seguridad_PantallasPorRol_ComponentesResult>();
                    var modulos = await multi.ReadAsync<PR_Seguridad_PantallasPorRol_ModulosResult>();
                    var pantallas = await multi.ReadAsync<PR_Seguridad_PantallasPorRol_PantallasResult>();

                    return (componentes.ToList(), modulos.ToList(), pantallas.ToList());
                });
            }
            catch
            {
                return (new List<PR_Seguridad_PantallasPorRol_ComponentesResult>(),
                        new List<PR_Seguridad_PantallasPorRol_ModulosResult>(),
                        new List<PR_Seguridad_PantallasPorRol_PantallasResult>());
            }
        }

        public async Task<bool> AsignarRolUsuarioAsync(int rolId, int usuId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@rol_Id", rolId);
                parameters.Add("@usu_Id", usuId);

                var result = await DbApp.Select<PR_Seguridad_RolesUsuarios_InsertResult>("Seguridad.PR_Seguridad_RolesUsuarios_Insert", parameters);
                return result?.rol_usu_Id > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoverRolUsuarioAsync(int rolId, int usuId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@rol_Id", rolId);
                parameters.Add("@usu_Id", usuId);

                var result = await DbApp.Select<PR_Seguridad_RolesUsuarios_DeleteResult>("Seguridad.PR_Seguridad_RolesUsuarios_Delete", parameters);
                return result?.FilasAfectadas > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RegistrarAccesoPantallaAsync(int usuId, int modptId, string userAgent = null, string direccionIP = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuId);
                parameters.Add("@modpt_Id", modptId);
                parameters.Add("@UserAgent", userAgent);
                parameters.Add("@DireccionIP", direccionIP);

                return await DbApp.Update("Seguridad.PR_Seguridad_RegistrarAccesoPantalla", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerificarAccesoPantallaAsync(int usuId, int modptId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuId);
                parameters.Add("@modpt_Id", modptId);

                var result = await DbApp.Select<int>("Seguridad.FN_UsuarioTieneAccesoPantalla", parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}