﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using PetsHome.Common.AppPetshomeDbContext;
using PetsHome.Common.Entities;
using System;

namespace PetsHome.Common.AppPetshomeDbContext.Configurations
{
    public partial class tbEntradasDetallesConfiguration : IEntityTypeConfiguration<tbEntradasDetalles>
    {
        public void Configure(EntityTypeBuilder<tbEntradasDetalles> entity)
        {
            entity.HasKey(e => e.entdet_Id);

            entity.ToTable("tbEntradasDetalles", "Inventario");

            entity.Property(e => e.entdet_Id).HasComment("Identificador único de la tabla tbEntradasDetalles.");

            entity.Property(e => e.ent_Id).HasComment("Este es el ID del departamento que hace referencia al primary key de la tabla tbEntradas.");

            entity.Property(e => e.entdet_Cantidad).HasComment("Suma del numero de producto ingresado.");

            entity.Property(e => e.entdet_EsEliminado).HasComment("Indica si el registro está desactivado permanentemente.");

            entity.Property(e => e.entdet_FechaCrea)
                .HasColumnType("datetime")
                .HasComment("Registra la fecha en que se creó el registro.");

            entity.Property(e => e.entdet_FechaModifica)
                .HasColumnType("datetime")
                .HasComment("Registra la última fecha en que se modificó el registro.");

            entity.Property(e => e.entdet_UsuarioCrea).HasComment("Indica el identificador del usuario que creó el registro.");

            entity.Property(e => e.entdet_UsuarioModifica).HasComment("Indica el identificador del último usuario que modificó el registro.");

            entity.Property(e => e.itm_Id).HasComment("Este es el ID del departamento que hace referencia al primary key de la tabla tbItems.");

            entity.HasOne(d => d.ent)
                .WithMany(p => p.tbEntradasDetalles)
                .HasForeignKey(d => d.ent_Id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.entdet_UsuarioCreaNavigation)
                .WithMany(p => p.tbEntradasDetalles)
                .HasForeignKey(d => d.entdet_UsuarioCrea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_tbEntradasDetalles_tbUsuarios_usu_Id");

            entity.HasOne(d => d.itm)
                .WithMany(p => p.tbEntradasDetalles)
                .HasForeignKey(d => d.itm_Id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<tbEntradasDetalles> entity);
    }
}