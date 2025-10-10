using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using PetsHome.DataAccess.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Image = SixLabors.ImageSharp.Image;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio de helpers compatible con AHM_INSTA_HELP_ADM
    /// Mantiene la misma interfaz y funcionalidad que HelpersServices de AHM
    /// </summary>
    public class HelpersServicesAHM
    {
        private readonly UsuarioRepositoryAHM _usuarioRepository;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly RolesRepositoryAHM _rolesRepository;

        public HelpersServicesAHM(
            UsuarioRepositoryAHM usuarioRepository, 
            IHostEnvironment hostEnvironment,
            RolesRepositoryAHM rolesRepository)
        {
            _usuarioRepository = usuarioRepository;
            _hostEnvironment = hostEnvironment;
            _rolesRepository = rolesRepository;
        }

        /// <summary>
        /// Obtener listado de pantallas por rol como lista de strings (compatible con AHM)
        /// </summary>
        public List<string> ListadoPantallaForRol(int rol_Id)
        {
            var listado = _rolesRepository.ListPantallas(rol_Id);
            var listadoString = new List<string>();
            
            try
            {
                foreach (var item in listado)
                {
                    listadoString.Add(item.modpt_Descripcion);
                }
                return listadoString;
            }
            catch (Exception)
            {
                return listadoString;
            }
        }

        /// <summary>
        /// Obtener listado de pantallas por usuario como lista de strings
        /// </summary>
        public List<string> ListadoPantallaForUsuario(int usu_Id)
        {
            var listado = _usuarioRepository.GetPantallasPorUsuario(usu_Id);
            var listadoString = new List<string>();
            
            try
            {
                foreach (var item in listado)
                {
                    listadoString.Add(item.modpt_Descripcion);
                }
                return listadoString;
            }
            catch (Exception)
            {
                return listadoString;
            }
        }

        /// <summary>
        /// Actualizar imagen de perfil (compatible con AHM)
        /// </summary>
        public string UpdateImagenPerfil(IFormFile file, int usu_Id, string nombre, string imagenPerfil)
        {
            string oldFile = "";
            string newExtension = "";
            try
            {
                if (file.ContentType != "image/jpeg" &&
                    file.ContentType != "image/jpg" &&
                    file.ContentType != "image/png" &&
                    file.ContentType != "image/gif")
                {
                    return ("Seleccione un archivo válido (.jpg, .png, .gif).");
                }

                string root = _hostEnvironment.ContentRootPath;
                string imagePath = $"{root}\\wwwroot\\images\\usuarios-images";
                string imageName = $"{nombre.ToLower().Trim().Replace(" ", "-")}";

                // Crear directorio si no existe
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                if (!string.IsNullOrEmpty(imagenPerfil))
                {
                    oldFile = $"{root}{imagenPerfil.Replace("/", "\\")}";
                }

                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }

                using (var imagenStream = file.OpenReadStream())
                {
                    using (var img = Image.Load(imagenStream))
                    {
                        img.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Min,
                            Size = new SixLabors.ImageSharp.Size(width: 500, height: 500)
                        }));

                        newExtension = Path.GetExtension(file.FileName);
                        if (newExtension == ".png")
                        {
                            img.Save($"{imagePath}\\{imageName}{newExtension}", new PngEncoder());
                        }
                        else if (newExtension == ".gif")
                        {
                            img.Save($"{imagePath}\\{imageName}{newExtension}", new GifEncoder());
                        }
                        else
                        {
                            img.Save($"{imagePath}\\{imageName}{newExtension}", new JpegEncoder { Quality = 70 });
                        }

                        imagenPerfil = $"/images/usuarios-images/{imageName}{newExtension}";
                    }
                }

                return (imagenPerfil);
            }
            catch (Exception ex)
            {
                return ("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Enviar correo electrónico (compatible con AHM)
        /// </summary>
        public string EnviarCorreo(string to, string subject, string client, string message)
        {
            string body = "<!DOCTYPE html>" +
                "<html lang='es'>" +
                "<head>" +
                "<meta charset='UTF-8'>" +
                "<meta http-equiv='X-UA-Compatible' content='IE=edge'>" +
                "<meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
                "<title>Correo PetsHome</title>" +
                "</head>" +
                "<body>" +
                "<div style='width:90%; display:flex; align-items:center; padding:20px;'>" +
                "<div style='margin:0px auto; border: solid #e4e4e4 1px; padding:20px; border-radius:20px; min-width:55%; max-width:100%; background-color: #f8f9fa;'>" +
                "<div style='display:flex;padding:30px;'>" +
                "<div style='margin:0px auto; width: 120px; height: 120px; background-color: #007bff; border-radius: 50%; display: flex; align-items: center; justify-content: center;'>" +
                "<span style='color: white; font-size: 48px;'>🐾</span>" +
                "</div>" +
                "</div>" +
                "<div style='font-family: Arial, Helvetica, sans-serif; font-weight:300; color:rgb(127, 128, 129); padding:40px; text-align: justify;'>";
            body += $"<p style='font-family: Arial, Helvetica, sans-serif; font-weight: 300; color:rgb(127, 128, 129);'>Hola <b>{client},</b><br><br> Te saluda el equipo de PetsHome, para informarte que: <br><br> {message} <br><br>Saludos.</p>";
            body += "</div><div style='width:100%; display:flex; text-align:center;'>" +
                "<b style='color: rgb(170, 170, 172); font-family: Arial, Helvetica, sans-serif; font-size: 12px; padding: 30px; margin:0px auto;'>" +
                "2025 © PetsHome - Sistema de Gestión de Refugios" +
                "</b>" +
                "</div>" +
                "</div>" +
                "</div>" +
                "</body>" +
                "</html>";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@petshome.com", "PetsHome System");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            try
            {
                // Configuración básica SMTP - ajustar según necesidades
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("tu_email@gmail.com", "tu_password");
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtpClient.Send(mail);
                    smtpClient.Dispose();
                }

                return "Exito";
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        /// <summary>
        /// Validar si el usuario tiene acceso a una pantalla específica
        /// </summary>
        public bool TieneAccesoPantalla(int usu_Id, string pantallaNombre)
        {
            try
            {
                var pantallas = ListadoPantallaForUsuario(usu_Id);
                return pantallas.Any(p => p.Equals(pantallaNombre, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Obtener imagen de perfil por defecto
        /// </summary>
        public string GetImagenPerfilDefault()
        {
            return "/images/users/avatar-default.jpg";
        }
    }
}