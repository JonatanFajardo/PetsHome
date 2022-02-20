using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Reflection;
using PetsHome.Logic.Repositories;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Extensions
{
    public class EventLogger
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// El ID del usuario del sistema que realiza el evento.        
        /// </summary>
        public static int? UserId { get; set; } = null;

        /// <summary>
        /// La dirección IP del usuario que realiza el evento.
        /// </summary>
        public static string Evt_DireccionIP { get; set; }

        /// <summary>
        /// El user agent del explorador del usuario que realiza el evento.
        /// </summary>
        public static string UserAgent { get; set; }

        /// <summary>
        /// Hace un registro de evento personalizado utilizando parametros.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="details"></param>
        /// <param name="previousState"></param>
        /// <param name="newState"></param>
        public static void Write(
            EventLogType eventType = EventLogType.None,
            string details = null,
            object previousState = null,
            object newState = null)
        {
            try
            {
                if (UserId < 1)
                    UserId = null;

                //TODO: Check reference loop, and serialization of nested objects
                var serializeSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CustomJsonResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None
                };

                var eventLog = new tbRegistroEventos
                {
                    Evt_DireccionIP = Evt_DireccionIP,
                    Evt_UserAgent = UserAgent,
                    Evt_Detalles = details,
                    Tpevt_Id = (int)eventType,
                    Evt_EstadoAnterior = previousState != null ? JsonConvert.SerializeObject(previousState, serializeSettings) : null,
                    Evt_NuevoEstado = newState != null ? JsonConvert.SerializeObject(newState, serializeSettings) : null,
                    Evt_Usu_Id = UserId
                };

                var repository = new EventoLoggerRepository();
                repository.Insert(eventLog);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        /// <summary>
        /// Hace un registro de evento para la visualización o lectura de objetos.
        /// </summary>
        /// <param name="details"></param>
        public static void View(string details)
        {
            Write(EventLogType.View, details, null, null);
        }

        /// <summary>
        /// Hace un registro de evento para la visualización o lectura de objetos.
        /// </summary>
        /// <param name="details"></param>
        public static void Search(string details)
        {
            Write(EventLogType.Search, details, null, null);
        }


        /// <summary>
        /// Hace un registro de evento para creación de objetos.
        /// </summary>
        /// <param name="details"></param>     
        /// <param name="newState"></param>
        public static void Create(string details, object newState = null)
        {
            Write(EventLogType.Create, details, null, newState);
        }

        /// <summary>
        /// Hace un registro de evento para actualización de objetos.
        /// </summary>
        /// <param name="details"></param> 
        /// <param name="previousState"></param>
        /// <param name="newState"></param>
        public static void Update(string details, object previousState = null, object newState = null)
        {
            Write(EventLogType.Update, details, previousState, newState);
        }

        /// <summary>
        /// Hace un registro de evento para actualización de objetos.
        /// </summary>
        /// <param name="details"></param>        
        public static void PasswordChange(string details)
        {
            Write(EventLogType.PasswordChange, details);
        }

        /// <summary>
        /// Hace un registro de evento para eliminación de objetos.
        /// </summary>
        /// <param name="details"></param>        
        public static void Delete(string details, object previousState = null)
        {
            Write(EventLogType.Delete, details, previousState, null);
        }

        /// <summary>
        /// Hace un registro de evento para actualización de objetos.
        /// </summary>
        /// <param name="details"></param>        
        public static void Login(string details)
        {
            Write(EventLogType.Login, details);
        }

        /// <summary>
        /// Hace un registro de evento para actualización de objetos.
        /// </summary>
        /// <param name="details"></param>        
        public static void Logout(string details)
        {
            Write(EventLogType.Logout, details);
        }

        /// <summary>
        /// Hace un registro de evento para actualización de objetos.
        /// </summary>
        /// <param name="details"></param>        
        public static void UpdateState(string details)
        {
            Write(EventLogType.Update, details);
        }


        /// <summary>
        /// Reinicia las propiedades de un envento de log, para evitar duplicidad.
        /// </summary>
        public static void ResetFields()
        {
            UserId = null;
            Evt_DireccionIP = null;
            UserAgent = null;
        }
    }

    public enum EventLogType
    {
        None = 0,
        Login = 1,
        Logout = 2,
        View = 3,
        Create = 4,
        Update = 5,
        Delete = 6,
        PasswordChange = 7,
        Search = 8
    }

    public class CustomJsonResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            if (prop.PropertyType.IsClass &&
                prop.PropertyType != typeof(string))
            {
                prop.ShouldSerialize = obj => false;
            }

            return prop;
        }
    }
}
