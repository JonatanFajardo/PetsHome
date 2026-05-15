using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using SmartBreadcrumbs.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetsHome.Business;
using PetsHome.Business.Models;
using PetsHome.UI.Middleware;
using System;

namespace PetsHome.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddRazorRuntimeCompilation();

            services.AddBreadcrumbs(GetType().Assembly, options =>
            {
                options.TagName = "nav";
                options.TagClasses = "";
                options.OlClasses = "breadcrumb";
                options.LiClasses = "breadcrumb-item";
                options.ActiveLiClasses = "breadcrumb-item active";
            });

            services.AddLogicLayer(Configuration.GetConnectionString("PetsHomeConnectionString"));
            services.AddBusinessLogic();
            services.Configure<MascotaFormViewModel>(Configuration.GetSection("Filepath"));

            // Configuración de autenticación por cookies
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });

            // Configuración de CORS para Angular
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", builder =>
                {
                    builder.WithOrigins(
                            "http://localhost:4200",
                            "http://localhost:4201",
                            "http://localhost:4202"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddResponseCompression(opts =>
            {
                opts.EnableForHttps = true;
                opts.Providers.Add<BrotliCompressionProvider>();
                opts.Providers.Add<GzipCompressionProvider>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Middleware global de manejo de errores
            //app.UseMiddleware<ErrorHandlerMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();// permite que se muestre el error en la pantalla
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Redirige /Controller/Index → /Controller para URLs más limpias
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;
                if (path != null && path.EndsWith("/Index", System.StringComparison.OrdinalIgnoreCase))
                {
                    var cleanPath = path.Substring(0, path.Length - 6);
                    context.Response.Redirect(cleanPath.Length > 0 ? cleanPath : "/", permanent: true);
                    return;
                }
                await next();
            });

            app.UseRouting();

            // Habilitar CORS antes de Authorization
            app.UseCors("AllowAngularApp");

            // Habilitar autenticación y autorización
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "controller-only",
                    pattern: "{controller}",
                    defaults: new { action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Landing}/{action=Index}/{id?}");
            });
        }
    }
}
