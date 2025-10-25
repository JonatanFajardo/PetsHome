namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para representar los registros del listado de mascotas.
    /// </summary>
    public class MascotaListViewModel
    {
        public int masc_Id { get; set; }
        public long? masc_Fila { get; set; }
        public string masc_Nombre { get; set; }
        public string raza_Descripcion { get; set; }
        public string refg_Nombre { get; set; }
        public bool? masc_EsAdoptado { get; set; }
        public bool? masc_EsReservado { get; set; }
        public byte[] masc_Imagen { get; set; }
    }
}
