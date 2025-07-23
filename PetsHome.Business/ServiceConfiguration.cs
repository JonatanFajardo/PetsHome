using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using PetsHome.Business.Extensions;
using PetsHome.Business.Services;
using PetsHome.DataAccess;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business
{
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Agrega las dependencias de la capa de negocio
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        public static void AddLogicLayer(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<MunicipioRepository>();
            services.AddScoped<LocalidadRepository>();
            services.AddScoped<CategoriaRepository>();
            services.AddScoped<EntradaRepository>();
            services.AddScoped<EntradasDetalleRepository>();
            services.AddScoped<InventarioRepository>();
            services.AddScoped<InventariosDetalleRepository>();
            services.AddScoped<ItemRepository>();
            services.AddScoped<RefugioRepository>();
            services.AddScoped<EmpleadoRepository>();
            services.AddScoped<EmpleadosCargoRepository>();
            services.AddScoped<EventoRepository>();
            services.AddScoped<AdopcionRepository>();
            services.AddScoped<CitaMedicaRepository>();
            services.AddScoped<MascotaRepository>();
            services.AddScoped<ProcedenciaRepository>();
            services.AddScoped<RazaRepository>();
            services.AddScoped<SolicitudRepository>();
            services.AddScoped<VacunaRepository>();
            services.AddScoped<VoluntarioRepository>();
            services.AddScoped<ReportesRepository>();
            services.AddScoped<DonacionRepository>();
            services.AddScoped<SalidasRepository>();
            services.AddScoped<SalidasDetallesRepository>();
            services.AddScoped<RecepcionesMercanciaRepository>();
            services.AddScoped<RecepcionesDetallesRepository>();
            services.AddScoped<ExistenciasRepository>();

            // Registrar DbContext para reportes que requieren acceso directo a la base de datos
            services.AddScoped<PetsHomeDbContext>();

            //https://www.it-swarm.dev/es/c%23/obtencion-de-url-absolutas-utilizando-asp.net-core/1053425403/
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped<IUrlHelper>(x => x
                .GetRequiredService<IUrlHelperFactory>()
                .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

            PetsHomeDbContext.BuildConnectionString(connectionString);
        }

        /// <summary>
        /// Agrega los servicios de la capa de negocio
        /// </summary>
        /// <param name="services"></param>
        public static void AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<MunicipioService>();
            services.AddScoped<DepartamentoService>();
            services.AddScoped<CategoriaService>();
            services.AddScoped<EntradaService>();
            services.AddScoped<EntradasDetalleService>();
            services.AddScoped<InventarioService>();
            services.AddScoped<InventariosDetalleService>();
            services.AddScoped<ItemService>();
            services.AddScoped<RefugioService>();
            services.AddScoped<EmpleadoService>();
            services.AddScoped<EmpleadosCargoService>();
            services.AddScoped<EventoService>();
            services.AddScoped<AdopcionService>();
            services.AddScoped<ComportamientosService>();
            services.AddScoped<CitaMedicaService>();
            services.AddScoped<MascotaService>();
            services.AddScoped<ProcedenciaService>();
            services.AddScoped<RazaService>();
            services.AddScoped<SolicitudService>();
            services.AddScoped<VacunaService>();
            services.AddScoped<VoluntarioService>();
            services.AddScoped<ReportesService>();
            services.AddScoped<DonacionService>();
            services.AddScoped<SalidaService>();
            services.AddScoped<SalidasDetallesService>();
            services.AddScoped<RecepcionMercanciaService>();
            services.AddScoped<RecepcionesDetallesService>();
            services.AddScoped<ExistenciasService>();

            /// Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileExtensions());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc();
        }
    }
}