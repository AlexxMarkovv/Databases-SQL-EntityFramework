using Microsoft.EntityFrameworkCore;
using CarDealer.Models;

namespace CarDealer.Data
{
    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {
        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Part> Parts { get; set; } = null!;
        public DbSet<PartCar> PartsCars { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PartCar>(e =>
            {
                e.HasKey(k => new { k.CarId, k.PartId });
            });

            modelBuilder.Entity<Part>()
                .Property(p => p.Price)
                .HasColumnType("decimal (18,2)");

            modelBuilder.Entity<Sale>()
                .Property(s => s.Discount)
                .HasColumnType("decimal (18,2)");
        }
    }
}
