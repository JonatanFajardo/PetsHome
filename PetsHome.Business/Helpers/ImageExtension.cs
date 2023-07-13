using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PetsHome.Business.Helpers
{
    public static class ImageExtension
    {
        private static IWebHostEnvironment _hostEnvironment;

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="extensions">Extensiones permitidas.</param>
        ///// <param name="file">Imagen recibida.</param>
        ///// <param name="direction">Ruta de almacenamiento.</param>
        ///// <param name="folderName">Nombre de la carpeta.</param>
        ///// <param name="nameFile">Nombre del archivo</param>
        ///// <returns></returns>
        public static String GetName(IFormFile image)
        {
            if (image == null)
            {
                return string.Empty;
            }
            string ficheroImagen = Path.Combine(_hostEnvironment.WebRootPath, "images");
            string imageName = Guid.NewGuid().ToString() + image.Name;
            string ruta = Path.Combine(ficheroImagen, imageName);
            image.CopyTo(new FileStream(ruta, FileMode.Create));
            return imageName;
        }

        /// <summary>
        /// Convierte imagenes a un array de byts.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetBytesAsync(this IFormFile formFile)
        {
            if (formFile != null)
            {
                if (formFile.Length > 1)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        return stream.ToArray();
                    }
                }
            }
            return null;
        }

        public static string GetImage(this byte[] byteData)
        {
            string base64Data = Convert.ToBase64String(byteData);
            return base64Data;
        }
    }
}