using Microsoft.EntityFrameworkCore;
using SalesService.Models;

namespace SalesService.Data
{
    public class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

    }
}