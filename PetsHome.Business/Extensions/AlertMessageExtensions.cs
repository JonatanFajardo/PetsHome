﻿namespace PetsHome.Business.Extensions
{
    /// <summary>
    /// Objeto para mostrar mensajes de alerta en la vista
    /// </summary>
    public class AlertMessageExtensions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AlertMessageExtensions()
        { }

        /// <summary>
        /// Propiedad para el texto del mensaje
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Propiedad para la clase css del mensaje
        /// </summary>
        public string CssClass { get; set; }

        private AlertMessageType _type;

        /// <summary>
        /// Propiedad para el tipo de mensaje
        /// </summary>
        public AlertMessageType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                CssClass = Type
                    switch
                {
                    AlertMessageType.Success => "success",
                    AlertMessageType.Warning => "warning",
                    AlertMessageType.Error => "error",
                    _ => "info",
                };
            }
        }

        public AlertMessageExtensions(string text, AlertMessageType type)
        {
            Text = text;
            Type = type;
        }
    }

    public enum AlertMessageType
    {
        Success = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>
    /// Mensajes de alerta
    /// </summary>
    public class AlertMessaje
    {
        public static string Error = "Parece haber ocurrido un problema.";
        public static string SuccessSave = "Registro guardado correctamente.";
        public static string SuccessEdit = "Registro editado correctamente.";
        public static string SuccessDelete = "Registro eliminado correctamente.";
    }
}