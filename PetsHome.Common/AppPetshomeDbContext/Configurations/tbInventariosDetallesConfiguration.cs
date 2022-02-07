﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using PetsHome.Common.AppPetshomeDbContext;
using PetsHome.Common.Entities;
using System;

namespace PetsHome.Common.AppPetshomeDbContext.Configurations
{
    public partial class tbInventariosDetallesConfiguration : IEntityTypeConfiguration<tbInventariosDetalles>
    {
        public void Configure(EntityTypeBuilder<tbInventariosDetalles> entity)
        {
            entity.HasKey(e => e.invdet_Id);

            entity.ToTable("tbInventariosDetalles", "Inventario");

            entity.Property(e => e.invdet_Id).HasComment("Identificador único de la tabla Inventarios Detalles.");

            entity.Property(e => e.invdet_EsEliminado).HasComment("Indica si el registro está desactivado permanentemente.");

            entity.Property(e => e.invdet_FechaCrea)
                .HasColumnType("datetime")
                .HasComment("Registra la fecha en que se creó el registro.");

            entity.Property(e => e.invdet_FechaModifica)
                .HasColumnType("datetime")
                .HasComment("Registra la última fecha en que se modificó el registro.");

            entity.Property(e => e.invdet_UsuarioCrea).HasComment("Indica el identificador del usuario que creó el registro.");

            entity.Property(e => e.invdet_UsuarioModifica).HasComment("Indica el identificador del último usuario que modificó el registro.");

            entity.HasOne(d => d.inv)
                .WithMany(p => p.tbInventariosDetalles)
                .HasForeignKey(d => d.inv_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbInventariosDetalles_tbInventarios");

            entity.HasOne(d => d.invdet_UsuarioCreaNavigation)
                .WithMany(p => p.tbInventariosDetalles)
                .HasForeignKey(d => d.invdet_UsuarioCrea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_tbInventariosDetalles_tbUsuarios_usu_Id");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<tbInventariosDetalles> entity);
    }
}
