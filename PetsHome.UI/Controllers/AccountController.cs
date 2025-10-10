using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.DataAccess.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PetsHome.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioRepositoryAHM _usuarioRepository;
        private readonly HelpersServicesAHM _helpersServices;
        private readonly IConfiguration _configuration;

        public AccountController(
            UsuarioRepositoryAHM usuarioRepository,
            HelpersServicesAHM helpersServices,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _helpersServices = helpersServices;
        }

        public IActionResult Login()
        {
            HttpContext.Session.Remove("pantallas");
            return View();
        }

        public IActionResult SinAcceso()
        {
            return View();
        }

        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Generar hash de la contraseña igual que en AHM
                string hashedPassword = GenerateHash(model.usu_Contraseña);
                
                var usuario = _usuarioRepository.Login(hashedPassword, model.usu_NombreUsuario);
                
                if (usuario != null)
                {
                    // Cambiar estado a usuario logueado
                    _usuarioRepository.UsuarioLogIn(usuario.usu_Id);
                    
                    // Configurar variables de sesión idénticas a AHM
                    HttpContext.Session.SetString("usu_NombreUsuario", usuario.usu_NombreUsuario);

                    if (string.IsNullOrEmpty(usuario.usu_ImagenPerfil))
                    {
                        HttpContext.Session.SetString("usu_ImagenPerfil", "/images/users/avatar-1.jpg");
                    }
                    else
                    {
                        HttpContext.Session.SetString("usu_ImagenPerfil", usuario.usu_ImagenPerfil);
                    }

                    // Obtener pantallas del rol del usuario (igual que AHM)
                    string pantallas = String.Join(",", _helpersServices.ListadoPantallaForRol(usuario.rol_Id));

                    HttpContext.Session.SetString("pantallas", pantallas);
                    HttpContext.Session.SetInt32("idUsuario", usuario.usu_Id);
                    HttpContext.Session.SetInt32("idrol", usuario.rol_Id);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "El usuario o contraseña ingresados son incorrectos");
                }
                return View(model);
            }
            return View(model);
        }

        public IActionResult VaciarNoti()
        {
            var usuario = HttpContext.Session.GetInt32("idUsuario");
            
            // Cambiar estado a usuario deslogueado
            if (usuario.HasValue)
            {
                _usuarioRepository.UsuarioLogOut(Convert.ToInt32(usuario));
            }
            
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            var usuario = HttpContext.Session.GetInt32("idUsuario");
            
            // Cambiar estado a usuario deslogueado
            if (usuario.HasValue)
            {
                _usuarioRepository.UsuarioLogOut(Convert.ToInt32(usuario));
            }
            
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Generar hash SHA256 de la contraseña (compatible con AHM)
        /// </summary>
        private string GenerateHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computar hash - retorna array de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convertir array de bytes a string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}