using AutoMapper;
using PetsHome.Contracts.DTOs;
using Entities = PetsHome.Common.Entities;

namespace PetsHome.Business.Extensions
{
    /// <summary>
    /// Mapeos manuales entre Entities y DTOs generados.
    /// </summary>
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            // Albergue
            //CreateMap<Entities.PR_Albergue_Albergues_DeleteResult, AlbergueAlberguesDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_EmpleadosCargos_DeleteResult, AlbergueEmpleadosCargosDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_FichaAdopcion_DeleteResult, AlbergueFichaAdopcionDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_FichasMedicas_DeleteResult, AlbergueFichasMedicasDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_Mascotas_DeleteResult, AlbergueMascotasDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_Procedencias_DeleteResult, AlbergueProcedenciasDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_Razas_DeleteResult, AlbergueRazasDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_Solicitudes_DeleteResult, AlbergueSolicitudesDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_Vacunas_DeleteResult, AlbergueVacunasDeleteDto>().ReverseMap();
            //CreateMap<Entities.PR_Albergue_Voluntarios_DeleteResult, AlbergueVoluntariosDeleteDto>().ReverseMap();

            // General
            CreateMap<Entities.PR_General_Departamentos_DetailResult, GeneralDepartamentosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_General_Departamentos_DropdownResult, GeneralDepartamentosDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_General_Departamentos_FindResult, GeneralDepartamentosFindDto>().ReverseMap();
            CreateMap<Entities.PR_General_Departamentos_ListResult, GeneralDepartamentosListDto>().ReverseMap();
            CreateMap<Entities.PR_General_Municipios_DetailResult, GeneralMunicipiosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_General_Municipios_DropdownResult, GeneralMunicipiosDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_General_Municipios_FindResult, GeneralMunicipiosFindDto>().ReverseMap();
            CreateMap<Entities.PR_General_Municipios_ListResult, GeneralMunicipiosListDto>().ReverseMap();
            CreateMap<Entities.PR_General_Municipios_SelectbyDepartamentoResult, GeneralMunicipiosSelectbyDepartamentoDto>().ReverseMap();

            // Inventario
            CreateMap<Entities.PR_Inventario_Categorias_DetailResult, InventarioCategoriasDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Categorias_DropdownResult, InventarioCategoriasDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Categorias_FindResult, InventarioCategoriasFindDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Categorias_ListResult, InventarioCategoriasListDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Entradas_FindResult, InventarioEntradasFindDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_EntradasDetalles_FindResult, InventarioEntradasDetallesFindDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Inventarios_DetailResult, InventarioInventariosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Inventarios_FindResult, InventarioInventariosFindDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Inventarios_ListResult, InventarioInventariosListDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Items_DetailResult, InventarioItemsDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Items_DropdownResult, InventarioItemsDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Items_FindResult, InventarioItemsFindDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_Items_ListResult, InventarioItemsListDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_RecepcionesDetalles_FindResult, InventarioRecepcionesDetallesFindDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_RecepcionesDetalles_ListResult, InventarioRecepcionesDetallesListDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_RecepcionesMercancia_DetailResult, InventarioRecepcionesMercanciaDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_RecepcionesMercancia_FindResult, InventarioRecepcionesMercanciaFindDto>().ReverseMap();
            CreateMap<Entities.PR_Inventario_RecepcionesMercancia_ListResult, InventarioRecepcionesMercanciaListDto>().ReverseMap();

            // Refugio
            CreateMap<Entities.PR_Refugio_Adopciones_DetailResult, RefugioAdopcionesDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Adopciones_FindResult, RefugioAdopcionesFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Adopciones_ListResult, RefugioAdopcionesListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Empleados_DetailResult, RefugioEmpleadosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Empleados_FindResult, RefugioEmpleadosFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Empleados_ListResult, RefugioEmpleadosListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_EmpleadosCargos_DetailResult, RefugioEmpleadosCargosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_EmpleadosCargos_DropdownResult, RefugioEmpleadosCargosDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_EmpleadosCargos_FindResult, RefugioEmpleadosCargosFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_EmpleadosCargos_ListResult, RefugioEmpleadosCargosListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Eventos_DetailResult, RefugioEventosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Eventos_FindResult, RefugioEventosFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Eventos_InsertResult, RefugioEventosInsertDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Eventos_ListResult, RefugioEventosListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_HistorialMedico_DetailResult, RefugioHistorialMedicoDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_HistorialMedico_FindResult, RefugioHistorialMedicoFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_HistorialMedico_ListResult, RefugioHistorialMedicoListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Mascotas_DetailResult, RefugioMascotasDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Mascotas_FindResult, RefugioMascotasFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Mascotas_ListResult, RefugioMascotasListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Procedencia_DropdownResult, RefugioProcedenciaDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Procedencias_DetailResult, RefugioProcedenciasDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Procedencias_FindResult, RefugioProcedenciasFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Procedencias_InsertResult, RefugioProcedenciasInsertDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Procedencias_ListResult, RefugioProcedenciasListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Raza_DropdownResult, RefugioRazaDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Razas_DetailResult, RefugioRazasDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Razas_FindResult, RefugioRazasFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Razas_ListResult, RefugioRazasListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Refugio_DropdownResult, RefugioRefugioDropdownDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Refugios_DetailResult, RefugioRefugiosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Refugios_FindResult, RefugioRefugiosFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Refugios_ListResult, RefugioRefugiosListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Solicitudes_DetailResult, RefugioSolicitudesDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Solicitudes_FindResult, RefugioSolicitudesFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Solicitudes_ListResult, RefugioSolicitudesListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Vacunas_DetailResult, RefugioVacunasDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Vacunas_FindResult, RefugioVacunasFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Vacunas_ListResult, RefugioVacunasListDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Voluntarios_DetailResult, RefugioVoluntariosDetailDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Voluntarios_FindResult, RefugioVoluntariosFindDto>().ReverseMap();
            CreateMap<Entities.PR_Refugio_Voluntarios_ListResult, RefugioVoluntariosListDto>().ReverseMap();

            // Seguridad
            CreateMap<Entities.PR_Seguridad_RegistroEventos_FindResult, SeguridadRegistroEventosFindDto>().ReverseMap();
            CreateMap<Entities.PR_Seguridad_RegistroEventos_InsertResult, SeguridadRegistroEventosInsertDto>().ReverseMap();
            CreateMap<Entities.PR_Seguridad_RegistroEventos_SelectResult, SeguridadRegistroEventosSelectDto>().ReverseMap();

            // Mapeos entre DTOs para conversiones
            CreateMap<RefugioMascotasFindDto, MascotasDto>();

            // Tablas base
            CreateMap<Entities.tbAdopciones, AdopcionesDto>().ReverseMap();
            CreateMap<Entities.tbCategorias, CategoriasDto>().ReverseMap();
            CreateMap<Entities.tbComportamientos, ComportamientosDto>().ReverseMap();
            CreateMap<Entities.tbDepartamentos, DepartamentosDto>().ReverseMap();
            CreateMap<Entities.tbEmpleados, EmpleadosDto>().ReverseMap();
            CreateMap<Entities.tbEmpleadosCargos, EmpleadosCargosDto>().ReverseMap();
            CreateMap<Entities.tbEventos, EventosDto>().ReverseMap();
            CreateMap<Entities.tbEventos_tbVoluntarios, EventosVoluntariosDto>().ReverseMap();
            CreateMap<Entities.tbHistorialMedico, HistorialMedicoDto>().ReverseMap();
            CreateMap<Entities.tbHistorialMedico_tbVacunas, HistorialMedicoVacunasDto>().ReverseMap();
            CreateMap<Entities.tbItems, ItemsDto>().ReverseMap();
            CreateMap<Entities.tbMascotas, MascotasDto>().ReverseMap();
            CreateMap<Entities.tbMunicipios, MunicipiosDto>().ReverseMap();
            CreateMap<Entities.tbPersonas, PersonasDto>().ReverseMap();
            CreateMap<Entities.tbProcedencias, ProcedenciasDto>().ReverseMap();
            CreateMap<Entities.tbRazas, RazasDto>().ReverseMap();
            CreateMap<Entities.tbRecepcionesDetalles, RecepcionesDetallesDto>().ReverseMap();
            CreateMap<Entities.tbRecepcionesMercancia, RecepcionesMercanciaDto>().ReverseMap();
            CreateMap<Entities.tbRefugios, RefugiosDto>().ReverseMap();
            CreateMap<Entities.tbRegistroEventos, RegistroEventosDto>().ReverseMap();
            CreateMap<Entities.tbSolicitudes, SolicitudesDto>().ReverseMap();
            CreateMap<Entities.tbUsuarios, UsuariosDto>().ReverseMap();
            CreateMap<Entities.tbVacunas, VacunasDto>().ReverseMap();
            CreateMap<Entities.tbVoluntarios, VoluntariosDto>().ReverseMap();
        }
    }
}