using Microsoft.EntityFrameworkCore;
using LanggoNew.Models;
using LanggoNew.Shared.Models;

namespace LanggoNew;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Dictionary> Dictionaries { get; set; }
    public DbSet<DictionaryWord> DictionaryWords { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<RoundUser> RoundUsers { get; set; }
    public DbSet<EmailVerificationToken>  EmailVerificationTokens { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.ToTable("Users");

        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(e => e.FullName)
            .HasMaxLength(200);

        entity.Property(e => e.Avatar)
            .HasMaxLength(500);

        entity.Property(e => e.LearningLanguage)
            .HasMaxLength(10);

        entity.Property(e => e.NativeLanguage)
            .HasMaxLength(10);

    });

    modelBuilder.Entity<Dictionary>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.ToTable("Dictionaries");

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(e => e.LangFrom)
            .IsRequired()
            .HasMaxLength(10);

        entity.Property(e => e.LangTo)
            .IsRequired()
            .HasMaxLength(10);

        entity.Property(e => e.Description)
            .HasMaxLength(1000);

        entity.HasOne(d => d.Owner)
            .WithMany(u => u.Dictionaries)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

    });

    modelBuilder.Entity<DictionaryWord>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.ToTable("DictionaryWords");

        entity.Property(e => e.Original)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.Translation)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.Example)
            .HasMaxLength(1000);

        entity.HasOne(dw => dw.Dictionary)
            .WithMany(d => d.Words)
            .HasForeignKey(dw => dw.DictionaryId)
            .OnDelete(DeleteBehavior.Cascade);

    });

    modelBuilder.Entity<Game>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.ToTable("Games");

        entity.Property(e => e.Mode)
            .IsRequired()
            .HasMaxLength(50);

        entity.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasOne(g => g.Dictionary)
            .WithMany(d => d.Games)
            .HasForeignKey(g => g.DictionaryId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(g => g.Winner)
            .WithMany(u => u.WonGames)
            .HasForeignKey(g => g.WinnerId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(e => e.DictionaryId);
        entity.HasIndex(e => e.WinnerId);
    });
    
    modelBuilder.Entity<Round>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.ToTable("Rounds");

        entity.Property(e => e.CorrectWord)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasOne(r => r.Game)
            .WithMany(g => g.Rounds)
            .HasForeignKey(r => r.GameId)
            .OnDelete(DeleteBehavior.Cascade);

    });
    
    modelBuilder.Entity<RoundUser>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.ToTable("RoundUsers");

        entity.HasOne(ru => ru.User)
            .WithMany(u => u.RoundUsers)
            .HasForeignKey(ru => ru.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(ru => ru.Round)
            .WithMany(r => r.RoundUsers)
            .HasForeignKey(ru => ru.RoundId)
            .OnDelete(DeleteBehavior.Cascade);
    });
    
    modelBuilder.Entity<EmailVerificationToken>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.ToTable("EmailVerificationTokens");

        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
    });
}
}
