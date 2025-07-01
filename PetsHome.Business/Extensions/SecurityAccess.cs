namespace PetsHome.Business.Extensions
{
    /// <summary>
    /// Clase que contiene las acciones de seguridad.
    /// </summary>
    public class SecurityAccess
    {
        #region EsquemaGeneral

        /// <summary>
        /// Acción: Listado de municipios.
        /// </summary>
        public const string ListMunicipios = "Listado_Municipios";

        /// <summary>
        /// Acción: Registro de municipios.
        /// </summary>
        public const string InsertMunicipios = "Registro_Municipios";

        /// <summary>
        /// Acción: Modificación de municipios.
        /// </summary>
        public const string EditMunicipios = "Modificar_Municipios";

        /// <summary>
        /// Acción: Eliminación de municipios.
        /// </summary>
        public const string DeleteMunicipios = "Eliminar_Municipios";

        /// <summary>
        /// Acción: Listado de personas.
        /// </summary>
        public const string ListPersonas = "Listado_Personas";

        /// <summary>
        /// Acción: Registro de personas.
        /// </summary>
        public const string InsertPersonas = "Registro_Personas";

        /// <summary>
        /// Acción: Modificación de personas.
        /// </summary>
        public const string EditPersonas = "Modificar_Personas";

        /// <summary>
        /// Acción: Eliminación de personas.
        /// </summary>
        public const string DeletePersonas = "Eliminar_Personas";

        /// <summary>
        /// Acción: Listado de departamentos.
        /// </summary>
        public const string ListDepartamentos = "Listado_Departamentos";

        /// <summary>
        /// Acción: Registro de departamentos.
        /// </summary>
        public const string InsertDepartamentos = "Registro_Departamentos";

        /// <summary>
        /// Acción: Modificación de departamentos.
        /// </summary>
        public const string EditDepartamentos = "Modificar_Departamentos";

        /// <summary>
        /// Acción: Eliminación de departamentos.
        /// </summary>
        public const string DeleteDepartamentos = "Eliminar_Departamentos";

        #endregion EsquemaGeneral

        #region EsquemaInventario

        /// <summary>
        /// Acción: Listado de categorías.
        /// </summary>
        public const string ListCategorias = "Listado_Categorias";

        /// <summary>
        /// Acción: Registro de categorías.
        /// </summary>
        public const string InsertCategorias = "Registro_Categorias";

        /// <summary>
        /// Acción: Modificación de categorías.
        /// </summary>
        public const string EditCategorias = "Modificar_Categorias";

        /// <summary>
        /// Acción: Eliminación de categorías.
        /// </summary>
        public const string DeleteCategorias = "Eliminar_Categorias";

        /// <summary>
        /// Acción: Listado de entradas.
        /// </summary>
        public const string ListEntradas = "Listado_Entradas";

        /// <summary>
        /// Acción: Registro de entradas.
        /// </summary>
        public const string InsertEntradas = "Registro_Entradas";

        /// <summary>
        /// Acción: Modificación de entradas.
        /// </summary>
        public const string EditEntradas = "Modificar_Entradas";

        /// <summary>
        /// Acción: Eliminación de entradas.
        /// </summary>
        public const string DeleteEntradas = "Eliminar_Entradas";

        /// <summary>
        /// Acción: Listado de detalles de entradas.
        /// </summary>
        public const string ListEntradasDetalles = "Listado_EntradasDetalles";

        /// <summary>
        /// Acción: Registro de detalles de entradas.
        /// </summary>
        public const string InsertEntradasDetalles = "Registro_EntradasDetalles";

        /// <summary>
        /// Acción: Modificación de detalles de entradas.
        /// </summary>
        public const string EditEntradasDetalles = "Modificar_EntradasDetalles";

        /// <summary>
        /// Acción: Eliminación de detalles de entradas.
        /// </summary>
        public const string DeleteEntradasDetalles = "Eliminar_EntradasDetalles";

        /// <summary>
        /// Acción: Listado de inventarios.
        /// </summary>
        public const string ListInventarios = "Listado_Inventarios";

        /// <summary>
        /// Acción: Registro de inventarios.
        /// </summary>
        public const string InsertInventarios = "Registro_Inventarios";

        /// <summary>
        /// Acción: Modificación de inventarios.
        /// </summary>
        public const string EditInventarios = "Modificar_Inventarios";

        /// <summary>
        /// Acción: Eliminación de inventarios.
        /// </summary>
        public const string DeleteInventarios = "Eliminar_Inventarios";

        /// <summary>
        /// Acción: Listado de detalles de inventarios.
        /// </summary>
        public const string ListInventariosDetalles = "Listado_InventariosDetalles";

        /// <summary>
        /// Acción: Registro de detalles de inventarios.
        /// </summary>
        public const string InsertInventariosDetalles = "Registro_InventariosDetalles";

        /// <summary>
        /// Acción: Modificación de detalles de inventarios.
        /// </summary>
        public const string EditInventariosDetalles = "Modificar_InventariosDetalles";

        /// <summary>
        /// Acción: Eliminación de detalles de inventarios.
        /// </summary>
        public const string DeleteInventariosDetalles = "Eliminar_InventariosDetalles";

        /// <summary>
        /// Acción: Listado de items.
        /// </summary>
        public const string ListItems = "Listado_Items";

        /// <summary>
        /// Acción: Registro de items.
        /// </summary>
        public const string InsertItems = "Registro_Items";

        /// <summary>
        /// Acción: Modificación de items.
        /// </summary>
        public const string EditItems = "Modificar_Items";

        /// <summary>
        /// Acción: Eliminación de items.
        /// </summary>
        public const string DeleteItems = "Eliminar_Items";

        #endregion EsquemaInventario

        #region EsquemaRefugio

        /// <summary>
        /// Acción: Listado de mascotas.
        /// </summary>
        public const string ListMascotas = "Listado_Mascotas";

        /// <summary>
        /// Acción: Registro de mascotas.
        /// </summary>
        public const string InsertMascotas = "Registro_Mascotas";

        /// <summary>
        /// Acción: Modificación de mascotas.
        /// </summary>
        public const string EditMascotas = "Modificar_Mascotas";

        /// <summary>
        /// Acción: Eliminación de mascotas.
        /// </summary>
        public const string DeleteMascotas = "Eliminar_Mascotas";

        /// <summary>
        /// Acción: Listado de refugios.
        /// </summary>
        public const string ListRefugios = "Listado_Refugios";

        /// <summary>
        /// Acción: Registro de refugios.
        /// </summary>
        public const string InsertRefugios = "Registro_Refugios";

        /// <summary>
        /// Acción: Modificación de refugios.
        /// </summary>
        public const string EditRefugios = "Modificar_Refugios";

        /// <summary>
        /// Acción: Eliminación de refugios.
        /// </summary>
        public const string DeleteRefugios = "Eliminar_Refugios";

        /// <summary>
        /// Acción: Listado de empleados.
        /// </summary>
        public const string ListEmpleados = "Listado_Empleados";

        /// <summary>
        /// Acción: Registro de empleados.
        /// </summary>
        public const string InsertEmpleados = "Registro_Empleados";

        /// <summary>
        /// Acción: Modificación de empleados.
        /// </summary>
        public const string EditEmpleados = "Modificar_Empleados";

        /// <summary>
        /// Acción: Eliminación de empleados.
        /// </summary>
        public const string DeleteEmpleados = "Eliminar_Empleados";

        /// <summary>
        /// Acción: Listado de cargos de empleados.
        /// </summary>
        public const string ListEmpleadosCargos = "Listado_EmpleadosCargos";

        /// <summary>
        /// Acción: Registro de cargos de empleados.
        /// </summary>
        public const string InsertEmpleadosCargos = "Registro_EmpleadosCargos";

        /// <summary>
        /// Acción: Modificación de cargos de empleados.
        /// </summary>
        public const string EditEmpleadosCargos = "Modificar_EmpleadosCargos";

        /// <summary>
        /// Acción: Eliminación de cargos de empleados.
        /// </summary>
        public const string DeleteEmpleadosCargos = "Eliminar_EmpleadosCargos";

        /// <summary>
        /// Acción: Listado de eventos.
        /// </summary>
        public const string ListEventos = "Listado_Eventos";

        /// <summary>
        /// Acción: Registro de eventos.
        /// </summary>
        public const string InsertEventos = "Registro_Eventos";

        /// <summary>
        /// Acción: Modificación de eventos.
        /// </summary>
        public const string EditEventos = "Modificar_Eventos";

        /// <summary>
        /// Acción: Eliminación de eventos.
        /// </summary>
        public const string DeleteEventos = "Eliminar_Eventos";

        /// <summary>
        /// Acción: Listado de eventos de voluntarios.
        /// </summary>
        public const string ListEventos_Voluntarios = "Listado_Eventos_Voluntarios";

        /// <summary>
        /// Acción: Registro de eventos de voluntarios.
        /// </summary>
        public const string InsertEventos_Voluntarios = "Registro_Eventos_Voluntarios";

        /// <summary>
        /// Acción: Modificación de eventos de voluntarios.
        /// </summary>
        public const string EditEventos_Voluntarios = "Modificar_Eventos_Voluntarios";

        /// <summary>
        /// Acción: Eliminación de eventos de voluntarios.
        /// </summary>
        public const string DeleteEventos_Voluntarios = "Eliminar_Eventos_Voluntarios";

        /// <summary>
        /// Acción: Listado de adopciones.
        /// </summary>
        public const string ListAdopciones = "Listado_Adopciones";

        /// <summary>
        /// Acción: Registro de adopciones.
        /// </summary>
        public const string InsertAdopciones = "Registro_Adopciones";

        /// <summary>
        /// Acción: Modificación de adopciones.
        /// </summary>
        public const string EditAdopciones = "Modificar_Adopciones";

        /// <summary>
        /// Acción: Eliminación de adopciones.
        /// </summary>
        public const string DeleteAdopciones = "Eliminar_Adopciones";

        /// <summary>
        /// Acción: Listado de historial médico.
        /// </summary>
        public const string ListHistorialMedico = "Listado_HistorialMedico";

        /// <summary>
        /// Acción: Registro de historial médico.
        /// </summary>
        public const string InsertHistorialMedico = "Registro_HistorialMedico";

        /// <summary>
        /// Acción: Modificación de historial médico.
        /// </summary>
        public const string EditHistorialMedico = "Modificar_HistorialMedico";

        /// <summary>
        /// Acción: Eliminación de historial médico.
        /// </summary>
        public const string DeleteHistorialMedico = "Eliminar_HistorialMedico";

        /// <summary>
        /// Acción: Listado de tratamientos médicos.
        /// </summary>
        public const string ListTratamientosMedicos = "Listado_TratamientosMedicos";

        /// <summary>
        /// Acción: Registro de tratamientos médicos.
        /// </summary>
        public const string InsertTratamientosMedicos = "Registro_TratamientosMedicos";

        /// <summary>
        /// Acción: Modificación de tratamientos médicos.
        /// </summary>
        public const string EditTratamientosMedicos = "Modificar_TratamientosMedicos";

        /// <summary>
        /// Acción: Eliminación de tratamientos médicos.
        /// </summary>
        public const string DeleteTratamientosMedicos = "Eliminar_TratamientosMedicos";

        /// <summary>
        /// Acción: Listado de voluntarios.
        /// </summary>
        public const string ListVoluntarios = "Listado_Voluntarios";

        /// <summary>
        /// Acción: Registro de voluntarios.
        /// </summary>
        public const string InsertVoluntarios = "Registro_Voluntarios";

        /// <summary>
        /// Acción: Modificación de voluntarios.
        /// </summary>
        public const string EditVoluntarios = "Modificar_Voluntarios";

        /// <summary>
        /// Acción: Eliminación de voluntarios.
        /// </summary>
        public const string DeleteVoluntarios = "Eliminar_Voluntarios";

        #endregion EsquemaRefugio
    }
}