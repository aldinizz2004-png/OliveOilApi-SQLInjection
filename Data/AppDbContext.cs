using Microsoft.EntityFrameworkCore;
using OliveOilApi.Models;

namespace OliveOilApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<OliveOilProduct> OliveOilProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OliveOilProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(50);
            entity.Property(e => e.PricePerLiter).HasPrecision(10, 2);

            entity.HasData(
                new OliveOilProduct { Id = 1, Name = "Nabali Extra Virgin", Region = "Nablus",   PricePerLiter = 45.00m, QuantityInStock = 120 },
                new OliveOilProduct { Id = 2, Name = "Souri Premium",       Region = "Jenin",    PricePerLiter = 50.00m, QuantityInStock = 80  },
                new OliveOilProduct { Id = 3, Name = "Rumi Classic",        Region = "Tulkarm",  PricePerLiter = 38.50m, QuantityInStock = 200 },
                new OliveOilProduct { Id = 4, Name = "Nabali Cold Pressed", Region = "Salfit",   PricePerLiter = 55.00m, QuantityInStock = 60  },
                new OliveOilProduct { Id = 5, Name = "Souri Organic",       Region = "Ramallah", PricePerLiter = 60.00m, QuantityInStock = 40  }
            );
        });
    }
}
