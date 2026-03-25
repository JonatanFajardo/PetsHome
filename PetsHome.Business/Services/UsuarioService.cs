using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio para la gestión de usuarios y autenticación
    /// </summary>
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(UsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        #region CRUD

        public async Task<IEnumerable<UsuarioCrudViewModel>> ListAsync()
        {
            var result = await _usuarioRepository.ListAsync();
            return _mapper.Map<IEnumerable<UsuarioCrudViewModel>>(result);
        }

        public async Task<UsuarioCrudViewModel> FindAsync(int id)
        {
            var result = await _usuarioRepository.FindAsync(id);
            return _mapper.Map<UsuarioCrudViewModel>(result);
        }

        public async Task<bool> CreateAsync(UsuarioCrudViewModel model)
        {
            string passwordHash = HashPassword(model.Usu_Nombre);
            return await _usuarioRepository.InsertAsync(model.Usu_Nombre, model.Emp_Id, model.Rol_Id, 0, model.Usu_EsActivo);
        }

        public async Task<bool> EditAsync(UsuarioCrudViewModel model)
        {
            return await _usuarioRepository.UpdateAsync(model.usu_Id, model.Usu_Nombre, model.Emp_Id, model.Rol_Id, model.Usu_EsActivo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _usuarioRepository.DeleteAsync(id);
        }

        public async Task<UsuarioCrudViewModel> ExistAsync(string nombre)
        {
            var result = await _usuarioRepository.ExistAsync(nombre);
            return result != null ? new UsuarioCrudViewModel { usu_Id = result.usu_Id, Usu_Nombre = result.Usu_Nombre } : null;
        }

        #endregion

        #region Autenticación

        /// <summary>
        /// Autentica un usuario con sus credenciales
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>UsuarioViewModel si las credenciales son válidas, null en caso contrario</returns>
        public async Task<UsuarioViewModel> AuthenticateAsync(string username, string password)
        {
            try
            {
                // Generar el hash de la contraseña
                string passwordHash = HashPassword(password);

                // Llamar al repositorio para validar credenciales
                var loginResult = await _usuarioRepository.LoginAsync(username, passwordHash);

                if (loginResult == null)
                {
                    return null; // Credenciales inválidas
                }

                // Mapear el resultado a UsuarioViewModel
                var usuarioViewModel = _mapper.Map<UsuarioViewModel>(loginResult);

                // Marcar al usuario como logueado en la base de datos
                await _usuarioRepository.LoginInAsync(loginResult.usu_Id);

                return usuarioViewModel;
            }
            catch (Exception ex)
            {
                // Log del error (puedes usar Serilog aquí)
                throw new Exception($"Error al autenticar usuario: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cierra la sesión de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si se cerró la sesión exitosamente</returns>
        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                var result = await _usuarioRepository.LogoutAsync(userId);
                return result == 0; // 0 = éxito
            }
            catch (Exception ex)
            {
                // Log del error
                throw new Exception($"Error al cerrar sesión: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utilidades de Hash

        /// <summary>
        /// Genera un hash SHA256 de la contraseña
        /// Nota: En producción, considera usar BCrypt o Argon2 para mayor seguridad
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña</returns>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("X2")); // X2 para mayúsculas
                }
                return builder.ToString();
            }
        }

        #endregion
    }
}
