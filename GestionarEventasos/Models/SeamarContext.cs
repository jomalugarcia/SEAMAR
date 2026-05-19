using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionarEventasos.Models;

public partial class SeamarContext : DbContext
{
    public SeamarContext()
    {
    }

    public SeamarContext(DbContextOptions<SeamarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Inscripcione> Inscripciones { get; set; }

    public virtual DbSet<Participante> Participantes { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.IdEvento);

            entity.Property(e => e.Lugar).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Inscripcione>(entity =>
        {
            entity.HasKey(e => e.IdInscripcion);

            entity.HasIndex(e => new { e.EventoId, e.ParticipanteId }, "IX_Inscripciones_EventoId_ParticipanteId").IsUnique();

            entity.Property(e => e.FechaInscripcion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Evento).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.EventoId)
                .HasConstraintName("FK_Inscripciones_Eventos");

            entity.HasOne(d => d.Participante).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.ParticipanteId)
                .HasConstraintName("FK_Inscripciones_Participantes");
        });

        modelBuilder.Entity<Participante>(entity =>
        {
            entity.HasKey(e => e.IdParticipante);

            entity.HasIndex(e => e.Email, "NO_EMAILS_REPETIDOS").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.NombreCompleto).HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
