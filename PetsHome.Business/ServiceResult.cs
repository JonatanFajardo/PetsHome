using System.Text.Json.Serialization;

namespace PetsHome.Business
{
    /// <summary>
    /// Objeto que encapsula el resultado de una solicitud en la capa de negocios.
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// Indica el tipo de solicitud.
        /// </summary>
        [JsonIgnore]
        public ServiceResultType Type { get; set; }

        /// <summary>
        /// El codigo de respuesta de la solicitud (utilizado en el response JSON del API).
        /// </summary>
        public int Code => (int)Type;

        /// <summary>
        /// Indica si la solicitud fue o no exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// El mensaje retornado por la capa de negocios.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// El objeto retornado por la capa de negocios.
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// Resultado de servicio cuando la operacion se ha realizado exitosamente (200).
        /// </summary>
        /// <param name="data">El objeto de respuesta que se pasa a la siguiente capa del sistema.</param>
        /// <returns></returns>
        public ServiceResult Ok(object data = null)
        {
            Success = true;
            Data = data;
            return SetMessage("Operación completada exitosamente.", ServiceResultType.Success);
        }

        /// <summary>
        /// Resultado de servicio cuando la operacion se ha realizado exitosamente (200).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="data">El objeto de respuesta que se pasa a la siguiente capa del sistema.</param>
        /// <returns></returns>
        public ServiceResult Ok(string message, object data = null)
        {
            Success = true;
            Data = data;
            return SetMessage(message, ServiceResultType.Success);
        }

        /// <summary>
        /// Resultado de servicio con un mensaje informativo (100).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult Info(string message, bool success = true)
        {
            Success = success;
            return SetMessage(message, ServiceResultType.Info);
        }

        /// <summary>
        /// Resultado de servicio cuando ocurre una advertencia (202).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult Warning(string message, bool success = true)
        {
            Success = success;
            return SetMessage(message, ServiceResultType.Warning);
        }

        /// <summary>
        /// Resultado de servicio cuando ocurre un error (500).
        /// </summary>
        /// <returns></returns>
        public ServiceResult Error()
        {
            return Error("Se ha producido un error al procesar la solicitud. Si el problema persiste, comuníquese con el administrador del sistema.");
        }

        /// <summary>
        /// Resultado de servicio cuando ocurre un error en la operacion (500).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <returns></returns>
        public ServiceResult Error(string message)
        {
            Success = false;
            return SetMessage(message, ServiceResultType.Error);
        }

        /// <summary>
        /// Resultado de servicio cuando no existe el objecto o recurso (404).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult NotFound(string message = "Objeto no encontrado.", bool success = false)
        {
            Success = success;
            return SetMessage(message, ServiceResultType.NotFound);
        }

        /// <summary>
        /// Resultado de servicio cuando un objeto o recurso no es aceptado (406).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="data">Los datos a pasar en la respuesta.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult NotAcceptable(string message = "Inaceptable.", dynamic data = null, bool success = false)
        {
            Success = success;
            Data = data;
            return SetMessage(message, ServiceResultType.NotAcceptable);
        }

        /// <summary>
        /// Resultado de servicio cuando existe un conflicto, por ejemplo objecto repetido (409).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="data">Los datos a pasar en la respuesta.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult Conflict(string message = "Se ha detectado un conflicto con el objeto.", dynamic data = null, bool success = false)
        {
            Success = success;
            Data = data;
            return SetMessage(message, ServiceResultType.Conflict);
        }

        /// <summary>
        /// Resultado de servicio cuando la solicitud no esta autorizada (401).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult Unauthorized(string message = "Acceso no autorizado al objeto.", bool success = false)
        {
            Success = success;
            return SetMessage(message, ServiceResultType.Unauthorized);
        }

        /// <summary>
        /// Resultado de servicio cuando la solicitud es denegada (403).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult Forbidden(string message = "Prohibido el acceso al objeto.", bool success = false)
        {
            Success = success;
            return SetMessage(message, ServiceResultType.Forbidden);
        }

        /// <summary>
        /// Resultado de servicio cuando la solicitud se realiza en un formato incorrecto (400).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult BadRequest(string message = "Bad request.", bool success = false)
        {
            Success = success;
            return SetMessage(message, ServiceResultType.BadRequest);
        }

        /// <summary>
        /// Resultado de servicio cuando un objeto o recurso esta deshabilidato (410).
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="data">Los datos a pasar en la respuesta..</param>
        /// <param name="success">Si el resultado del servicio se debe marcar como exitoso o no.</param>
        /// <returns></returns>
        public ServiceResult Disabled(string message = "Disabled resource.", dynamic data = null, bool success = false)
        {
            Success = success;
            Data = data;
            return SetMessage(message, ServiceResultType.Disabled);
        }

        /// <summary>
        /// Resultado de servicio con mensaje y tipo personalizado.
        /// </summary>
        /// <param name="message">Mensaje personalizado que se pasa a la siguiente capa del sistema.</param>
        /// <param name="serviceResultType">El tipo de resultado a retornar.</param>
        /// <returns></returns>
        public ServiceResult SetMessage(string message, ServiceResultType serviceResultType)
        {
            Message = message;
            Type = serviceResultType;
            return this;
        }

        public ServiceResult()
        {
            Success = false;
            Type = ServiceResultType.Error;
        }
    }

    /// <summary>
    /// Tipos de resultados que puede generar un servicio.
    /// </summary>
    public enum ServiceResultType
    {
        Info = 100,
        Success = 200,
        Warning = 202,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        NotAcceptable = 406,
        Conflict = 409,
        Disabled = 410,
        Error = 500
    }
}
