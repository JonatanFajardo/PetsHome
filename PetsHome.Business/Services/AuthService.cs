using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Logic.Interfaces.Especific;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<ServiceResult> LoginAsync(LoginViewModel model, string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Usu_Nombre) || string.IsNullOrEmpty(model.Contrasena))
                {
                    return new ServiceResult { Success = false, Message = "Usuario y contraseña son requeridos" };
                }

                // Hash de la contraseña para comparar (SHA256 simple)
                string hashedPassword = HashPasswordSimple(model.Contrasena);

                var loginResult = await _authRepository.LoginAsync(model.Usu_Nombre, hashedPassword);

                if (loginResult == null)
                {
                    return new ServiceResult { Success = false, Message = "Usuario o contraseña incorrectos" };
                }

                // Verificar si el usuario está activo
                if (loginResult.Usu_EsActivo != true)
                {
                    return new ServiceResult { Success = false, Message = "Usuario inactivo" };
                }

                // Verificar si el usuario está suspendido
                if (loginResult.Usu_Suspendido == true)
                {
                    return new ServiceResult { Success = false, Message = "Usuario suspendido" };
                }

                // Actualizar último acceso
                await _authRepository.UpdateLastAccessAsync(loginResult.usu_Id, ipAddress);

                var usuarioViewModel = new UsuarioViewModel
                {
                    usu_Id = loginResult.usu_Id,
                    Emp_Id = loginResult.Emp_Id,
                    Usu_Nombre = loginResult.Usu_Nombre,
                    Emp_NombreCompleto = $"{loginResult.Emp_Nombres} {loginResult.Emp_Apellidos}",
                    Rol_Id = loginResult.Rol_Id,
                    Rol_Descripcion = loginResult.Rol_Descripcion,
                    Usu_EsActivo = loginResult.Usu_EsActivo,
                    Usu_Suspendido = loginResult.Usu_Suspendido
                };

                return new ServiceResult
                {
                    Success = true,
                    Message = "Login exitoso",
                    Data = usuarioViewModel
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
                if (await _authRepository.UsuarioExistsAsync(model.Usu_Nombre))
                {
                    return new ServiceResult { Success = false, Message = "El nombre de usuario ya existe" };
                }

                // Validar si el empleado ya tiene usuario
                if (await _authRepository.EmpleadoTieneUsuarioAsync(model.Emp_Id))
                {
                    return new ServiceResult { Success = false, Message = "El empleado ya tiene un usuario asignado" };
                }

                // Generar hash simple de la contraseña
                string hashedPassword = HashPasswordSimple(model.Contrasena);

                // Crear usuario directamente (ya no usamos tabla de contraseñas separada)
                int usuarioId = await _authRepository.CreateUsuarioAsync(
                    model.Emp_Id, 
                    model.Usu_Nombre, 
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
    }
}