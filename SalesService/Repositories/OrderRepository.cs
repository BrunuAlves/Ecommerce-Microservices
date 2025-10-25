using Microsoft.EntityFrameworkCore;
using SalesService.Data;
using SalesService.Models;

namespace SalesService.Repositories
{
    public class OrderRepository(SalesDbContext ctx) : IOrderRepository
    {
        private readonly SalesDbContext _ctx = ctx;

        public async Task<Order> AddAsync(Order order)
        {
            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();
            return order;
        }

        public Task<List<Order>> GetByUserIdAsync(int userId) =>
            _ctx.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .ToListAsync();

        public Task<Order> GetByIdAsync(int id, int userId) =>
            _ctx.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        public async Task<bool> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _ctx.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}