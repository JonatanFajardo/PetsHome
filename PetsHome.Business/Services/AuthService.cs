using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Logic.Interfaces.Especific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PetsHome.Business.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResult> LoginAsync(LoginViewModel model, string ipAddress, string userAgent = null)
        {
            try
            {
                if (string.IsNullOrEmpty(model.usu_NombreUsuario) || string.IsNullOrEmpty(model.usu_Contraseña))
                {
                    return new ServiceResult { Success = false, Message = "Usuario y contraseña son requeridos" };
                }

                // Hash de la contraseña para comparar (SHA256 simple)
                string hashedPassword = HashPasswordSimple(model.usu_Contraseña);

                // Usar el nuevo método de login V2 que maneja bloqueos y auditoría
                var (loginResult, roles) = await _authRepository.LoginV2Async(model.usu_NombreUsuario, hashedPassword, userAgent, ipAddress);

                if (loginResult == null || loginResult.Resultado == 0)
                {
                    return new ServiceResult { Success = false, Message = loginResult?.Mensaje ?? "Usuario o contraseña incorrectos" };
                }

                var usuarioViewModel = new UsuarioViewModel
                {
                    usu_Id = loginResult.usu_Id,
                    Emp_Id = loginResult.Emp_Id,
                    Usu_Nombre = loginResult.Usu_Nombre,
                    Emp_NombreCompleto = $"{loginResult.Emp_Nombres} {loginResult.Emp_Apellidos}",
                    usu_ImagenPerfil = loginResult.usu_ImagenPerfil,
                    // Para mantener compatibilidad, asignar el primer rol como rol principal
                    Rol_Id = roles.FirstOrDefault()?.rol_Id ?? 0,
                    Rol_Descripcion = roles.FirstOrDefault()?.Rol_Descripcion ?? "Sin rol",
                    // Los nuevos campos se llenarán cuando sea necesario
                    Usu_EsActivo = true, // Si llegó aquí, está activo
                    Usu_Suspendido = false
                };

                return new ServiceResult
                {
                    Success = true,
                    Message = loginResult.Mensaje,
                    Data = new LoginExtendidoResult
                    {
                        Usuario = usuarioViewModel,
                        Roles = roles // Incluir todos los roles del usuario
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error en el login: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> GetUsuarioDetailAsync(int usuarioId)
        {
            try
            {
                var usuarioDetail = await _authRepository.GetUsuarioDetailAsync(usuarioId);

                if (usuarioDetail == null)
                {
                    return new ServiceResult { Success = false, Message = "Usuario no encontrado" };
                }

                var usuarioViewModel = new UsuarioViewModel
                {
                    usu_Id = usuarioDetail.usu_Id,
                    Emp_Id = usuarioDetail.Emp_Id,
                    Usu_Nombre = usuarioDetail.Usu_Nombre,
                    Emp_NombreCompleto = $"{usuarioDetail.Emp_Nombres} {usuarioDetail.Emp_Apellidos}",
                    Rol_Id = usuarioDetail.Rol_Id,
                    Rol_Descripcion = usuarioDetail.Rol_Descripcion,
                    Usu_Ip = usuarioDetail.Usu_Ip,
                    Usu_EsActivo = usuarioDetail.Usu_EsActivo,
                    Usu_Suspendido = usuarioDetail.Usu_Suspendido,
                    Usu_FechaCreacion = usuarioDetail.Usu_FechaCreacion,
                    Usu_fechaModificacion = usuarioDetail.Usu_fechaModificacion
                };

                return new ServiceResult { Success = true, Data = usuarioViewModel };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener usuario: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> GetRolesAsync()
        {
            try
            {
                var roles = await _authRepository.GetRolesAsync();
                return new ServiceResult { Success = true, Data = roles };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener roles: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> CreateUsuarioAsync(RegistroUsuarioViewModel model, int usuarioCreacion, string ipAddress)
        {
            try
            {
                // Validar si el usuario ya existe
                if (await _authRepository.UsuarioExistsAsync(model.usu_NombreUsuario))
                {
                    return new ServiceResult { Success = false, Message = "El nombre de usuario ya existe" };
                }

                // Validar si el empleado ya tiene usuario
                if (await _authRepository.EmpleadoTieneUsuarioAsync(model.Emp_Id))
                {
                    return new ServiceResult { Success = false, Message = "El empleado ya tiene un usuario asignado" };
                }

                // Generar hash simple de la contraseña
                string hashedPassword = HashPasswordSimple(model.usu_Contraseña);

                // Crear usuario directamente (ya no usamos tabla de contraseñas separada)
                int usuarioId = await _authRepository.CreateUsuarioAsync(
                    model.Emp_Id, 
                    model.usu_NombreUsuario, 
                    hashedPassword, 
                    model.Rol_Id, 
                    ipAddress, 
                    usuarioCreacion);

                if (usuarioId <= 0)
                {
                    return new ServiceResult { Success = false, Message = "Error al crear el usuario" };
                }

                return new ServiceResult { Success = true, Message = "Usuario creado exitosamente", Data = usuarioId };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al crear usuario: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> ChangePasswordAsync(int usuarioId, CambiarContrasenaViewModel model)
        {
            try
            {
                // Verificar contraseña actual
                var usuario = await _authRepository.GetUsuarioDetailAsync(usuarioId);
                if (usuario == null)
                {
                    return new ServiceResult { Success = false, Message = "Usuario no encontrado" };
                }

                // Generar hash de la nueva contraseña
                string newPasswordHash = HashPasswordSimple(model.NuevaContrasena);

                // Cambiar contraseña
                bool success = await _authRepository.ChangePasswordAsync(usuarioId, newPasswordHash);

                if (!success)
                {
                    return new ServiceResult { Success = false, Message = "Error al cambiar la contraseña" };
                }

                return new ServiceResult { Success = true, Message = "Contraseña cambiada exitosamente" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al cambiar contraseña: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> GetUsuarioPermissionsAsync(int usuarioId)
        {
            try
            {
                var permisos = await _authRepository.GetUsuarioPermissionsAsync(usuarioId);
                return new ServiceResult { Success = true, Data = permisos };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener permisos: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> GetRolePermissionsAsync(int rolId)
        {
            try
            {
                var permisos = await _authRepository.GetRolePermissionsAsync(rolId);
                return new ServiceResult { Success = true, Data = permisos };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener permisos del rol: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> CheckPermissionAsync(int usuarioId, string modulo, string permiso)
        {
            try
            {
                var tienePermiso = await _authRepository.CheckPermissionAsync(usuarioId, modulo, permiso);
                return new ServiceResult { Success = true, Data = tienePermiso };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al verificar permiso: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> GetPantallasUsuarioAsync(int usuarioId)
        {
            try
            {
                var pantallas = await _authRepository.GetPantallasUsuarioAsync(usuarioId);
                if (pantallas == null || !pantallas.Any())
                {
                    return new ServiceResult { Success = false, Message = "Usuario sin pantallas asignadas" };
                }

                return new ServiceResult { Success = true, Data = pantallas };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener pantallas: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> LogoutAsync(int usuarioId, string userAgent = null, string ipAddress = null)
        {
            try
            {
                // Usar el nuevo método de logout V2 que registra eventos de auditoría
                var success = await _authRepository.LogoutV2Async(usuarioId, userAgent, ipAddress);
                
                if (success)
                {
                    return new ServiceResult { Success = true, Message = "Logout exitoso" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al cerrar sesión" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al hacer logout: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> LoadUserPermissionsInSessionAsync(int usuarioId)
        {
            try
            {
                // Obtener todos los permisos del usuario
                var permisos = await _authRepository.GetUserPermissionsAsync(usuarioId);
                
                // Crear diccionario agrupado por pantalla
                var permisosPorPantalla = permisos
                    .GroupBy(p => p.Pantalla)
                    .ToDictionary(
                        g => g.Key, 
                        g => g.Select(p => p.Permiso).ToList()
                    );

                // Convertir a JSON para almacenar en sesión
                var permisosJson = JsonConvert.SerializeObject(permisosPorPantalla);

                // Obtener lista simple de pantallas (para compatibilidad con sistema actual)
                var pantallas = await _authRepository.GetUserPantallasAsync(usuarioId);
                var pantallasString = string.Join(",", pantallas.Select(p => p.Pantalla));

                return new ServiceResult 
                { 
                    Success = true, 
                    Data = new PermisosSessionResult
                    {
                        PermisosJson = permisosJson,
                        PantallasString = pantallasString,
                        PermisosPorPantalla = permisosPorPantalla
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al cargar permisos en sesión: {ex.Message}" };
            }
        }

        public bool CheckSessionPermission(string permisosJson, string pantalla, string permiso)
        {
            try
            {
                if (string.IsNullOrEmpty(permisosJson) || string.IsNullOrEmpty(pantalla) || string.IsNullOrEmpty(permiso))
                    return false;

                var permisosPorPantalla = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(permisosJson);
                
                if (permisosPorPantalla != null && permisosPorPantalla.ContainsKey(pantalla))
                {
                    return permisosPorPantalla[pantalla].Contains(permiso);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #region Métodos privados para hash de contraseñas

        private string HashPasswordSimple(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private (string hash, string salt) HashPasswordWithSalt(string password)
        {
            // Generar salt aleatorio
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);

            // Combinar contraseña con salt y generar hash
            string saltedPassword = password + salt;
            string hash = HashPassword(saltedPassword);

            return (hash, salt);
        }

        private bool VerifyPassword(string password, string hash, string salt)
        {
            string saltedPassword = password + salt;
            string hashedInput = HashPassword(saltedPassword);
            return hashedInput.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Métodos para el sistema de seguridad mejorado

        public async Task<ServiceResult> GetPantallasPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var (componentes, modulos, pantallas) = await _authRepository.GetPantallasPorUsuarioAsync(usuarioId);

                if (!pantallas.Any())
                {
                    return new ServiceResult { Success = false, Message = "Usuario sin pantallas asignadas" };
                }

                var result = new PantallasUsuarioResult
                {
                    Componentes = componentes,
                    Modulos = modulos,
                    Pantallas = pantallas
                };

                return new ServiceResult { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener pantallas del usuario: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> GetPantallasPorRolAsync(int rolId)
        {
            try
            {
                var (componentes, modulos, pantallas) = await _authRepository.GetPantallasPorRolAsync(rolId);

                var result = new PantallasUsuarioResult
                {
                    Componentes = componentes,
                    Modulos = modulos,
                    Pantallas = pantallas
                };

                return new ServiceResult { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al obtener pantallas del rol: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> AsignarRolUsuarioAsync(int rolId, int usuId)
        {
            try
            {
                var success = await _authRepository.AsignarRolUsuarioAsync(rolId, usuId);

                if (success)
                {
                    return new ServiceResult { Success = true, Message = "Rol asignado exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al asignar el rol o el usuario ya tiene este rol" };
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
                var success = await _authRepository.RemoverRolUsuarioAsync(rolId, usuId);

                if (success)
                {
                    return new ServiceResult { Success = true, Message = "Rol removido exitosamente" };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Error al remover el rol" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al remover rol: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> VerificarAccesoPantallaAsync(int usuId, int modptId)
        {
            try
            {
                var tieneAcceso = await _authRepository.VerificarAccesoPantallaAsync(usuId, modptId);
                return new ServiceResult { Success = true, Data = tieneAcceso };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al verificar acceso: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> RegistrarAccesoPantallaAsync(int usuId, int modptId, string userAgent = null, string ipAddress = null)
        {
            try
            {
                var success = await _authRepository.RegistrarAccesoPantallaAsync(usuId, modptId, userAgent, ipAddress);
                return new ServiceResult { Success = success, Message = success ? "Acceso registrado" : "Error al registrar acceso" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al registrar acceso: {ex.Message}" };
            }
        }

        #endregion
    }
}