﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using PetsHome.Common.AppPetshomeDbContext;
using PetsHome.Common.Entities;
using System;

namespace PetsHome.Common.AppPetshomeDbContext.Configurations
{
    public partial class tbRegistroEventosConfiguration : IEntityTypeConfiguration<tbRegistroEventos>
    {
        public void Configure(EntityTypeBuilder<tbRegistroEventos> entity)
        {
            entity.HasKey(e => e.Evt_Id);

            entity.ToTable("tbRegistroEventos", "Seguridad");

            entity.Property(e => e.Evt_Id).HasComment(" Id identificador de la tabla");

            entity.Property(e => e.Evt_Detalles)
                .HasComment("Detalles del evento")
                .UseCollation("Modern_Spanish_CI_AS");

            entity.Property(e => e.Evt_DireccionIP)
                .HasComment("Dirección IP del usuario que realizará el evento")
                .UseCollation("Modern_Spanish_CI_AS");

            entity.Property(e => e.Evt_EstadoAnterior)
                .HasComment("Estado anterior del evento")
                .UseCollation("Modern_Spanish_CI_AS");

            entity.Property(e => e.Evt_FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del evento");

            entity.Property(e => e.Evt_NuevoEstado)
                .HasComment("Nuevo estado del evento")
                .UseCollation("Modern_Spanish_CI_AS");

            entity.Property(e => e.Evt_UserAgent)
                .HasComment("Navegador del usuario que realizará el evento")
                .UseCollation("Modern_Spanish_CI_AS");

            entity.Property(e => e.Evt_Usu_Id).HasComment("Id identificador del usuario del sistema que realizará el evento");

            entity.Property(e => e.Tpevt_Id).HasComment("Número que hace referencia al ID del tipo de evento");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<tbRegistroEventos> entity);
    }
}
