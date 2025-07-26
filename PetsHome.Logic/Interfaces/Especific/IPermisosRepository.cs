using PetsHome.Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.Logic.Interfaces.Especific
{
    public interface IPermisosRepository
    {
        // Módulos
        Task<List<PR_Seguridad_Modulos_ListResult>> GetModulosAsync();
        Task<bool> CreateModuloAsync(string nombre, string descripcion, string icono, string url, int orden);
        Task<bool> UpdateModuloAsync(int modId, string nombre, string descripcion, string icono, string url, int orden, bool activo);
        Task<bool> DeleteModuloAsync(int modId);
        
        // Permisos
        Task<List<PR_Seguridad_Permisos_ListResult>> GetPermisosAsync();
        Task<bool> CreatePermisoAsync(string nombre, string descripcion);
        Task<bool> UpdatePermisoAsync(int perId, string nombre, string descripcion, bool activo);
        Task<bool> DeletePermisoAsync(int perId);
        
        // Gestión de permisos por rol
        Task<List<PR_Seguridad_RolModuloPermisos_ListResult>> GetRolPermisosAsync(int rolId);
        Task<bool> AsignarModuloRolAsync(int rolId, int modId);
        Task<bool> RemoverModuloRolAsync(int rolId, int modId);
        Task<bool> AsignarPermisoRolModuloAsync(int rolId, int modId, int perId);
        Task<bool> RemoverPermisoRolModuloAsync(int rolId, int modId, int perId);
        
        // Obtener estructura completa de permisos
        Task<List<PR_Seguridad_RolModulosCompleto_ListResult>> GetRolModulosCompletoAsync(int rolId);
        
        // Para menús dinámicos
        Task<List<PR_Seguridad_MenuUsuario_ListResult>> GetMenuUsuarioAsync(int usuarioId);
        Task<List<PR_Seguridad_MenuUsuarioCompleto_ListResult>> GetMenuUsuarioCompletoAsync(int usuarioId);
        
        // ===== NUEVAS FUNCIONES PARA TABLAS EXTENDIDAS =====
        
        // Componentes
        Task<List<PR_Seguridad_Componentes_ListResult>> GetComponentesAsync();
        Task<bool> CreateComponenteAsync(string descripcion);
        Task<bool> UpdateComponenteAsync(int compId, string descripcion);
        Task<bool> DeleteComponenteAsync(int compId);
        
        // Módulos Pantallas
        Task<List<PR_Seguridad_ModulosPantallas_ListResult>> GetModulosPantallasAsync(int? modId = null);
        Task<bool> CreateModuloPantallaAsync(int modId, string descripcion, string url, string icono, int? orden);
        Task<bool> UpdateModuloPantallaAsync(int modptId, int modId, string descripcion, string url, string icono, int? orden, bool activo);
        Task<bool> DeleteModuloPantallaAsync(int modptId);
        
        // Rol Módulos Pantallas
        Task<List<PR_Seguridad_RolModulosPantallas_ListResult>> GetRolModulosPantallasAsync(int? rolId = null);
        Task<bool> AsignarPantallaRolAsync(int modptId, int rolId);
        Task<bool> RemoverPantallaRolAsync(int modptId, int rolId);
        Task<bool> AsignarMultiplesPantallasRolAsync(int rolId, string modptIds);
        Task<bool> RemoverMultiplesPantallasRolAsync(int rolId, string modptIds);
        
        // Roles Usuarios
        Task<List<PR_Seguridad_RolesUsuarios_ListResult>> GetRolesUsuariosAsync(int? usuId = null);
        Task<bool> AsignarRolUsuarioAsync(int rolId, int usuId);
        Task<bool> RemoverRolUsuarioAsync(int rolId, int usuId);
        
        // Menús extendidos
        Task<List<PR_Seguridad_PantallasPorUsuario_ListResult>> GetPantallasPorUsuarioAsync(int usuId);
        Task<bool> VerificarAccesoPantallaAsync(int usuId, int modptId);
    }
}