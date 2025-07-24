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
    }
}