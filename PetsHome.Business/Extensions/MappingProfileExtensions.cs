using AutoMapper;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Extensions
{
    public class MappingProfileExtensions : Profile
    {
        public MappingProfileExtensions()
        {
            //Referencia
            //CreateMap<tbRoles, RoleViewModel>().ReverseMap()();
            // CreateMap<tbContratos, ContratosViewModel>().ReverseMap()();


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
            CreateMap<tbRazas, RazaViewModel>().ReverseMap();
            CreateMap<PR_Refugio_Razas_ListResult, RazaViewModel>().ReverseMap();
            ////CreateMap<tbEventos, EventoViewModel>().ReverseMap();
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


            //Departamento

            //CreateMap<tbDepartamentos, EditarDepartamento>().ReverseMap();
            //CreateMap<PR_General_Departamentos_FindResult, EditarDepartamento>().ReverseMap();


        }
    }
}
