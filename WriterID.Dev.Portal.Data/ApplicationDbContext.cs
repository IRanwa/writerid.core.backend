using WriterID.Dev.Portal.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace WriterID.Dev.Portal.Data;

/// <summary>
/// The application database context for the WriterID portal.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the datasets.
    /// </summary>
    public DbSet<Dataset> Datasets { get; set; }
    
    /// <summary>
    /// Gets or sets the writer identification models.
    /// </summary>
    public DbSet<WriterIdentificationModel> Models { get; set; }
    
    /// <summary>
    /// Gets or sets the writer identification tasks.
    /// </summary>
    public DbSet<WriterIdentificationTask> Tasks { get; set; }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure User entity
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure Dataset entity
        builder.Entity<Dataset>(entity =>
        {
            entity.ToTable("Datasets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ContainerName).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.AnalysisResult).HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Model entity
        builder.Entity<WriterIdentificationModel>(entity =>
        {
            entity.ToTable("Models");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ContainerName).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.TrainingResult).HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.TrainingDataset)
                .WithMany()
                .HasForeignKey(m => m.TrainingDatasetId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure WriterIdentificationTask entity
        builder.Entity<WriterIdentificationTask>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.WriterIds).HasColumnType("jsonb");
            entity.Property(e => e.Result).HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Model)
                .WithMany()
                .HasForeignKey(t => t.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Dataset)
                .WithMany()
                .HasForeignKey(t => t.DatasetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
} 