using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PostService.Models;

namespace PostService.DataAccess;

public class PostingDbContext : DbContext
{
    public const string DatabaseFileName = "PostingEfDb.db";

    public PostingDbContext(DbContextOptions<PostingDbContext> options) : base(options) { }

    public DbSet<Posting> Postings { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite($"Data Source={DatabaseFileName};");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Дата створення залишається незмінною після PUT, як у репозиторії ПР-41.
        var createdAt = modelBuilder.Entity<Posting>().Property(p => p.CreatedAt);
        createdAt.HasConversion(value => value,
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
        createdAt.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
