using Microsoft.EntityFrameworkCore;
using TestChat.Server.Data.Entities;

namespace TestChat.Server.Data;

public class ChatDbContext : DbContext
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<SentimentAnalysis> SentimentAnalyses { get; set; }

    public ChatDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(builder =>
        {
            builder.Property(m => m.SenderName).HasMaxLength(32);
            builder.Property(m => m.RecipientName).HasMaxLength(32);
            builder.Property(m => m.Text).HasMaxLength(1024);
        });

        modelBuilder.Entity<SentimentAnalysis>(builder =>
        {
            builder.HasKey(sa => sa.MessageId);

            builder.HasOne<Message>(sa => sa.Message)
                .WithOne(m => m.SentimentAnalysis)
                .HasForeignKey<SentimentAnalysis>(sa => sa.MessageId);
        });

        base.OnModelCreating(modelBuilder);
    }
}