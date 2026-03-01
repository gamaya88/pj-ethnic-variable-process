using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PJ.Oti.ProcesaNuevosExpedientes.Service.Model.HelpDeskDb;

public partial class HelpDeskDbContext : DbContext
{
    public HelpDeskDbContext()
    {
    }

    public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<IngresoExpediente> IngresoExpedientes { get; set; }

    public virtual DbSet<IngresoMesaDeParte> IngresoMesaDePartes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS2022;Database=HELPDESK;Trusted_Connection=False;uid=sa;password=123..abc;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IngresoExpediente>(entity =>
        {
            entity.HasKey(e => e.IexId);

            entity.ToTable("IngresoExpediente", "Hdk");

            entity.Property(e => e.IexId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IexFraseActivacion).HasMaxLength(256);
            entity.Property(e => e.IexHoraHost).HasColumnType("datetime");
            entity.Property(e => e.IexHost).HasMaxLength(32);
            entity.Property(e => e.IexSede)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.IexUsuario).HasMaxLength(32);
            entity.Property(e => e.SecActivo).HasDefaultValue(true);
            entity.Property(e => e.SecFechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.SecFechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SecFechaEliminacion).HasColumnType("datetime");
            entity.Property(e => e.SecUsuarioActualizacionId).HasMaxLength(64);
            entity.Property(e => e.SecUsuarioCreacionId)
                .HasMaxLength(64)
                .HasDefaultValueSql("(suser_name())");
            entity.Property(e => e.SecUsuarioEliminacionId).HasMaxLength(64);
        });

        modelBuilder.Entity<IngresoMesaDeParte>(entity =>
        {
            entity.HasKey(e => e.ImpId);

            entity.ToTable("IngresoMesaDePartes", "Pju");

            entity.Property(e => e.ImpId).ValueGeneratedNever();
            entity.Property(e => e.CodigoInstancia)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DescripcionSede).HasMaxLength(60);
            entity.Property(e => e.Distrito)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Especialidad)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Expediente).HasMaxLength(50);
            entity.Property(e => e.FechaAsociacion).HasColumnType("datetime");
            entity.Property(e => e.FechaIngreso).HasColumnType("datetime");
            entity.Property(e => e.MotivoIngreso).HasMaxLength(150);
            entity.Property(e => e.NombreInstancia).HasMaxLength(60);
            entity.Property(e => e.OrganoJurisdiccional)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Provincia)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SecActivo).HasDefaultValue(true);
            entity.Property(e => e.SecFechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.SecFechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SecFechaEliminacion).HasColumnType("datetime");
            entity.Property(e => e.SecUsuarioActualizacionId).HasMaxLength(64);
            entity.Property(e => e.SecUsuarioCreacionId)
                .HasMaxLength(64)
                .HasDefaultValueSql("(suser_name())");
            entity.Property(e => e.SecUsuarioEliminacionId).HasMaxLength(64);
            entity.Property(e => e.Sede)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Usuario).HasMaxLength(15);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
