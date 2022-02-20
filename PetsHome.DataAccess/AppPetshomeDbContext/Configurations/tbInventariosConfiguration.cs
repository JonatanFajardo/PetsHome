﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using PetsHome.Common.AppPetshomeDbContext;
using PetsHome.Common.Entities;
using System;

namespace PetsHome.Common.AppPetshomeDbContext.Configurations
{
    public partial class tbInventariosConfiguration : IEntityTypeConfiguration<tbInventarios>
    {
        public void Configure(EntityTypeBuilder<tbInventarios> entity)
        {
            entity.HasKey(e => e.inv_Id);

            entity.ToTable("tbInventarios", "Inventario");

            entity.Property(e => e.inv_Id).HasComment("Identificador único de la tabla Inventarios.");

            entity.Property(e => e.inv_Descripcion)
                .HasMaxLength(250)
                .HasComment("Información relevante que se desee agregar.");

            entity.Property(e => e.inv_EsEliminado).HasComment("Indica si el registro está desactivado permanentemente.");

            entity.Property(e => e.inv_Fecha).HasColumnType("datetime");

            entity.Property(e => e.inv_FechaCrea)
                .HasColumnType("datetime")
                .HasComment("Registra la fecha en que se creó el registro.");

            entity.Property(e => e.inv_FechaModifica)
                .HasColumnType("datetime")
                .HasComment("Registra la última fecha en que se modificó el registro.");

            entity.Property(e => e.inv_UsuarioCrea).HasComment("Indica el identificador del usuario que creó el registro.");

            entity.Property(e => e.inv_UsuarioModifica).HasComment("Indica el identificador del último usuario que modificó el registro.");

            entity.Property(e => e.refg_Id).HasComment("Este es el ID del departamento que hace referencia al primary key de la tabla tbRefugios.");

            entity.HasOne(d => d.inv_UsuarioCreaNavigation)
                .WithMany(p => p.tbInventarios)
                .HasForeignKey(d => d.inv_UsuarioCrea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_tbInventarios_tbUsuarios_usu_Id");

            entity.HasOne(d => d.refg)
                .WithMany(p => p.tbInventarios)
                .HasForeignKey(d => d.refg_Id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<tbInventarios> entity);
    }
}