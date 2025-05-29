using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Barangay.SchemaModels;

public partial class BarangayContext : DbContext
{
    private readonly IConfiguration _configuration;

    public BarangayContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public BarangayContext(DbContextOptions<BarangayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasIndex(e => e.DoctorId, "IX_Appointments_DoctorId");

            entity.HasIndex(e => e.PatientId, "IX_Appointments_PatientId");

            entity.HasIndex(e => e.PatientUserId, "IX_Appointments_PatientUserId");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Allergies).HasMaxLength(500);
            entity.Property(e => e.AppointmentTimeInput).HasDefaultValue("");
            entity.Property(e => e.AttachmentPath).HasMaxLength(500);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CurrentMedications).HasMaxLength(500);
            entity.Property(e => e.DependentFullName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DoctorId).HasDefaultValue("");
            entity.Property(e => e.EmergencyContact).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactNumber).HasMaxLength(20);
            entity.Property(e => e.FamilyNumber).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.HealthFacilityId).HasMaxLength(100);
            entity.Property(e => e.Instructions).HasMaxLength(1000);
            entity.Property(e => e.MedicalHistory).HasMaxLength(1000);
            entity.Property(e => e.PatientId).HasDefaultValue("");
            entity.Property(e => e.PatientName).HasMaxLength(100);
            entity.Property(e => e.Prescription).HasMaxLength(1000);
            entity.Property(e => e.ReasonForVisit).HasMaxLength(500);
            entity.Property(e => e.RelationshipToDependent).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
