namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Picker de servicios facturables por categoria (Vacuna/Laboratorio/Procedimiento/Consulta).
    /// Lo consume el endpoint GET /Consulta/Servicios.
    /// </summary>
    public partial class PR_Veterinaria_Servicio_DropdownResult
    {
        public int serv_Id { get; set; }
        public string serv_Nombre { get; set; }
        public decimal serv_Precio { get; set; }
        public string serv_Categoria { get; set; }
    }
}
