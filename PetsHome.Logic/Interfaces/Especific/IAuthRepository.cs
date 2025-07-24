using PetsHome.Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.Logic.Interfaces.Especific
{
    public interface IAuthRepository
    {
        Task<PR_Seguridad_Usuarios_LoginResult> LoginAsync(string usuario, string hashContrasena);
        Task<PR_Seguridad_Usuarios_DetailResult> GetUsuarioDetailAsync(int usuarioId);
        Task<List<PR_Seguridad_Roles_ListResult>> GetRolesAsync();
        Task<int> CreateContrasenaAsync(string hash, string salt, int usuarioCreacion);
        Task<int> CreateUsuarioAsync(int empId, string usuario, string passwordHash, int rolId, string ip, int usuarioCreacion);
        Task<bool> UpdateLastAccessAsync(int usuarioId, string ip);
        Task<bool> ChangePasswordAsync(int usuarioId, string passwordHash);
        Task<bool> UsuarioExistsAsync(string usuario);
        Task<bool> EmpleadoTieneUsuarioAsync(int empId);
        
        // Nuevos métodos para sistema RBAC
        Task<List<PR_Seguridad_Usuarios_GetPermissionsResult>> GetUsuarioPermissionsAsync(int usuarioId);
        Task<List<PR_Seguridad_Roles_GetPermissionsResult>> GetRolePermissionsAsync(int rolId);
        Task<bool> CheckPermissionAsync(int usuarioId, string modulo, string permiso);
    }
}