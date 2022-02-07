﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using PetsHome.Common.AppPetshomeDbContext;
using PetsHome.Common.Entities;
using System;

namespace PetsHome.Common.AppPetshomeDbContext.Configurations
{
    public partial class tbPersonasConfiguration : IEntityTypeConfiguration<tbPersonas>
    {
        public void Configure(EntityTypeBuilder<tbPersonas> entity)
        {
            entity.HasKey(e => e.per_Id)
                .HasName("PK__tbPerson__32A0223FDCD73405");

            entity.ToTable("tbPersonas", "General");

            entity.HasIndex(e => e.per_Correo, "UQ_tbPersonas_Correo")
                .IsUnique();

            entity.HasIndex(e => e.per_Identidad, "UQ_tbPersonas_Identidad")
                .IsUnique();

            entity.Property(e => e.per_Id).HasComment("Identificador único de la tabla Personas.");

            entity.Property(e => e.per_ApellidoMaterno).HasMaxLength(50);

            entity.Property(e => e.per_ApellidoPaterno)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.per_Correo)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.per_Domicilio).HasMaxLength(100);

            entity.Property(e => e.per_FechaCrea)
                .HasColumnType("datetime")
                .HasComment("Registra la fecha en que se creó el registro.");

            entity.Property(e => e.per_FechaModifica)
                .HasColumnType("datetime")
                .HasComment("Registra la última fecha en que se modificó el registro.");

            entity.Property(e => e.per_FechaNacimiento).HasColumnType("date");

            entity.Property(e => e.per_Identidad)
                .IsRequired()
                .HasMaxLength(13)
                .IsUnicode(false);

            entity.Property(e => e.per_PrimerNombre)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.per_SegundoNombre).HasMaxLength(50);

            entity.Property(e => e.per_Telefono)
                .IsRequired()
                .HasMaxLength(8)
                .IsUnicode(false);

            entity.Property(e => e.per_UsuarioModifica).HasComment("Indica el identificador del último usuario que modificó el registro.");

            entity.HasOne(d => d.per_UsuarioCreaNavigation)
                .WithMany(p => p.tbPersonas)
                .HasForeignKey(d => d.per_UsuarioCrea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_General_tbPersonas_tbUsuarios_usu_Id");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<tbPersonas> entity);
    }
}
