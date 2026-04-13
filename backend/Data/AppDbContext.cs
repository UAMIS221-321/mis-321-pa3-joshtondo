using CryptoBot.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoBot.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatSession>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(36);
            e.Property(x => x.Title).HasMaxLength(255);
            e.HasMany(x => x.Messages)
             .WithOne(x => x.Session)
             .HasForeignKey(x => x.SessionId);
        });

        modelBuilder.Entity<ChatMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SessionId).HasMaxLength(36);
            e.Property(x => x.Role).HasMaxLength(20);
            e.Property(x => x.Content).HasColumnType("TEXT");
        });

        modelBuilder.Entity<KnowledgeDocument>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(255);
            e.Property(x => x.Content).HasColumnType("TEXT");
            e.Property(x => x.Category).HasMaxLength(100);
            e.Property(x => x.Keywords).HasMaxLength(500);
        });
    }
}
