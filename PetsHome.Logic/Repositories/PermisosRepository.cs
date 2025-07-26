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
    public class PermisosRepository : IPermisosRepository
    {
        public async Task<List<PR_Seguridad_Modulos_ListResult>> GetModulosAsync()
        {
            try
            {
                var result = await DbApp.Select<PR_Seguridad_Modulos_ListResult>("PR_Seguridad_Modulos_List");
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_Modulos_ListResult>();
            }
        }

        public async Task<List<PR_Seguridad_Permisos_ListResult>> GetPermisosAsync()
        {
            try
            {
                var result = await DbApp.Select<PR_Seguridad_Permisos_ListResult>("PR_Seguridad_Permisos_List");
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_Permisos_ListResult>();
            }
        }

        public async Task<bool> CreateModuloAsync(string nombre, string descripcion, string icono, string url, int orden)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mod_Nombre", nombre);
                parameters.Add("@Mod_Descripcion", descripcion);
                parameters.Add("@Mod_Icono", icono);
                parameters.Add("@Mod_Url", url);
                parameters.Add("@Mod_Orden", orden);

                return await DbApp.Insert("PR_Seguridad_Modulos_Insert", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateModuloAsync(int modId, string nombre, string descripcion, string icono, string url, int orden, bool activo)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mod_Id", modId);
                parameters.Add("@Mod_Nombre", nombre);
                parameters.Add("@Mod_Descripcion", descripcion);
                parameters.Add("@Mod_Icono", icono);
                parameters.Add("@Mod_Url", url);
                parameters.Add("@Mod_Orden", orden);
                parameters.Add("@Mod_EsActivo", activo);

                return await DbApp.Update("PR_Seguridad_Modulos_Update", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteModuloAsync(int modId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mod_Id", modId);

                return await DbApp.Delete("PR_Seguridad_Modulos_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreatePermisoAsync(string nombre, string descripcion)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Per_Nombre", nombre);
                parameters.Add("@Per_Descripcion", descripcion);

                return await DbApp.Insert("PR_Seguridad_Permisos_Insert", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePermisoAsync(int perId, string nombre, string descripcion, bool activo)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Per_Id", perId);
                parameters.Add("@Per_Nombre", nombre);
                parameters.Add("@Per_Descripcion", descripcion);
                parameters.Add("@Per_EsActivo", activo);

                return await DbApp.Update("PR_Seguridad_Permisos_Update", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePermisoAsync(int perId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Per_Id", perId);

                return await DbApp.Delete("PR_Seguridad_Permisos_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PR_Seguridad_RolModuloPermisos_ListResult>> GetRolPermisosAsync(int rolId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Rol_Id", rolId);

                var result = await DbApp.SelectById<PR_Seguridad_RolModuloPermisos_ListResult>("PR_Seguridad_RolModuloPermisos_List", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_RolModuloPermisos_ListResult>();
            }
        }

        public async Task<List<PR_Seguridad_RolModulosCompleto_ListResult>> GetRolModulosCompletoAsync(int rolId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Rol_Id", rolId);

                var result = await DbApp.SelectById<PR_Seguridad_RolModulosCompleto_ListResult>("PR_Seguridad_RolModulosCompleto_List", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_RolModulosCompleto_ListResult>();
            }
        }

        public async Task<bool> AsignarModuloRolAsync(int rolId, int modId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Rol_Id", rolId);
                parameters.Add("@Mod_Id", modId);

                return await DbApp.Insert("PR_Seguridad_RolModulos_Insert", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoverModuloRolAsync(int rolId, int modId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Rol_Id", rolId);
                parameters.Add("@Mod_Id", modId);

                return await DbApp.Delete("PR_Seguridad_RolModulos_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AsignarPermisoRolModuloAsync(int rolId, int modId, int perId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Rol_Id", rolId);
                parameters.Add("@Mod_Id", modId);
                parameters.Add("@Per_Id", perId);

                return await DbApp.Insert("PR_Seguridad_RolModuloPermisos_Insert", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoverPermisoRolModuloAsync(int rolId, int modId, int perId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Rol_Id", rolId);
                parameters.Add("@Mod_Id", modId);
                parameters.Add("@Per_Id", perId);

                return await DbApp.Delete("PR_Seguridad_RolModuloPermisos_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PR_Seguridad_MenuUsuario_ListResult>> GetMenuUsuarioAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                var result = await DbApp.SelectById<PR_Seguridad_MenuUsuario_ListResult>("PR_Seguridad_MenuUsuario_List", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_MenuUsuario_ListResult>();
            }
        }

        public async Task<List<PR_Seguridad_MenuUsuarioCompleto_ListResult>> GetMenuUsuarioCompletoAsync(int usuarioId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuarioId);

                var result = await DbApp.SelectById<PR_Seguridad_MenuUsuarioCompleto_ListResult>("PR_Seguridad_MenuUsuarioCompleto_List", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_MenuUsuarioCompleto_ListResult>();
            }
        }

        // ===== IMPLEMENTACIÓN DE NUEVAS FUNCIONES =====

        #region Componentes

        public async Task<List<PR_Seguridad_Componentes_ListResult>> GetComponentesAsync()
        {
            try
            {
                var result = await DbApp.Select<PR_Seguridad_Componentes_ListResult>("Seguridad.PR_Seguridad_Componentes_List");
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_Componentes_ListResult>();
            }
        }

        public async Task<bool> CreateComponenteAsync(string descripcion)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@comp_Descripcion", descripcion);

                return await DbApp.Insert("Seguridad.PR_Seguridad_Componentes_Insert", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateComponenteAsync(int compId, string descripcion)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@comp_Id", compId);
                parameters.Add("@comp_Descripcion", descripcion);

                return await DbApp.Update("Seguridad.PR_Seguridad_Componentes_Update", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteComponenteAsync(int compId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@comp_Id", compId);

                return await DbApp.Delete("Seguridad.PR_Seguridad_Componentes_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Módulos Pantallas

        public async Task<List<PR_Seguridad_ModulosPantallas_ListResult>> GetModulosPantallasAsync(int? modId = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                if (modId.HasValue)
                    parameters.Add("@mod_Id", modId.Value);

                var result = await DbApp.SelectById<PR_Seguridad_ModulosPantallas_ListResult>("Seguridad.PR_Seguridad_ModulosPantallas_List", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_ModulosPantallas_ListResult>();
            }
        }

        public async Task<bool> CreateModuloPantallaAsync(int modId, string descripcion, string url, string icono, int? orden)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@mod_Id", modId);
                parameters.Add("@modpt_Descripcion", descripcion);
                parameters.Add("@modpt_Url", url);
                parameters.Add("@modpt_Icono", icono);
                parameters.Add("@modpt_Orden", orden);

                return await DbApp.Insert("Seguridad.PR_Seguridad_ModulosPantallas_Insert", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateModuloPantallaAsync(int modptId, int modId, string descripcion, string url, string icono, int? orden, bool activo)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@modpt_Id", modptId);
                parameters.Add("@mod_Id", modId);
                parameters.Add("@modpt_Descripcion", descripcion);
                parameters.Add("@modpt_Url", url);
                parameters.Add("@modpt_Icono", icono);
                parameters.Add("@modpt_Orden", orden);
                parameters.Add("@modpt_EsActivo", activo);

                return await DbApp.Update("Seguridad.PR_Seguridad_ModulosPantallas_Update", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteModuloPantallaAsync(int modptId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@modpt_Id", modptId);

                return await DbApp.Delete("Seguridad.PR_Seguridad_ModulosPantallas_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Rol Módulos Pantallas

        public async Task<List<PR_Seguridad_RolModulosPantallas_ListResult>> GetRolModulosPantallasAsync(int? rolId = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                if (rolId.HasValue)
                    parameters.Add("@rol_Id", rolId.Value);

                var result = await DbApp.SelectById<PR_Seguridad_RolModulosPantallas_ListResult>("Seguridad.PR_Seguridad_RolModulosPantallas_List", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_RolModulosPantallas_ListResult>();
            }
        }

        public async Task<bool> AsignarPantallaRolAsync(int modptId, int rolId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@modpt_Id", modptId);
                parameters.Add("@rol_Id", rolId);

                return await DbApp.Insert("Seguridad.PR_Seguridad_RolModulosPantallas_Insert", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoverPantallaRolAsync(int modptId, int rolId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@modpt_Id", modptId);
                parameters.Add("@rol_Id", rolId);

                return await DbApp.Delete("Seguridad.PR_Seguridad_RolModulosPantallas_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AsignarMultiplesPantallasRolAsync(int rolId, string modptIds)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@rol_Id", rolId);
                parameters.Add("@modpt_Ids", modptIds);

                return await DbApp.Insert("Seguridad.PR_Seguridad_AsignarPantallasRol", parameters);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoverMultiplesPantallasRolAsync(int rolId, string modptIds)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@rol_Id", rolId);
                parameters.Add("@modpt_Ids", modptIds);

                return await DbApp.Delete("Seguridad.PR_Seguridad_RemoverPantallasRol", parameters);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Roles Usuarios

        public async Task<List<PR_Seguridad_RolesUsuarios_ListResult>> GetRolesUsuariosAsync(int? usuId = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                if (usuId.HasValue)
                    parameters.Add("@usu_Id", usuId.Value);

                var result = await DbApp.SelectById<PR_Seguridad_RolesUsuarios_ListResult>("Seguridad.PR_Seguridad_RolesUsuarios_List", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_RolesUsuarios_ListResult>();
            }
        }

        public async Task<bool> AsignarRolUsuarioAsync(int rolId, int usuId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@rol_Id", rolId);
                parameters.Add("@usu_Id", usuId);

                return await DbApp.Insert("Seguridad.PR_Seguridad_RolesUsuarios_Insert", parameters);
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

                return await DbApp.Delete("Seguridad.PR_Seguridad_RolesUsuarios_Delete", parameters);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Menús Extendidos

        public async Task<List<PR_Seguridad_PantallasPorUsuario_ListResult>> GetPantallasPorUsuarioAsync(int usuId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuId);

                var result = await DbApp.SelectById<PR_Seguridad_PantallasPorUsuario_ListResult>("Seguridad.PR_Seguridad_MenuUsuarioCompleto_V2", parameters);
                return result.ToList();
            }
            catch
            {
                return new List<PR_Seguridad_PantallasPorUsuario_ListResult>();
            }
        }

        public async Task<bool> VerificarAccesoPantallaAsync(int usuId, int modptId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@usu_Id", usuId);
                parameters.Add("@modpt_Id", modptId);

                var result = await DbApp.Select<int>("Seguridad.PR_Seguridad_VerificarAccesoPantalla", parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}