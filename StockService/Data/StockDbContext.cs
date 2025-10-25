using Microsoft.EntityFrameworkCore;
using StockService.Models;

namespace StockService.Data
{
    public class StockDbContext(DbContextOptions<StockDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        //especificar precisão (precision) e escala (scale) apropriadas para evitar perda de dados.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)"); // define precisão e escala
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
