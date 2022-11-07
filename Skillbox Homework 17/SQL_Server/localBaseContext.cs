using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Skillbox_Homework_17.SQL_Server
{
    public partial class localBaseContext : DbContext
    {
        public localBaseContext()
        {
        }

        public localBaseContext(DbContextOptions<localBaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ClassLocalDbs> ClassLocalDbs { get; set; }
        public virtual DbSet<LocalDbtable> LocalDbtable { get; set; }
        public virtual DbSet<MigrationHistory> MigrationHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=localBase;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassLocalDbs>(entity =>
            {
                entity.ToTable("ClassLocalDBs");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<LocalDbtable>(entity =>
            {
                //entity.HasNoKey();

                entity.ToTable("localDBTable");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Имя)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.НомерТелефона)
                    .HasColumnName("Номер_телефона")
                    .HasMaxLength(255);

                entity.Property(e => e.Отчество)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Фамилия)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<MigrationHistory>(entity =>
            {
                entity.HasKey(e => new { e.MigrationId, e.ContextKey })
                    .HasName("PK_dbo.__MigrationHistory");

                entity.ToTable("__MigrationHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ContextKey).HasMaxLength(300);

                entity.Property(e => e.Model).IsRequired();

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
