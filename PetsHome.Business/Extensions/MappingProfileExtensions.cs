using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Extensions
{
    public class MappingProfileExtensions : Profile
    {
        public MappingProfileExtensions()
        {
            CreateMap<PR_General_Departamentos_DetailResult, DepartamentoViewModel>().ReverseMap();
            CreateMap<PR_General_Departamentos_DropdownResult, DepartamentoViewModel>().ReverseMap();
            CreateMap<PR_General_Departamentos_FindResult, DepartamentoViewModel>().ReverseMap();
            CreateMap<PR_General_Departamentos_ListResult, DepartamentoViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_DetailResult, MunicipioViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_DropdownResult, MunicipioViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_FindResult, MunicipioViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_ListResult, MunicipioViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_SelectbyDepartamentoResult, MunicipioViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Categorias_DetailResult, CategoriaViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Categorias_FindResult, CategoriaViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Categorias_ListResult, CategoriaViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Categorias_DropdownResult, CategoriaViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Entradas_DetailResult, EntradaViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Entradas_FindResult, EntradaViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Entradas_ListResult, EntradaViewModel>().ReverseMap();
            CreateMap<tbEntradasDetalles, EntradaDetalleViewModel>().ReverseMap();
            //CreateMap<PR_Inventario_EntradasDetalles_DetailResult, EntradaDetalleViewModel>().ReverseMap();
            CreateMap<EntradaDetalleViewModel, PR_Inventario_EntradasDetalles_ListResult>().ReverseMap();
            CreateMap<PR_Inventario_Inventarios_DetailResult, InventarioViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Inventarios_FindResult, InventarioViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Inventarios_ListResult, InventarioViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Items_DetailResult, ItemViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Items_FindResult, ItemViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Items_ListResult, ItemViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Adopciones_DetailResult, AdopcionViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Adopciones_FindResult, AdopcionViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Adopciones_ListResult, AdopcionViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Empleados_DetailResult, EmpleadoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Empleados_FindResult, EmpleadoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Empleados_ListResult, EmpleadoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_EmpleadosCargos_DetailResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_EmpleadosCargos_DropdownResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_EmpleadosCargos_FindResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_EmpleadosCargos_ListResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Eventos_DetailResult, EventoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Eventos_FindResult, EventoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Eventos_ListResult, EventoViewModel>().ReverseMap();
            //CreateMap<PR_Refugio_HistorialMedico_DetailResult, HistorialMedicViewModel>().ReverseMap();
            CreateMap<PR_Refugio_HistorialMedico_FindResult, HistorialMedicoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_HistorialMedico_ListResult, HistorialMedicoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Mascotas_DetailResult, MascotaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Mascotas_FindResult, MascotaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Mascotas_ListResult, MascotaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencia_DropdownResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_DetailResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_FindResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_InsertResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_ListResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Raza_DropdownResult, RazaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Razas_DetailResult, RazaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Razas_FindResult, RazaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Razas_ListResult, RazaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Refugio_DropdownResult, RefugioViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Refugios_DetailResult, RefugioViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Refugios_FindResult, RefugioViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Refugios_ListResult, RefugioViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Solicitudes_DetailResult, SolicitudViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Solicitudes_FindResult, SolicitudViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Solicitudes_ListResult, SolicitudViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Vacunas_DetailResult, VacunaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Vacunas_FindResult, VacunaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Vacunas_ListResult, VacunaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Voluntarios_DetailResult, VoluntarioViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Voluntarios_FindResult, VoluntarioViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Voluntarios_ListResult, VoluntarioViewModel>().ReverseMap();
            //CreateMap<PR_Albergue_Albergues_DeleteResult, AlbergueViewModel>().ReverseMap();
            CreateMap<PR_Albergue_EmpleadosCargos_DeleteResult, EmpleadoCargoViewModel>().ReverseMap();
            //CreateMap<PR_Albergue_FichaAdopcion_DeleteResult, FichaAdopcioViewModel>().ReverseMap();
            //CreateMap<PR_Albergue_FichasMedicas_DeleteResult, FichaMedicaViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Mascotas_DeleteResult, MascotaViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Procedencias_DeleteResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Razas_DeleteResult, RazaViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Solicitudes_DeleteResult, SolicitudViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Vacunas_DeleteResult, VacunaViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Voluntarios_DeleteResult, VoluntarioViewModel>().ReverseMap();
            CreateMap<tbAdopciones, AdopcionViewModel>().ReverseMap();
            CreateMap<tbCategorias, CategoriaViewModel>().ReverseMap();
            CreateMap<tbDepartamentos, DepartamentoViewModel>().ReverseMap();
            CreateMap<tbEmpleados, EmpleadoViewModel>().ReverseMap();
            CreateMap<tbItems, ItemViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Items_DropdownResult, ItemViewModel>().ReverseMap();
            //.ForMember(x => x.per, opt =>
            //opt.MapFrom(x => x.Select(y => new tbPersonas
            //{
            //    PetId = y.PetId
            //    PetName = y.PetName
            //})));
            //.ForMember(x => x, opt =>
            //opt.MapFrom(y => new tbPersonas
            //{
            //    per_Id = y.per.per_Id,
            //    per_Identidad = y.per.per_Identidad,
            //    per_PrimerNombre = y.per.per_PrimerNombre,
            //    per_SegundoNombre = y.per.per_SegundoNombre,
            //    per_ApellidoPaterno = y.per.per_ApellidoPaterno,
            //    per_ApellidoMaterno = y.per.per_ApellidoMaterno,
            //    per_FechaNacimiento = y.per.per_FechaNacimiento,
            //    per_Domicilio = y.per.per_Domicilio,
            //    per_Telefono = y.per.per_Telefono,
            //    per_Correo = y.per.per_Correo,
            //    per_UsuarioCrea = y.per.per_UsuarioCrea,
            //    per_FechaCrea = y.per.per_FechaCrea,
            //    per_UsuarioModifica = y.per.per_UsuarioModifica,
            //    per_FechaModifica = y.per.per_FechaModifica
            //}));
            //            CreateMap<tbEmpleadosCargos, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<tbEntradas, EntradaViewModel>().ReverseMap();
            //CreateMap<tbEntradasDetalles, EntradasDetalleViewModel>().ReverseMap();
            CreateMap<tbEventos, EventoViewModel>().ReverseMap();
            //CreateMap<tbEventos_tbVoluntarios, Eventos_tbVoluntarioViewModel>().ReverseMap();
            //CreateMap<tbHistorialMedico, HistorialMedicViewModel>().ReverseMap();
            //CreateMap<tbHistorialMedico_tbVacunas, HistorialMedico_tbVacunaViewModel>().ReverseMap();
            CreateMap<tbInventarios, InventarioViewModel>().ReverseMap();
            CreateMap<tbInventariosDetalles, InventarioDetalleViewModel>().ReverseMap();
            CreateMap<tbItems, ItemViewModel>().ReverseMap();
            CreateMap<tbMascotas, MascotaViewModel>().ReverseMap();
            CreateMap<tbMunicipios, MunicipioViewModel>().ReverseMap();
            CreateMap<tbPersonas, PersonaViewModel>().ReverseMap();
            CreateMap<tbProcedencias, ProcedenciaViewModel>().ReverseMap();
            CreateMap<tbRazas, RazaViewModel>().ReverseMap();
            CreateMap<tbRefugios, RefugioViewModel>().ReverseMap();
            CreateMap<tbSolicitudes, SolicitudViewModel>().ReverseMap();
            CreateMap<tbVacunas, VacunaViewModel>().ReverseMap();
            CreateMap<tbVoluntarios, VoluntarioViewModel>().ReverseMap();

            ////Referencia
            ////CreateMap<tbRoles, RoleViewModel>().ReverseMap()();
            ////CreateMap<tbContratos, ContratosViewModel>().ReverseMap()();

            //CreateMap<tbEntradas, EntradaViewModel>().ReverseMap();
            //CreateMap<tbAdopciones, AdopcionViewModel>().ReverseMap();
            //CreateMap<tbHistorialMedico, HistorialMedicoViewModel>().ReverseMap();
            ////CreateMap<tbHistorialMedico_Vacunas, HistorialMedico_VacunaViewModel>().ReverseMap();
            //CreateMap<tbInventarios, InventarioViewModel>().ReverseMap();
            ////CreateMap<tbEventos_tbVoluntarios, Eventos_tbVoluntarioViewModel>().ReverseMap();
            //CreateMap<tbInventariosDetalles, InventarioDetalleViewModel>().ReverseMap();
            //CreateMap<tbItems, ItemViewModel>().ReverseMap();
            //CreateMap<tbRefugios, RefugioViewModel>().ReverseMap();
            //CreateMap<tbMascotas, MascotaViewModel>().ReverseMap();
            //CreateMap<tbPersonas, PersonaViewModel>().ReverseMap();
            //CreateMap<tbProcedencias, ProcedenciaViewModel>().ReverseMap();
            //CreateMap<tbEmpleadosCargos, EmpleadoCargoViewModel>().ReverseMap();
            //CreateMap<tbRazas, RazaViewModel>().ReverseMap();
            //CreateMap<PR_Refugio_Razas_ListResult, RazaViewModel>().ReverseMap();
            //CreateMap<tbEventos, EventoViewModel>().ReverseMap();
            ////CreateMap<tbSalida, SalidaViewModel>().ReverseMap();
            //CreateMap<tbVacunas, VacunaViewModel>().ReverseMap();
            //CreateMap<tbSolicitudes, SolicitudViewModel>().ReverseMap();
            ////CreateMap<tbSubmenu, SubmenuViewModel>().ReverseMap();
            ////CreateMap<tbSubmenu_tbModpan, Submenu_tbModpanViewModel>().ReverseMap();
            //CreateMap<tbVoluntarios, VoluntarioViewModel>().ReverseMap();
            //CreateMap<tbCategorias, CategoriaViewModel>().ReverseMap();
            //CreateMap<tbEmpleados, EmpleadoViewModel>().ReverseMap();

            //CreateMap<tbDepartamentos, MunicipioViewModel>().ReverseMap();
            //CreateMap<MascotaViewModel, PR_Refugio_Mascotas_FindResult>().ReverseMap();

            ////Departamento

            //CreateMap<tbDepartamentos, EditarDepartamento>().ReverseMap();
            //CreateMap<PR_General_Departamentos_FindResult, EditarDepartamento>().ReverseMap();
        }
    }
}