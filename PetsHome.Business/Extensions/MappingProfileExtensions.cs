using System;
using System.Linq;
using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Extensions
{
    /// <summary>
    /// Clase que contiene los mapeos de las entidades
    /// </summary>
    public class MappingProfileExtensions : Profile
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public MappingProfileExtensions()
        {
            // Mapeos para Departamento - ViewModels separados
            CreateMap<PR_General_Departamentos_ListResult, DepartamentoListViewModel>().ReverseMap();
            CreateMap<PR_General_Departamentos_FindResult, DepartamentoFormViewModel>().ReverseMap();
            CreateMap<PR_General_Departamentos_DetailResult, DepartamentoDetailsViewModel>().ReverseMap();
            CreateMap<PR_General_Departamentos_DropdownResult, DepartamentoListViewModel>().ReverseMap();
            CreateMap<tbDepartamentos, DepartamentoFormViewModel>().ReverseMap();

            // Mapeos para Municipio - ViewModels separados
            CreateMap<PR_General_Municipios_ListResult, MunicipioListViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_FindResult, MunicipioFormViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_DetailResult, MunicipioDetailsViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_DetailResult, MunicipioListViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_DropdownResult, MunicipioListViewModel>().ReverseMap();
            CreateMap<PR_General_Municipios_SelectbyDepartamentoResult, MunicipioListViewModel>().ReverseMap();
            CreateMap<tbMunicipios, MunicipioFormViewModel>().ReverseMap();

            // Mantener compatibilidad con ViewModels antiguos (deprecated, se eliminará en futuras versiones)
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
            CreateMap<PR_Inventario_Items_DetailResult, ItemViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Items_FindResult, ItemViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Items_ListResult, ItemViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Adopciones_DetailResult, AdopcionViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Adopciones_FindResult, AdopcionViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Adopciones_ListResult, AdopcionViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Empleados_ListResult, EmpleadoListViewModel>()
                .ForMember(dest => dest.esActivo, opt => opt.MapFrom(src => src.EsActivo));
            CreateMap<PR_Refugio_Empleados_DetailResult, EmpleadoDetailsViewModel>()
                .ForMember(dest => dest.esActivo, opt => opt.MapFrom(src => src.EsActivo))
                .ForMember(dest => dest.emp_UsuarioCrea, opt => opt.Ignore())
                .ForMember(dest => dest.emp_UsuarioModifica, opt => opt.Ignore())
                .ForMember(dest => dest.emp_FechaCrea, opt => opt.MapFrom(src => src.per_FechaCrea))
                .ForMember(dest => dest.emp_FechaModifica, opt => opt.MapFrom(src => src.per_FechaModifica));
            CreateMap<PR_Refugio_Empleados_FindResult, EmpleadoFormViewModel>()
                .ForMember(dest => dest.esActivo, opt => opt.MapFrom(src => src.esActivo))
                .ForMember(dest => dest.per, opt => opt.MapFrom(src => new PersonaViewModel
                {
                    per_Id = src.per_Id,
                    per_PrimerNombre = src.per_PrimerNombre,
                    per_SegundoNombre = src.per_SegundoNombre,
                    per_ApellidoPaterno = src.per_ApellidoPaterno,
                    per_ApellidoMaterno = src.per_ApellidoMaterno,
                    per_Identidad = src.per_Identidad,
                    per_FechaNacimiento = src.per_FechaNacimiento,
                    per_Domicilio = src.per_Domicilio,
                    per_Telefono = src.per_Telefono,
                    per_Correo = src.per_Correo,
                    per_UsuarioCrea = src.per_UsuarioCrea,
                    per_FechaCrea = src.per_FechaCrea,
                    per_UsuarioModifica = src.per_UsuarioModifica,
                    per_FechaModifica = src.per_FechaModifica
                }));
            CreateMap<PR_Refugio_EmpleadosCargos_DetailResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_EmpleadosCargos_DropdownResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_EmpleadosCargos_FindResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_EmpleadosCargos_ListResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Eventos_DetailResult, EventoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Eventos_FindResult, EventoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Eventos_ListResult, EventoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_HistorialMedico_FindResult, HistorialMedicoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_HistorialMedico_ListResult, HistorialMedicoViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Mascotas_DetailResult, MascotaDetailsViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Mascotas_FindResult, MascotaFormViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Mascotas_ListResult, MascotaListViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencia_DropdownResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_DetailResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_FindResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_InsertResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Procedencias_ListResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Raza_DropdownResult, RazaDropdownViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Razas_DetailResult, RazaDetailsViewModel>()
                .ForMember(dest => dest.UsuarioCreacion, opt => opt.MapFrom(src => src.UsuarioCreacion))
                .ForMember(dest => dest.UsuarioModificacion, opt => opt.MapFrom(src => src.UsuarioModificacion))
                .ReverseMap();
            CreateMap<PR_Refugio_Razas_FindResult, RazaFormViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Razas_ListResult, RazaListViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Refugio_DropdownResult, RefugioDropdownViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Refugios_DetailResult, RefugioDetailsViewModel>()
                .ForMember(dest => dest.refg_UsuarioCrea, opt => opt.Ignore())
                .ForMember(dest => dest.refg_UsuarioModifica, opt => opt.Ignore())
                .ForMember(dest => dest.refg_NombreUsuarioCrea, opt => opt.MapFrom(src => src.UsuarioCreacion))
                .ForMember(dest => dest.refg_NombreUsuarioModifica, opt => opt.MapFrom(src => src.UsuarioModificacion));
            CreateMap<PR_Refugio_Refugios_FindResult, RefugioFormViewModel>()
                .ForMember(dest => dest.refg_NombreUsuarioCrea, opt => opt.MapFrom(src => src.usuarioCrea))
                .ForMember(dest => dest.refg_NombreUsuarioModifica, opt => opt.MapFrom(src => src.usuarioModifica))
                .ReverseMap();
            CreateMap<PR_Refugio_Refugios_ListResult, RefugioListViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Solicitudes_ListResult, SolicitudListViewModel>();
            CreateMap<PR_Refugio_Solicitudes_DetailResult, SolicitudDetailsViewModel>()
                .ForMember(dest => dest.sol_UsuarioCrea, opt => opt.Ignore())
                .ForMember(dest => dest.sol_UsuarioModifica, opt => opt.Ignore())
                .ForMember(dest => dest.sol_NombreUsuarioCrea, opt => opt.MapFrom(src => src.UsuarioCreacion))
                .ForMember(dest => dest.sol_NombreUsuarioModifica, opt => opt.MapFrom(src => src.UsuarioModificacion));
            CreateMap<PR_Refugio_Solicitudes_FindResult, SolicitudFormViewModel>()
                .ForMember(dest => dest.sol_NombreUsuarioCrea, opt => opt.MapFrom(src => src.usuarioCrea))
                .ForMember(dest => dest.sol_NombreUsuarioModifica, opt => opt.MapFrom(src => src.usuarioModifica));
            CreateMap<PR_Refugio_Vacunas_ListResult, VacunaListViewModel>()
                .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.EsActivo));
            CreateMap<PR_Refugio_Vacunas_DetailResult, VacunaDetailsViewModel>()
                .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.EsActivo))
                .ForMember(dest => dest.vac_NombreUsuarioCrea, opt => opt.MapFrom(src => src.UsuarioCreacion))
                .ForMember(dest => dest.vac_NombreUsuarioModifica, opt => opt.MapFrom(src => src.UsuarioModificacion));
            CreateMap<PR_Refugio_Vacunas_FindResult, VacunaFormViewModel>()
                .ForMember(dest => dest.vac_EsActivo, opt => opt.MapFrom(src => src.vac_EsActivo))
                .ForMember(dest => dest.vac_NombreUsuarioCrea, opt => opt.MapFrom(src => src.usuarioCrea))
                .ForMember(dest => dest.vac_NombreUsuarioModifica, opt => opt.MapFrom(src => src.usuarioModifica));
            CreateMap<PR_Refugio_Voluntarios_ListResult, VoluntarioListViewModel>();
            CreateMap<PR_Refugio_Voluntarios_DetailResult, VoluntarioDetailsViewModel>()
                .ForMember(dest => dest.vol_HorasTrabajadas, opt => opt.MapFrom(src => (int?)src.vol_HorasTrabajadas))
                .ForMember(dest => dest.vol_Nombres, opt => opt.MapFrom(src => string.Join(" ", new[]
                {
                    src.per_PrimerNombre,
                    src.per_SegundoNombre,
                    src.per_ApellidoPaterno,
                    src.per_ApellidoMaterno
                }.Where(name => !string.IsNullOrWhiteSpace(name))).Trim()))
                .ForMember(dest => dest.per_Apellidos, opt => opt.MapFrom(src => string.Join(" ", new[]
                {
                    src.per_ApellidoPaterno,
                    src.per_ApellidoMaterno
                }.Where(name => !string.IsNullOrWhiteSpace(name))).Trim()))
                .ForMember(dest => dest.vol_NombreUsuarioCrea, opt => opt.MapFrom(src => src.UsuarioCreacion))
                .ForMember(dest => dest.vol_NombreUsuarioModifica, opt => opt.MapFrom(src => src.UsuarioModificacion))
                .ForMember(dest => dest.vol_FechaCrea, opt => opt.MapFrom(src => src.per_FechaCrea))
                .ForMember(dest => dest.per_UsuarioCrea, opt => opt.MapFrom(src => src.per_UsuarioCrea))
                .ForMember(dest => dest.vol_FechaModifica, opt => opt.MapFrom(src => src.per_FechaModifica))
                .ForMember(dest => dest.vol_UsuarioModifica, opt => opt.MapFrom(src => src.per_UsuarioModifica))
                .ForMember(dest => dest.per, opt => opt.MapFrom(src => new PersonaViewModel
                {
                    per_Id = 0,
                    per_PrimerNombre = src.per_PrimerNombre,
                    per_SegundoNombre = src.per_SegundoNombre,
                    per_ApellidoPaterno = src.per_ApellidoPaterno,
                    per_ApellidoMaterno = src.per_ApellidoMaterno,
                    per_Identidad = src.per_Identidad,
                    per_FechaNacimiento = src.per_FechaNacimiento,
                    per_Domicilio = src.per_Domicilio,
                    per_Telefono = src.per_Telefono,
                    per_Correo = src.per_Correo,
                    per_UsuarioCrea = src.per_UsuarioCrea,
                    per_FechaCrea = src.per_FechaCrea,
                    per_UsuarioModifica = src.per_UsuarioModifica,
                    per_FechaModifica = src.per_FechaModifica
                }));
            CreateMap<PR_Refugio_Voluntarios_FindResult, VoluntarioFormViewModel>()
                .ForMember(dest => dest.vol_HorasTrabajadas, opt => opt.MapFrom(src => (int?)src.vol_HorasTrabajadas))
                .ForMember(dest => dest.per_Id, opt => opt.MapFrom(src => src.per_Id))
                .ForMember(dest => dest.per_Apellidos, opt => opt.MapFrom(src => string.Join(" ", new[]
                {
                    src.per_ApellidoPaterno,
                    src.per_ApellidoMaterno
                }.Where(name => !string.IsNullOrWhiteSpace(name))).Trim()))
                .ForMember(dest => dest.per_Identidad, opt => opt.MapFrom(src => src.per_Identidad))
                .ForMember(dest => dest.vol_Nombres, opt => opt.MapFrom(src => string.Join(" ", new[]
                {
                    src.per_PrimerNombre,
                    src.per_SegundoNombre,
                    src.per_ApellidoPaterno,
                    src.per_ApellidoMaterno
                }.Where(name => !string.IsNullOrWhiteSpace(name))).Trim()))
                .ForMember(dest => dest.per, opt => opt.MapFrom(src => new PersonaViewModel
                {
                    per_Id = src.per_Id,
                    per_PrimerNombre = src.per_PrimerNombre,
                    per_SegundoNombre = src.per_SegundoNombre,
                    per_ApellidoPaterno = src.per_ApellidoPaterno,
                    per_ApellidoMaterno = src.per_ApellidoMaterno,
                    per_Identidad = src.per_Identidad,
                    per_FechaNacimiento = src.per_FechaNacimiento,
                    per_Domicilio = src.per_Domicilio,
                    per_Telefono = src.per_Telefono,
                    per_Correo = src.per_Correo,
                    per_UsuarioCrea = src.per_UsuarioCrea,
                    per_FechaCrea = src.per_FechaCrea,
                    per_UsuarioModifica = src.per_UsuarioModifica,
                    per_FechaModifica = src.per_FechaModifica
                }));
            CreateMap<PR_Albergue_EmpleadosCargos_DeleteResult, EmpleadoCargoViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Mascotas_DeleteResult, MascotaListViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Procedencias_DeleteResult, ProcedenciaViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Razas_DeleteResult, RazaListViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Solicitudes_DeleteResult, SolicitudListViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Vacunas_DeleteResult, VacunaListViewModel>().ReverseMap();
            CreateMap<PR_Albergue_Voluntarios_DeleteResult, VoluntarioListViewModel>().ReverseMap();
            CreateMap<tbAdopciones, AdopcionViewModel>().ReverseMap();
            CreateMap<tbCategorias, CategoriaViewModel>().ReverseMap();
            CreateMap<tbDepartamentos, DepartamentoViewModel>().ReverseMap(); // Deprecated - usar DepartamentoFormViewModel
            CreateMap<tbEmpleados, EmpleadoFormViewModel>().ReverseMap();
            CreateMap<tbItems, ItemViewModel>().ReverseMap();
            CreateMap<PR_Inventario_Items_DropdownResult, ItemViewModel>().ReverseMap();
            CreateMap<tbEventos, EventoViewModel>().ReverseMap();
            CreateMap<tbItems, ItemViewModel>().ReverseMap();
            CreateMap<tbMascotas, MascotaFormViewModel>().ReverseMap();
            CreateMap<tbMunicipios, MunicipioViewModel>().ReverseMap(); // Deprecated - usar MunicipioFormViewModel
            CreateMap<tbPersonas, PersonaViewModel>().ReverseMap();
            CreateMap<tbProcedencias, ProcedenciaViewModel>().ReverseMap();
            CreateMap<tbRazas, RazaFormViewModel>().ReverseMap();
            CreateMap<tbRefugios, RefugioFormViewModel>().ReverseMap();
            CreateMap<tbSolicitudes, SolicitudFormViewModel>().ReverseMap();
            CreateMap<tbVacunas, VacunaFormViewModel>().ReverseMap();
            CreateMap<tbVoluntarios, VoluntarioFormViewModel>().ReverseMap();

            // Mapeos para RecepcionMercancia - ViewModels separados
            CreateMap<PR_Inventario_RecepcionesMercancia_ListResult, RecepcionMercanciaListViewModel>().ReverseMap();
            CreateMap<PR_Inventario_RecepcionesMercancia_FindResult, RecepcionMercanciaFormViewModel>().ReverseMap();
            CreateMap<PR_Inventario_RecepcionesMercancia_DetailResult, RecepcionMercanciaDetailsViewModel>().ReverseMap();
            CreateMap<tbRecepcionesMercancia, RecepcionMercanciaFormViewModel>().ReverseMap();

            // Mapeos para RecepcionDetalle - ViewModels separados
            CreateMap<PR_Inventario_RecepcionesDetalles_ListResult, RecepcionDetalleListViewModel>().ReverseMap();
            CreateMap<PR_Inventario_RecepcionesDetalles_FindResult, RecepcionDetalleFormViewModel>().ReverseMap();
            CreateMap<tbRecepcionesDetalles, RecepcionDetalleFormViewModel>().ReverseMap();

            ////Referencia
            ////CreateMap<tbRoles, RoleViewModel>().ReverseMap()();
            ////CreateMap<tbContratos, ContratosViewModel>().ReverseMap()();

            ////CreateMap<tbHistorialMedico_Vacunas, HistorialMedico_VacunaViewModel>().ReverseMap();
            ////CreateMap<tbEventos_tbVoluntarios, Eventos_tbVoluntarioViewModel>().ReverseMap();
            ////CreateMap<tbSalida, SalidaViewModel>().ReverseMap();
            ////CreateMap<tbSubmenu, SubmenuViewModel>().ReverseMap();
            ////CreateMap<tbSubmenu_tbModpan, Submenu_tbModpanViewModel>().ReverseMap();
            ////Departamento
        }
    }
}
