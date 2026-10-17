namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado de [Seguridad].[PR_Seguridad_Veterinarios_Dropdown].
    /// Usuarios con rol 4 (Veterinario) activos: id + nombre.
    /// Alimenta el selector "Veterinario (opcional)" del check-in de recepcion.
    /// </summary>
    public class PR_Seguridad_Veterinarios_DropdownResult
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
