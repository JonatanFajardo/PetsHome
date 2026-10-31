using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_Recordatorios_DetailResult
    {
        public int rec_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string ResponsableContacto { get; set; }
        public string rec_Tipo { get; set; }
        public DateTime rec_FechaObjetivo { get; set; }
        public string rec_Estado { get; set; }
        public string rec_Nota { get; set; }
        public string rec_Origen { get; set; }
        public DateTime? rec_FechaCompletado { get; set; }
        public int DiasRestantes { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime rec_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? rec_FechaModifica { get; set; }
    }
}
