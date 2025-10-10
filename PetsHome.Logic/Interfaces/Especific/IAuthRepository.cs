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
        Task<List<PR_Seguridad_PantallasPorUsuario_ListResult>> GetPantallasUsuarioAsync(int usuarioId);
        
        // Nuevos métodos para sistema basado en sesiones
        Task<List<PR_Seguridad_GetUserPermissionsResult>> GetUserPermissionsAsync(int usuarioId);
        Task<List<PR_Seguridad_GetUserPantallasResult>> GetUserPantallasAsync(int usuarioId);
        
        // Nuevos métodos para el sistema de seguridad mejorado
        Task<(PR_Seguridad_Usuarios_Login_V2Result usuario, List<PR_Seguridad_Usuarios_Login_V2RolesResult> roles)> LoginV2Async(string usuario, string hashContrasena, string userAgent = null, string direccionIP = null);
        Task<bool> LogoutV2Async(int usuarioId, string userAgent = null, string direccionIP = null);
        Task<(List<PR_Seguridad_PantallasPorRol_ComponentesResult> componentes, List<PR_Seguridad_PantallasPorRol_ModulosResult> modulos, List<PR_Seguridad_PantallasPorRol_PantallasResult> pantallas)> GetPantallasPorRolAsync(int rolId);
        Task<(List<PR_Seguridad_PantallasPorRol_ComponentesResult> componentes, List<PR_Seguridad_PantallasPorRol_ModulosResult> modulos, List<PR_Seguridad_PantallasPorRol_PantallasResult> pantallas)> GetPantallasPorUsuarioAsync(int usuarioId);
        Task<bool> AsignarRolUsuarioAsync(int rolId, int usuId);
        Task<bool> RemoverRolUsuarioAsync(int rolId, int usuId);
        Task<bool> RegistrarAccesoPantallaAsync(int usuId, int modptId, string userAgent = null, string direccionIP = null);
        Task<bool> VerificarAccesoPantallaAsync(int usuId, int modptId);
    }
}