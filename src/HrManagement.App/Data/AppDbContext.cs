using HrManagement.App.Models;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.App.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Absence> Absences => Set<Absence>();

    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(item => item.Name).IsUnique();
            entity.HasIndex(item => item.Code).IsUnique();
            entity.Property(item => item.Name).HasMaxLength(120);
            entity.Property(item => item.Code).HasMaxLength(20);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasIndex(item => item.Name).IsUnique();
            entity.Property(item => item.Name).HasMaxLength(120);
            entity.Property(item => item.SalaryRate).HasPrecision(12, 2);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(item => item.PersonnelNumber).IsUnique();
            entity.HasIndex(item => item.Email).IsUnique();
            entity.Property(item => item.PersonnelNumber).HasMaxLength(20);
            entity.Property(item => item.LastName).HasMaxLength(80);
            entity.Property(item => item.FirstName).HasMaxLength(80);
            entity.Property(item => item.MiddleName).HasMaxLength(80);
            entity.Property(item => item.Email).HasMaxLength(160);
            entity.Property(item => item.Phone).HasMaxLength(30);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(item => item.Department)
                .WithMany(item => item.Employees)
                .HasForeignKey(item => item.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Position)
                .WithMany(item => item.Employees)
                .HasForeignKey(item => item.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Absence>(entity =>
        {
            entity.Property(item => item.Type).HasMaxLength(40);
            entity.Property(item => item.DocumentNumber).HasMaxLength(60);
            entity.Property(item => item.Comment).HasMaxLength(500);
            entity.HasOne(item => item.Employee)
                .WithMany(item => item.Absences)
                .HasForeignKey(item => item.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasIndex(item => item.Login).IsUnique();
            entity.Property(item => item.Login).HasMaxLength(60);
            entity.Property(item => item.DisplayName).HasMaxLength(160);
            entity.Property(item => item.Role).HasMaxLength(40);
            entity.Property(item => item.PasswordHash).HasMaxLength(256);
            entity.Property(item => item.PasswordSalt).HasMaxLength(128);
        });

        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.Property(item => item.Action).HasMaxLength(80);
            entity.Property(item => item.EntityName).HasMaxLength(80);
            entity.Property(item => item.EntityId).HasMaxLength(80);
            entity.HasIndex(item => item.CreatedAt);
        });
    }
}
