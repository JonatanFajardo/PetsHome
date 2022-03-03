//using SixLabors.ImageSharp;
//using Image = SixLabors.ImageSharp.Image;
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
        //public static string ImageUrl(string[] extensions, IFormFile file, string direction, string folderName, string nameFile)
        //{
        //    string newExtension = "";

        //    //string[] extensions;
        //    //bool validateExtension = true;
        //    //foreach (var ext in extensions)
        //    //{
        //    //    if (file.ContentType != $"image/{ext}")
        //    //    {
        //    //        validateExtension = false;
        //    //    }

        //    //}
        //    if (!ValidateExtension(extensions, file))
        //    {
        //        string ext="";
        //        foreach (var item in extensions)
        //        {
        //            ext += $".{item} ";
        //        }
        //        string msj = $"Seleccione un archivo valido ({ext}).";
        //        return msj;
        //    }

        //    CreateFile.Path = direction;
        //    CreateFile.FolderName = folderName;
        //    CreateFile.Folder();

        //    CreateFile.DeleteFile(nameFile);

        //    string imageName = $"{folderName.ToLower().Trim().Replace(" ", "-")}";
        //    string newReturn = CreateFile.Image(imageName, file);
        //    return newReturn;
        //}



        //static Boolean ValidateExtension(string[] extensions, IFormFile file)
        //{
        //    bool validateExtension = true;
        //    if (extensions.Contains(file.ContentType))
        //    {
        //        return validateExtension = false;
        //    }
        //    return validateExtension;
        //}


        public static String GetName(IFormFile image)
        {
            //string imageName = "";
            var s = Convert.ToByte(image);

            if (image == null)
            {
                return string.Empty;
            }
            string ficheroImagen = Path.Combine(_hostEnvironment.WebRootPath, "images");
            string imageName = Guid.NewGuid().ToString() + image.Name;
            string ruta = Path.Combine(ficheroImagen, imageName);
            // Guardamos mediante la ruta completa la imagen en disco.
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
