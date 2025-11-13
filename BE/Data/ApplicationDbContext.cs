using System;
using BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BE.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var movieBuilder = modelBuilder.Entity<Movie>();
        movieBuilder.ToTable("movies");
        movieBuilder.HasKey(m => m.Id);
        movieBuilder.Property(m => m.Id)
                    .HasColumnName("id");
        movieBuilder.Property(m => m.Title)
                    .HasColumnName("title")
                    .IsRequired()
                    .HasMaxLength(200);
        movieBuilder.Property(m => m.Genre)
                    .HasColumnName("genre")
                    .HasMaxLength(100);
        movieBuilder.Property(m => m.Rating)
                    .HasColumnName("rating");
        movieBuilder.Property(m => m.PosterUrl)
                    .HasColumnName("poster_url")
                    .HasMaxLength(500);
        movieBuilder.Property(m => m.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .ValueGeneratedOnAdd();
        movieBuilder.Property(m => m.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .ValueGeneratedOnAddOrUpdate();

        movieBuilder.HasIndex(m => m.Title).HasDatabaseName("idx_movies_title");
        movieBuilder.HasIndex(m => m.Genre).HasDatabaseName("idx_movies_genre");

        movieBuilder.ToTable(t => t.HasCheckConstraint("CK_movies_rating_range", "rating BETWEEN 1 AND 5"));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps(ChangeTracker);
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps(ChangeTracker);
        return base.SaveChanges();
    }

    private static void UpdateTimestamps(ChangeTracker changeTracker)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in changeTracker.Entries<Movie>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(m => m.CreatedAt).IsModified = false;
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }
}

