using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CoreDemo
{
    /// <summary>
    /// 安装 Microsoft.EntityFrameworkCore.Tools NuGet 包
    /// Install-Package Microsoft.EntityFrameworkCore.Tools
    /// 
    /// Scaffold-DbContext "Data Source=.;Initial Catalog=DWQueue;Integrated Security=True"
    /// 
    /// Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer
    /// </summary>
    public partial class DWQueueContext : DbContext
    {
        public DWQueueContext()
        {
        }

        public DWQueueContext(DbContextOptions<DWQueueContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MessageQueue> MessageQueue { get; set; }
        public virtual DbSet<TransactionState> TransactionState { get; set; }
        public virtual DbSet<WholesaleBrand> WholesaleBrand { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DWQueue;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageQueue>(entity =>
            {
                entity.HasKey(e => new { e.MessageId, e.QueueName });

                entity.HasIndex(e => e.LookupField1);

                entity.HasIndex(e => e.LookupField2);

                entity.HasIndex(e => e.LookupField3);

                entity.HasIndex(e => new { e.QueueName, e.IsActive, e.Priority, e.DateActive, e.Sequence, e.MessageId })
                    .HasName("IX_ActiveMessages")
                    .IsUnique();

                entity.Property(e => e.QueueName).HasMaxLength(255);

                entity.Property(e => e.DateActive).HasColumnType("datetime");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateRequestExpires).HasColumnType("datetime");

                entity.Property(e => e.LookupField1).HasMaxLength(255);

                entity.Property(e => e.LookupField2).HasMaxLength(255);

                entity.Property(e => e.LookupField3).HasMaxLength(255);

                entity.Property(e => e.MessageBody).IsRequired();

                entity.Property(e => e.Sequence).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TransactionState>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.OperationId)
                    .HasName("PK_TransactionState")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.DateCreated).HasColumnType("datetime");
            });

            modelBuilder.Entity<WholesaleBrand>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("WHOLESALE_BRAND");

                entity.Property(e => e.AccountSubType)
                    .HasColumnName("ACCOUNT_SUB_TYPE")
                    .HasMaxLength(255);

                entity.Property(e => e.AccountType)
                    .HasColumnName("ACCOUNT_TYPE")
                    .HasMaxLength(255);

                entity.Property(e => e.Brand)
                    .HasColumnName("BRAND")
                    .HasMaxLength(255);

                entity.Property(e => e.DfProduct)
                    .HasColumnName("DF_PRODUCT")
                    .HasMaxLength(255);

                entity.Property(e => e.DfProductNo).HasColumnName("DF_PRODUCT_NO");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
