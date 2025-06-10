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

        // Configure relationships with cascade delete behavior
        builder.Entity<Dataset>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<WriterIdentificationModel>(entity =>
        {
            entity.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.TrainingDataset)
                .WithMany()
                .HasForeignKey(m => m.TrainingDatasetId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<WriterIdentificationTask>(entity =>
        {
            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Model relationship is optional (nullable) when using default model
            entity.HasOne(t => t.Model)
                .WithMany()
                .HasForeignKey(t => t.ModelId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(t => t.Dataset)
                .WithMany()
                .HasForeignKey(t => t.DatasetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
} 