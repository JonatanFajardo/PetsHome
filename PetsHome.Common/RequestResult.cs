namespace PetsHome.Common
{
    /// <summary>
    /// Resultado estándar de operaciones de escritura en base de datos.
    /// </summary>
    public class RequestResult
    {
        public int CodeStatus { get; set; }
        public string MessageStatus { get; set; }

        /// <summary>
        /// True cuando CodeStatus > 0 (operación exitosa).
        /// </summary>
        public bool Success => CodeStatus > 0;

        public static RequestResult Error(string message) =>
            new RequestResult { CodeStatus = -5, MessageStatus = message };
    }
}
