using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace PetsHome.Business.Helpers
{
    public static class CreateFile
    {
        private static string _path;
        private static string _folderName;

        public static string Path { set => _path = value; }
        public static string FolderName { set => _folderName = value; }

        /// <summary>
        /// Crea un nuevo folder en la ruta especificada.
        /// </summary>
        /// <param name="path">Ruta en la cual se creara la carpeta.</param>
        /// <returns>
        /// Retorna
        /// 0 Si la carpeta se creo.
        /// 1 No pudo crearse.
        /// 2 Ya existe.
        /// </returns>
        /// <summary>
        /// Limpia las propiedades.
        /// </summary>
        /// <remarks>
        /// Prepara para crear otro archivo.
        /// </remarks>
        public static void Clear()
        {
            _path = "";
            _folderName = "";
        }

        /// <summary>
        /// Crea un nuevo folder en la ruta especificada.
        /// </summary>
        /// <remarks>
        /// Requisito: Debe de llenarse la propiedad Path antes de utilizar el metodo.
        /// </remarks>
        /// <returns>
        /// Retorna
        /// 0 Si la carpeta se creo.
        /// 1 No pudo crearse, occurrio un problema.
        /// 2 Ya existe.
        /// </returns>
        public static int Folder()
        {
            string path = $"{_path}/{_folderName}";
            if (path != null)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Crea un archivo.
        /// </summary>
        /// <remarks>
        /// Requiere: Necesita llenar anticipadamente las propiedades Path y FolderName para su correcto funcionamiento.
        /// </remarks>
        /// <param name="name">Nombre del archivo.</param>
        /// <param name="extension">Tipo de extension.</param>
        /// <param name="writer">Contenido a guardar.</param>
        /// <returns></returns>
        public static int File(string name, string extension, string writer = "")
        {
            string path = $"{_path}/{_folderName}";
            StreamWriter streamWriter = new StreamWriter($"{path}/{name}.{extension}", true);
            if (writer != "")
            {
                streamWriter.Write(writer);
            }
            streamWriter.Flush();
            streamWriter.Close();
            return 0;
        }

        /// <summary>
        /// Crea una imagen.
        /// </summary>
        /// <remarks>
        /// Aplica configuraciones personalizadas a la imagen.
        /// </remarks>
        /// <param name="name">Nombre de la imagen a crear.</param>
        /// <param name="file">Datos de la imagen seleccionada.</param>
        /// <param name="extension">Tipo de extension.</param>
        /// <returns>Retorna una cadena string con la nueva ruta de almacenamiento.</returns>
        public static string Image(string name, IFormFile file)
        {
            string path = $"{_path}/{_folderName}";

            string newExtension = "";
            string newDirection = "";

            try
            {
                using (var imagenStream = file.OpenReadStream())
                {
                    using (var img = SixLabors.ImageSharp.Image.Load(imagenStream))
                    {
                        img.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Min,
                            Size = new SixLabors.ImageSharp.Size(width: 500, height: 500)
                        }));

                        newExtension = System.IO.Path.GetExtension(file.FileName);
                        string ruta = $"{path}/{name}{newExtension}";
                        switch (newExtension)
                        {
                            case ".png":
                                img.Save(ruta, new PngEncoder());
                                break;

                            case ".jpg":
                                img.Save(ruta, new JpegEncoder { Quality = 70 });
                                break;

                            case ".jpeg":
                                img.Save(ruta, new JpegEncoder { Quality = 70 });
                                break;

                            case ".gif":
                                img.Save(ruta, new GifEncoder());
                                break;
                        }

                        return newDirection = $"{path}\\{name}{newExtension}";
                    }
                }
            }
            catch (Exception error)
            {
                newDirection = $"Error al guardar la imagen: {error.Message}";
                return newDirection;
            }
        }

        public enum Extension
        {
            png,
            jpgandjpeg,
            gif
        }

        /// <summary>
        /// Elimina un archivo en la ruta especificada.
        /// </summary>
        /// <remarks>
        /// Requisito: Debe de llenarse la propiedad Path antes de utilizar el metodo.
        /// </remarks>
        /// <returns>
        /// Retorna
        /// 0 Si el archivo se elimino.
        /// 1 No pudo eliminarse, occurrio un problema.
        /// 2 Ya existe.
        /// </returns>
        public static int DeleteFile(string name)
        {
            string path = $"{_path}/{_folderName}/{name}";
            if (name != null)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }
    }
}