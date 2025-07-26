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

        #region Servicios Extendidos para Nuevas Tablas

        #region Componentes

        public async Task<ServiceResult> GetComponentesAsync()
        {
            try
            {
                var componentes = await _permisosRepository.GetComponentesAsync();
                var componentesViewModel = componentes.Select(c => new ComponenteViewModel
                {
                    comp_Id = c.comp_Id,
                    comp_Descripcion = c.comp_Descripcion
                }).ToList();

                return new ServiceResult { Success = true, Data = componentesViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener componentes: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> CreateComponenteAsync(ComponenteViewModel modelo)
        {
            try
            {
                var created = await _permisosRepository.CreateComponenteAsync(modelo.comp_Descripcion);

                if (created)
                {
                    return new ServiceResult { Success = true, Message = "Componente creado exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al crear el componente" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al crear componente: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> UpdateComponenteAsync(ComponenteViewModel modelo)
        {
            try
            {
                var updated = await _permisosRepository.UpdateComponenteAsync(
                    modelo.comp_Id, modelo.comp_Descripcion);

                if (updated)
                {
                    return new ServiceResult { Success = true, Message = "Componente actualizado exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al actualizar el componente" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al actualizar componente: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> DeleteComponenteAsync(int compId)
        {
            try
            {
                var deleted = await _permisosRepository.DeleteComponenteAsync(compId);

                if (deleted)
                {
                    return new ServiceResult { Success = true, Message = "Componente eliminado exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al eliminar el componente" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al eliminar componente: {ex.Message}" };
            }
        }

        #endregion

        #region Módulos Pantallas

        public async Task<ServiceResult> GetModulosPantallasAsync(int? modId = null)
        {
            try
            {
                var pantallas = await _permisosRepository.GetModulosPantallasAsync(modId);
                var pantallasViewModel = pantallas.Select(p => new ModuloPantallaViewModel
                {
                    modpt_Id = p.modpt_Id,
                    mod_Id = p.mod_Id,
                    modpt_Descripcion = p.modpt_Descripcion,
                    modpt_Url = p.modpt_Url,
                    modpt_Icono = p.modpt_Icono,
                    modpt_Orden = p.modpt_Orden,
                    modpt_EsActivo = p.modpt_EsActivo,
                    Mod_Nombre = p.Mod_Nombre,
                    Mod_Descripcion = p.Mod_Descripcion,
                    comp_Descripcion = p.comp_Descripcion
                }).ToList();

                return new ServiceResult { Success = true, Data = pantallasViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener pantallas: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> CreateModuloPantallaAsync(ModuloPantallaViewModel modelo)
        {
            try
            {
                var created = await _permisosRepository.CreateModuloPantallaAsync(
                    modelo.mod_Id, modelo.modpt_Descripcion, modelo.modpt_Url, 
                    modelo.modpt_Icono, modelo.modpt_Orden);

                if (created)
                {
                    return new ServiceResult { Success = true, Message = "Pantalla creada exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al crear la pantalla" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al crear pantalla: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> UpdateModuloPantallaAsync(ModuloPantallaViewModel modelo)
        {
            try
            {
                var updated = await _permisosRepository.UpdateModuloPantallaAsync(
                    modelo.modpt_Id, modelo.mod_Id, modelo.modpt_Descripcion, 
                    modelo.modpt_Url, modelo.modpt_Icono, modelo.modpt_Orden, modelo.modpt_EsActivo);

                if (updated)
                {
                    return new ServiceResult { Success = true, Message = "Pantalla actualizada exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al actualizar la pantalla" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al actualizar pantalla: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> DeleteModuloPantallaAsync(int modptId)
        {
            try
            {
                var deleted = await _permisosRepository.DeleteModuloPantallaAsync(modptId);

                if (deleted)
                {
                    return new ServiceResult { Success = true, Message = "Pantalla eliminada exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al eliminar la pantalla" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al eliminar pantalla: {ex.Message}" };
            }
        }

        #endregion

        #region Gestión de Permisos por Pantallas

        public async Task<ServiceResult> GetRolModulosPantallasAsync(int? rolId = null)
        {
            try
            {
                var asignaciones = await _permisosRepository.GetRolModulosPantallasAsync(rolId);
                var asignacionesViewModel = asignaciones.Select(a => new RolModuloPantallaViewModel
                {
                    rolpt_Id = a.rolpt_Id,
                    modpt_Id = a.modpt_Id,
                    rol_Id = a.rol_Id,
                    rolpt_FechaAsignacion = a.rolpt_FechaAsignacion,
                    modpt_Descripcion = a.modpt_Descripcion,
                    modpt_Url = a.modpt_Url,
                    Mod_Nombre = a.Mod_Nombre,
                    Rol_Descripcion = a.Rol_Descripcion
                }).ToList();

                return new ServiceResult { Success = true, Data = asignacionesViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener asignaciones: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> AsignarPantallaRolAsync(int modptId, int rolId)
        {
            try
            {
                var assigned = await _permisosRepository.AsignarPantallaRolAsync(modptId, rolId);

                if (assigned)
                {
                    return new ServiceResult { Success = true, Message = "Pantalla asignada al rol exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al asignar pantalla al rol" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al asignar pantalla: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> RemoverPantallaRolAsync(int modptId, int rolId)
        {
            try
            {
                var removed = await _permisosRepository.RemoverPantallaRolAsync(modptId, rolId);

                if (removed)
                {
                    return new ServiceResult { Success = true, Message = "Pantalla removida del rol exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al remover pantalla del rol" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al remover pantalla: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> AsignacionMasivaPantallasAsync(AsignacionMasivaViewModel modelo)
        {
            try
            {
                var pantallasIds = string.Join(",", modelo.PantallasIds);
                bool result;

                if (modelo.Operacion == "ASIGNAR")
                {
                    result = await _permisosRepository.AsignarMultiplesPantallasRolAsync(modelo.Rol_Id, pantallasIds);
                }
                else
                {
                    result = await _permisosRepository.RemoverMultiplesPantallasRolAsync(modelo.Rol_Id, pantallasIds);
                }

                if (result)
                {
                    var mensaje = modelo.Operacion == "ASIGNAR" ? "Pantallas asignadas" : "Pantallas removidas";
                    return new ServiceResult { Success = true, Message = $"{mensaje} exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error en la operación masiva" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error en asignación masiva: {ex.Message}" };
            }
        }

        #endregion

        #region Gestión de Roles por Usuario

        public async Task<ServiceResult> GetRolesUsuariosAsync(int? usuId = null)
        {
            try
            {
                var rolesUsuarios = await _permisosRepository.GetRolesUsuariosAsync(usuId);
                var rolesUsuariosViewModel = rolesUsuarios.Select(ru => new RolUsuarioViewModel
                {
                    rol_usu_Id = ru.rol_usu_Id,
                    rol_Id = ru.rol_Id,
                    usu_Id = ru.usu_Id,
                    rol_usu_FechaAsignacion = ru.rol_usu_FechaAsignacion,
                    Rol_Descripcion = ru.Rol_Descripcion,
                    Usu_Nombre = ru.Usu_Nombre,
                    Emp_NombreCompleto = ru.Emp_NombreCompleto
                }).ToList();

                return new ServiceResult { Success = true, Data = rolesUsuariosViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener roles de usuarios: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> AsignarRolUsuarioAsync(int rolId, int usuId)
        {
            try
            {
                var assigned = await _permisosRepository.AsignarRolUsuarioAsync(rolId, usuId);

                if (assigned)
                {
                    return new ServiceResult { Success = true, Message = "Rol asignado al usuario exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al asignar rol al usuario" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al asignar rol: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> RemoverRolUsuarioAsync(int rolId, int usuId)
        {
            try
            {
                var removed = await _permisosRepository.RemoverRolUsuarioAsync(rolId, usuId);

                if (removed)
                {
                    return new ServiceResult { Success = true, Message = "Rol removido del usuario exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al remover rol del usuario" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al remover rol: {ex.Message}" };
            }
        }

        #endregion

        #region Menús Extendidos

        public async Task<ServiceResult> GetMenuExtendidoUsuarioAsync(int usuarioId)
        {
            try
            {
                var pantallasUsuario = await _permisosRepository.GetPantallasPorUsuarioAsync(usuarioId);

                if (!pantallasUsuario.Any())
                {
                    return new ServiceResult { Success = false, Message = "Usuario sin permisos de pantallas asignados" };
                }

                var menuExtendido = new MenuExtendidoViewModel();

                // Agrupar por componentes
                var componentesAgrupados = pantallasUsuario
                    .GroupBy(p => new { p.comp_Id, p.comp_Descripcion })
                    .Select(c => new ComponenteMenuViewModel
                    {
                        comp_Id = c.Key.comp_Id,
                        comp_Descripcion = c.Key.comp_Descripcion,
                        Modulos = c.GroupBy(m => new { m.mod_Id, m.Mod_Nombre, m.Mod_Descripcion, m.Mod_Icono, m.Mod_Orden })
                            .Select(m => new ModuloMenuExtendidoViewModel
                            {
                                Mod_Id = m.Key.mod_Id,
                                Mod_Nombre = m.Key.Mod_Nombre,
                                Mod_Descripcion = m.Key.Mod_Descripcion,
                                Mod_Icono = m.Key.Mod_Icono,
                                Mod_Orden = m.Key.Mod_Orden,
                                TieneAcceso = true,
                                Permisos = !string.IsNullOrEmpty(m.First().Permisos) ? 
                                          m.First().Permisos.Split(',').ToList() : new List<string>(),
                                PuedeCrear = !string.IsNullOrEmpty(m.First().Permisos) && m.First().Permisos.Contains("CREATE"),
                                PuedeEditar = !string.IsNullOrEmpty(m.First().Permisos) && m.First().Permisos.Contains("UPDATE"),
                                PuedeEliminar = !string.IsNullOrEmpty(m.First().Permisos) && m.First().Permisos.Contains("DELETE"),
                                Pantallas = m.Select(p => new PantallaMenuViewModel
                                {
                                    modpt_Id = p.modpt_Id,
                                    modpt_Descripcion = p.modpt_Descripcion,
                                    modpt_Url = p.modpt_Url,
                                    modpt_Icono = p.modpt_Icono,
                                    modpt_Orden = p.modpt_Orden,
                                    TieneAcceso = true
                                }).OrderBy(p => p.modpt_Orden).ToList()
                            }).OrderBy(m => m.Mod_Orden).ToList()
                    }).ToList();

                menuExtendido.Componentes = componentesAgrupados;

                return new ServiceResult { Success = true, Data = menuExtendido };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener menú extendido: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> VerificarAccesoPantallaAsync(int usuId, int modptId)
        {
            try
            {
                var tieneAcceso = await _permisosRepository.VerificarAccesoPantallaAsync(usuId, modptId);
                return new ServiceResult { Success = true, Data = tieneAcceso };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al verificar acceso: {ex.Message}" };
            }
        }

        #endregion

        #endregion
    }
}