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
    }
}