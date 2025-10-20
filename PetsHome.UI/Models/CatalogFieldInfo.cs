namespace PetsHome.UI.Models
{
    /// <summary>
    /// Clase auxiliar para pasar información de campos a la vista compartida de catálogos
    /// </summary>
    public class CatalogFieldInfo
    {
        public string Icon { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsLarge { get; set; }
        public bool IsBadge { get; set; }
        public string BadgeClass { get; set; }
    }
}