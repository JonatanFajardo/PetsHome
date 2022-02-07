﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore; 
using PetsHome.Common.Entities;
using System;
#nullable disable

#nullable disable

namespace PetsHome.DataAccess.AppPetshomeDbContext
{
    public partial class AppPetshomeContext : DbContext
    {
        public AppPetshomeContext()
        {
        }

        public AppPetshomeContext(DbContextOptions<AppPetshomeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<tbAdopciones> tbAdopciones { get; set; }
        public virtual DbSet<tbCategorias> tbCategorias { get; set; }
        public virtual DbSet<tbDepartamentos> tbDepartamentos { get; set; }
        public virtual DbSet<tbEmpleados> tbEmpleados { get; set; }
        public virtual DbSet<tbEmpleadosCargos> tbEmpleadosCargos { get; set; }
        public virtual DbSet<tbEntradas> tbEntradas { get; set; }
        public virtual DbSet<tbEntradasDetalles> tbEntradasDetalles { get; set; }
        public virtual DbSet<tbEventos> tbEventos { get; set; }
        public virtual DbSet<tbEventos_tbVoluntarios> tbEventos_tbVoluntarios { get; set; }
        public virtual DbSet<tbHistorialMedico> tbHistorialMedico { get; set; }
        public virtual DbSet<tbHistorialMedico_tbVacunas> tbHistorialMedico_tbVacunas { get; set; }
        public virtual DbSet<tbInventarios> tbInventarios { get; set; }
        public virtual DbSet<tbInventariosDetalles> tbInventariosDetalles { get; set; }
        public virtual DbSet<tbItems> tbItems { get; set; }
        public virtual DbSet<tbMascotas> tbMascotas { get; set; }
        public virtual DbSet<tbMunicipios> tbMunicipios { get; set; }
        public virtual DbSet<tbPersonas> tbPersonas { get; set; }
        public virtual DbSet<tbProcedencias> tbProcedencias { get; set; }
        //public virtual DbSet<tbRazas> tbRazas { get; set; }
        public virtual DbSet<tbRefugios> tbRefugios { get; set; }
        public virtual DbSet<tbSolicitudes> tbSolicitudes { get; set; }
        public virtual DbSet<tbUsuarios> tbUsuarios { get; set; }
        public virtual DbSet<tbVacunas> tbVacunas { get; set; }
        public virtual DbSet<tbVoluntarios> tbVoluntarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            //modelBuilder.ApplyConfiguration(new Configurations.tbAdopcionesConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbCategoriasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbDepartamentosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbEmpleadosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbEmpleadosCargosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbEntradasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbEntradasDetallesConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbEventosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbEventos_tbVoluntariosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbHistorialMedicoConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbHistorialMedico_tbVacunasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbInventariosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbInventariosDetallesConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbItemsConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbMascotasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbMunicipiosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbPersonasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbProcedenciasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbRazasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbRefugiosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbSolicitudesConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbUsuariosConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbVacunasConfiguration());
            //modelBuilder.ApplyConfiguration(new Configurations.tbVoluntariosConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
