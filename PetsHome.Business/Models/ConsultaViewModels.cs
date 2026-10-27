using System;
using System.Collections.Generic;
using System.Linq;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// ViewModel de la pantalla "En consulta" (cabina de trabajo del veterinario).
    /// Reutiliza PerfilMedicoViewModel para poblar la ficha del paciente,
    /// el historial rapido y las banderas rojas (meds activos + vacunas vencidas).
    /// </summary>
    public class ConsultaViewModel
    {
        public int MascId { get; set; }

        /// <summary>
        /// Visita en curso proveniente de la sala de espera (si aplica). El front
        /// la reenvia al Finalizar para cerrar esa cita en vez de crear una nueva.
        /// 0 = consulta directa sin visita encolada.
        /// </summary>
        public int CitaId { get; set; }

        /// <summary>Datos medicos del paciente (reutiliza PerfilMedicoService).</summary>
        public PerfilMedicoViewModel Perfil { get; set; } = new PerfilMedicoViewModel();

        /// <summary>Tipos de consulta para el dropdown del flujo de trabajo.</summary>
        public List<TipoConsultaViewModel> TiposConsulta { get; set; } = new List<TipoConsultaViewModel>();

        /// <summary>
        /// True cuando no se recibio un paciente (mascId &lt;= 0): la pantalla
        /// muestra el ESTADO VACIO para elegir a quien atender, sin cargar ficha.
        /// </summary>
        public bool SinPaciente { get; set; }

        /// <summary>
        /// Cola de la sala de espera del dia. Alimenta el estado vacio (elegir
        /// paciente) y el panel lateral cuando ya hay un paciente cargado.
        /// </summary>
        public List<SalaEsperaItemViewModel> SalaEspera { get; set; } = new List<SalaEsperaItemViewModel>();

        // ── Accesos rapidos a la ficha ──────────────────────────────
        public PR_Medico_PerfilMedico_FichaMascotaResult Ficha =>
            Perfil?.FichaMascota?.FirstOrDefault();

        public string NombreMascota => Ficha?.masc_Nombre ?? "Mascota";

        // ── Banderas rojas (datos reales existentes) ────────────────
        /// <summary>Medicamentos activos en curso.</summary>
        public List<PR_Medico_PerfilMedico_MedicamentosActivosResult> MedicamentosActivos =>
            Perfil?.MedicamentosActivos ?? new List<PR_Medico_PerfilMedico_MedicamentosActivosResult>();

        /// <summary>Vacunas con estado "Vencida".</summary>
        public List<PR_Medico_PerfilMedico_VacunasResult> VacunasVencidas =>
            (Perfil?.Vacunas ?? new List<PR_Medico_PerfilMedico_VacunasResult>())
                .Where(v => (v.EstadoVacuna ?? "") == "Vencida")
                .ToList();

        public int TotalBanderasRojas => MedicamentosActivos.Count + VacunasVencidas.Count;
    }

    /// <summary>
    /// Payload que envia el flujo AJAX al Finalizar la consulta.
    /// </summary>
    public class GuardarConsultaDto
    {
        public int MascId { get; set; }

        /// <summary>
        /// Visita existente (proveniente de la sala de espera) que esta consulta
        /// finaliza. Si es &gt; 0 no se crea una cita nueva: se cierra esta.
        /// </summary>
        public int CitaId { get; set; }

        public int? TipoConsultaId { get; set; }
        public string Motivo { get; set; }
        public string Diagnostico { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Temperatura { get; set; }

        // ── Signos vitales (bloque Objetivo del formato SOAP) ──────────
        /// <summary>Frecuencia cardiaca (latidos por minuto). Columna ya existente.</summary>
        public int? FrecuenciaCardiaca { get; set; }

        /// <summary>Frecuencia respiratoria (respiraciones por minuto). Columna ya existente.</summary>
        public int? FrecuenciaRespiratoria { get; set; }

        /// <summary>Color de mucosas (enum sin tildes: Rosadas/Palidas/Ictericas/Cianoticas/Congestivas).</summary>
        public string Mucosas { get; set; }

        /// <summary>Tiempo de llenado capilar en segundos.</summary>
        public decimal? LlenadoCapilar { get; set; }

        /// <summary>Escala de dolor 0 a 10.</summary>
        public int? EscalaDolor { get; set; }

        /// <summary>Condicion corporal (BCS) 1 a 9.</summary>
        public decimal? CondicionCorporal { get; set; }

        /// <summary>Vacuna aplicada en la consulta (se persiste en la cita).</summary>
        public int? VacunaId { get; set; }

        /// <summary>Lineas de medicamentos recetados/aplicados (ver TODO de persistencia por linea).</summary>
        public List<MedicamentoLineaDto> Medicamentos { get; set; } = new List<MedicamentoLineaDto>();

        public DateTime? ProximaCita { get; set; }
        public string MotivoProximaCita { get; set; }
    }

    /// <summary>
    /// Linea de medicamento seleccionada con el componente SearchSelect.
    /// </summary>
    public class MedicamentoLineaDto
    {
        /// <summary>Id del item de inventario (0 = medicamento de texto libre).</summary>
        public int Id { get; set; }

        public string Nombre { get; set; }

        /// <summary>True si proviene del catalogo de inventario (se vincula itm_Id).</summary>
        public bool EnStock { get; set; }

        public string Dosis { get; set; }

        public string Frecuencia { get; set; }

        /// <summary>Duracion del tratamiento (ej. "7 dias").</summary>
        public string Duracion { get; set; }

        /// <summary>True si la doctora marco entregar este medicamento del inventario (descuenta stock).</summary>
        public bool Entregar { get; set; }

        /// <summary>Cantidad a entregar/descontar del inventario. Valor &lt;= 0 se trata como 1.</summary>
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Resultado de la operacion Guardar.
    /// </summary>
    public class GuardarConsultaResultDto
    {
        public bool Ok { get; set; }
        public int CitaId { get; set; }
        public string Mensaje { get; set; }

        /// <summary>
        /// Medicamentos que no pudieron entregarse por falta de stock. Si tiene
        /// elementos, no se creo nada (cita/recetas/salidas) y Ok es false.
        /// </summary>
        public List<FaltanteStockDto> Faltantes { get; set; } = new List<FaltanteStockDto>();
    }

    /// <summary>
    /// Detalle de un medicamento que no alcanza el stock solicitado.
    /// </summary>
    public class FaltanteStockDto
    {
        public string Nombre { get; set; }
        public int Disponible { get; set; }
        public int Pedido { get; set; }
    }
}
