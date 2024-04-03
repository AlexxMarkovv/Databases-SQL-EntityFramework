// Built-in libraries namespaces

// Third-party packages namespaces
using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data.Common;
using P02_FootballBetting.Data.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
// Solution namespaces

namespace P02_FootballBetting.Data;
public class FootballBettingContext : DbContext
{
    // Use it when developing the App
    // When we test the App locally on our PC
    public FootballBettingContext()
    {

    }

    // Used by Judge
    // Loading of the DbContext with DI -> In real App it is usefull

    public FootballBettingContext(DbContextOptions options)
    : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Color> Colors { get; set; } = null!;
    public DbSet<Town> Towns { get; set; } = null!;

    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;

    public DbSet<PlayerStatistic> PlayersStatistics { get; set; } = null!;

    public DbSet<Game> Games { get; set; } = null!;

    public DbSet<Bet> Bets { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    // Connection configuration
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Set default Connection String
            optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }

    // Fluent API and Entities Config
    // TODO: 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlayerStatistic>(entity =>
        {
            entity.HasKey(ps => new { ps.PlayerId, ps.GameId });
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity
                .HasOne(t => t.PrimaryKitColor)
                .WithMany(c => c.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasOne(t => t.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity
                .HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
