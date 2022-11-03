using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Skillbox_Homework_17
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

        public virtual DbSet<ClassLocalDb> ClassLocalDbs { get; set; }
        public virtual DbSet<LocalDbtable> LocalDbtables { get; set; }
        public virtual DbSet<MigrationHistory> MigrationHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=localBase;User ID=qwerty;Password=12345;Trusted_Connection=false;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ClassLocalDb>(entity =>
            {
                entity.ToTable("ClassLocalDBs");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<LocalDbtable>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("localDBTable");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Имя)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.НомерТелефона)
                    .HasMaxLength(255)
                    .HasColumnName("Номер_телефона");

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
