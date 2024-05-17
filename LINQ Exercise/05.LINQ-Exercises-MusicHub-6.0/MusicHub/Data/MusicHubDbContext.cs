namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext() { }

        public MusicHubDbContext(DbContextOptions options) : base(options) { }


        public DbSet<Album> Albums { get; set; } = null!;

        public DbSet<Performer> Performers { get; set; } = null!;

        public DbSet<Producer> Producers { get; set; } = null!;

        public DbSet<Song> Songs { get; set; } = null!;

        public DbSet<SongPerformer> SongsPerformers { get; set; } = null!;

        public DbSet<Writer> Writers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SongPerformer>()
                .HasKey(sp => new { sp.SongId, sp.PerformerId });

            modelBuilder.Entity<SongPerformer>()
                .HasOne(s => s.Song)
                .WithMany(s => s.SongPerformers)
                .HasForeignKey(s => s.SongId);

            modelBuilder.Entity<SongPerformer>()
                .HasOne(p => p.Performer)
                .WithMany(p => p.PerformerSongs)
                .HasForeignKey(p => p.PerformerId);
        }
    }
}
