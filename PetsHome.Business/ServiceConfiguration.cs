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
            services.AddScoped<PersonaRepository>();
            services.AddScoped<LocalidadRepository>();
            services.AddScoped<CategoriaRepository>();
            services.AddScoped<EntradaRepository>();
            services.AddScoped<EntradasDetalleRepository>();
            services.AddScoped<InventarioRepository>();
            services.AddScoped<ItemRepository>();
            services.AddScoped<RefugioRepository>();
            services.AddScoped<EmpleadoRepository>();
            services.AddScoped<EmpleadosCargoRepository>();
            services.AddScoped<EventoRepository>();
            services.AddScoped<AdopcionRepository>();
            services.AddScoped<HistorialMedicoRepository>();
            services.AddScoped<MascotaRepository>();
            services.AddScoped<ProcedenciaRepository>();
            services.AddScoped<RazaRepository>();
            services.AddScoped<SolicitudRepository>();
            services.AddScoped<VacunaRepository>();
            services.AddScoped<VoluntarioRepository>();

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
            services.AddScoped<ItemService>();
            services.AddScoped<RefugioService>();
            services.AddScoped<EmpleadoService>();
            services.AddScoped<EmpleadosCargoService>();
            services.AddScoped<EventoService>();
            services.AddScoped<AdopcionService>();
            services.AddScoped<HistorialMedicoService>();
            services.AddScoped<MascotaService>();
            services.AddScoped<ProcedenciaService>();
            services.AddScoped<RazaService>();
            services.AddScoped<SolicitudService>();
            services.AddScoped<VacunaService>();
            services.AddScoped<VoluntarioService>();

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