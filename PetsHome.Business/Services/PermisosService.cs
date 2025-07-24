using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Logic.Interfaces.Especific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class PermisosService
    {
        private readonly IPermisosRepository _permisosRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public PermisosService(IPermisosRepository permisosRepository, IAuthRepository authRepository, IMapper mapper)
        {
            _permisosRepository = permisosRepository;
            _authRepository = authRepository;
            _mapper = mapper;
        }

        #region Gestión de Módulos

        public async Task<ServiceResult> GetModulosAsync()
        {
            try
            {
                var modulos = await _permisosRepository.GetModulosAsync();
                var modulosViewModel = modulos.Select(m => new ModuloViewModel
                {
                    Mod_Id = m.Mod_Id,
                    Mod_Nombre = m.Mod_Nombre,
                    Mod_Descripcion = m.Mod_Descripcion,
                    Mod_Icono = m.Mod_Icono,
                    Mod_Url = m.Mod_Url,
                    Mod_Orden = m.Mod_Orden,
                    Mod_EsActivo = m.Mod_EsActivo
                }).ToList();

                return new ServiceResult { Success = true, Data = modulosViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener módulos: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> CreateModuloAsync(ModuloViewModel modelo)
        {
            try
            {
                var created = await _permisosRepository.CreateModuloAsync(
                    modelo.Mod_Nombre,
                    modelo.Mod_Descripcion,
                    modelo.Mod_Icono,
                    modelo.Mod_Url,
                    modelo.Mod_Orden ?? 0
                );

                if (created)
                {
                    return new ServiceResult { Success = true, Message = "Módulo creado exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al crear el módulo" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al crear módulo: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> UpdateModuloAsync(ModuloViewModel modelo)
        {
            try
            {
                var updated = await _permisosRepository.UpdateModuloAsync(
                    modelo.Mod_Id,
                    modelo.Mod_Nombre,
                    modelo.Mod_Descripcion,
                    modelo.Mod_Icono,
                    modelo.Mod_Url,
                    modelo.Mod_Orden ?? 0,
                    modelo.Mod_EsActivo
                );

                if (updated)
                {
                    return new ServiceResult { Success = true, Message = "Módulo actualizado exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al actualizar el módulo" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al actualizar módulo: {ex.Message}" };
            }
        }

        #endregion

        #region Gestión de Permisos

        public async Task<ServiceResult> GetPermisosAsync()
        {
            try
            {
                var permisos = await _permisosRepository.GetPermisosAsync();
                var permisosViewModel = permisos.Select(p => new PermisoViewModel
                {
                    Per_Id = p.Per_Id,
                    Per_Nombre = p.Per_Nombre,
                    Per_Descripcion = p.Per_Descripcion,
                    Per_EsActivo = p.Per_EsActivo
                }).ToList();

                return new ServiceResult { Success = true, Data = permisosViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener permisos: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> CreatePermisoAsync(PermisoViewModel modelo)
        {
            try
            {
                var created = await _permisosRepository.CreatePermisoAsync(
                    modelo.Per_Nombre,
                    modelo.Per_Descripcion
                );

                if (created)
                {
                    return new ServiceResult { Success = true, Message = "Permiso creado exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al crear el permiso" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al crear permiso: {ex.Message}" };
            }
        }

        #endregion

        #region Gestión de Permisos por Rol

        public async Task<ServiceResult> GetGestionPermisosAsync(int rolId)
        {
            try
            {
                // Obtener información del rol
                var roles = await _authRepository.GetRolesAsync();
                var rol = roles.FirstOrDefault(r => r.Rol_Id == rolId);

                if (rol == null)
                {
                    return new ServiceResult { Success = false, Message = "Rol no encontrado" };
                }

                // Obtener módulos y permisos completos del rol
                var rolModulos = await _permisosRepository.GetRolModulosCompletoAsync(rolId);
                var todosPermisos = await _permisosRepository.GetPermisosAsync();

                // Construir el ViewModel
                var modelo = new GestionPermisosViewModel
                {
                    Rol_Id = rolId,
                    Rol_Descripcion = rol.Rol_Descripcion,
                    RolesDropdown = roles.Select(r => new { Value = r.Rol_Id, Text = r.Rol_Descripcion }).ToList<dynamic>()
                };

                // Mapear módulos con sus permisos
                modelo.Modulos = rolModulos.Select(rm => new ModuloPermisosViewModel
                {
                    Mod_Id = rm.Mod_Id,
                    Mod_Nombre = rm.Mod_Nombre,
                    Mod_Descripcion = rm.Mod_Descripcion,
                    Mod_Icono = rm.Mod_Icono,
                    TieneAcceso = rm.TieneAcceso,
                    Permisos = todosPermisos.Select(p => new PermisoCheckViewModel
                    {
                        Per_Id = p.Per_Id,
                        Per_Nombre = p.Per_Nombre,
                        Per_Descripcion = p.Per_Descripcion,
                        Seleccionado = !string.IsNullOrEmpty(rm.Permisos) && rm.Permisos.Split(',').Contains(p.Per_Nombre)
                    }).ToList()
                }).ToList();

                return new ServiceResult { Success = true, Data = modelo };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener gestión de permisos: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> GuardarPermisosRolAsync(int rolId, List<ModuloPermisosViewModel> modulos)
        {
            try
            {
                foreach (var modulo in modulos)
                {
                    if (modulo.TieneAcceso)
                    {
                        // Asegurar que el rol tenga acceso al módulo
                        await _permisosRepository.AsignarModuloRolAsync(rolId, modulo.Mod_Id);

                        // Procesar permisos específicos
                        foreach (var permiso in modulo.Permisos)
                        {
                            if (permiso.Seleccionado)
                            {
                                await _permisosRepository.AsignarPermisoRolModuloAsync(rolId, modulo.Mod_Id, permiso.Per_Id);
                            }
                            else
                            {
                                await _permisosRepository.RemoverPermisoRolModuloAsync(rolId, modulo.Mod_Id, permiso.Per_Id);
                            }
                        }
                    }
                    else
                    {
                        // Remover acceso completo al módulo (esto también removerá los permisos específicos)
                        await _permisosRepository.RemoverModuloRolAsync(rolId, modulo.Mod_Id);
                    }
                }

                return new ServiceResult { Success = true, Message = "Permisos guardados exitosamente" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al guardar permisos: {ex.Message}" };
            }
        }

        #endregion

        #region Menús Dinámicos

        public async Task<ServiceResult> GetMenuUsuarioAsync(int usuarioId)
        {
            try
            {
                var menuItemsCompleto = await _permisosRepository.GetMenuUsuarioCompletoAsync(usuarioId);

                if (!menuItemsCompleto.Any())
                {
                    return new ServiceResult { Success = false, Message = "Usuario sin permisos asignados" };
                }

                // Convertir a MenuItemViewModel
                var todosLosItems = menuItemsCompleto.Select(mi => new MenuItemViewModel
                {
                    Mod_Id = mi.Mod_Id,
                    Mod_Nombre = mi.Mod_Nombre,
                    Mod_Descripcion = mi.Mod_Descripcion,
                    Mod_Icono = mi.Mod_Icono,
                    Mod_Url = mi.Mod_Url,
                    Mod_Orden = mi.Mod_Orden ?? 0,
                    Permisos = !string.IsNullOrEmpty(mi.Permisos) ? mi.Permisos.Split(',').ToList() : new List<string>(),
                    TieneAcceso = true,
                    PuedeCrear = !string.IsNullOrEmpty(mi.Permisos) && mi.Permisos.Contains("CREATE"),
                    PuedeEditar = !string.IsNullOrEmpty(mi.Permisos) && mi.Permisos.Contains("UPDATE"),
                    PuedeEliminar = !string.IsNullOrEmpty(mi.Permisos) && mi.Permisos.Contains("DELETE"),
                    TipoItem = mi.TipoItem,
                    Mod_Padre = mi.Mod_Padre
                }).ToList();

                // Separar módulos principales de submódulos
                var modulosPrincipales = todosLosItems.Where(m => m.TipoItem == "MODULE").OrderBy(m => m.Mod_Orden).ToList();
                var submodulos = todosLosItems.Where(m => m.TipoItem == "SUBMODULE").ToList();

                // Asignar submódulos a sus módulos padre
                foreach (var modulo in modulosPrincipales)
                {
                    modulo.SubModulos = submodulos
                        .Where(s => s.Mod_Padre == modulo.Mod_Id)
                        .OrderBy(s => s.Mod_Orden)
                        .ToList();
                }

                var menuViewModel = new MenuViewModel
                {
                    UsuarioNombre = "", // Se puede obtener del contexto
                    RolDescripcion = menuItemsCompleto.First().Rol_Descripcion,
                    MenuItems = modulosPrincipales
                };

                return new ServiceResult { Success = true, Data = menuViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener menú: {ex.Message}" };
            }
        }

        #endregion
    }
}