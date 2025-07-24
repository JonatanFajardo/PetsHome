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
    }
}