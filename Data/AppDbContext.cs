using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Models.Entities;

namespace MilkCollector.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Farmer> Farmers => Set<Farmer>();
        public DbSet<FarmerFatKindSegment> FarmerFatKindSegments => Set<FarmerFatKindSegment>();
        public DbSet<FatRateRule> FatRateRules => Set<FatRateRule>();
        public DbSet<Collection> Collections => Set<Collection>();
        public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
        public DbSet<Settlement> Settlements => Set<Settlement>();
        public DbSet<SettlementLine> SettlementLines => Set<SettlementLine>();
        public DbSet<PashuAahar> PashuAahars => Set<PashuAahar>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Phone).IsUnique();
            });

            // Manager
            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasIndex(m => m.Phone).IsUnique();
            });

            // Farmer
            modelBuilder.Entity<Farmer>(entity =>
            {
                entity.HasIndex(f => f.Name);
            });

            // Collection unique constraint
            modelBuilder.Entity<Collection>(entity =>
            {
                entity.HasIndex(c => new { c.FarmerId, c.CollectionDate, c.Shift })
                    .IsUnique();
            });

            // FatRateRule
            modelBuilder.Entity<FatRateRule>(entity =>
            {
                entity.Property(r => r.RupeesPerFatPointPerLiterPaise)
                    .IsRequired();
            });

            // Seed Manager
            modelBuilder.Entity<Manager>().HasData(new Manager
            {
                Id = 1,
                Phone = "8889647593",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                Role = "manager",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

        }
    }
}
