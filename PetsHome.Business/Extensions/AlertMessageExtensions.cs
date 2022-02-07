namespace PetsHome.Business.Extensions
{
    public class AlertMessageExtensions
    {
        public AlertMessageExtensions() { }
        public string Text { get; set; }
        public string CssClass { get; set; }

        private AlertMessageType _type;
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

    public class AlertMessaje
    {
        public static string Error { get; set; }
        public static string SuccessSave { get; set; }
        public static string SuccessEdit { get; set; }
        public static string SuccessDelete { get; set; }
        public AlertMessaje()
        {
            Error = "Parece haber ocurrido un problema.";
            SuccessSave = "Registro guardado correctamente.";
            SuccessEdit = "Registro editado correctamente.";
            SuccessDelete = "Registro eliminado correctamente.";
        }
    }
}
