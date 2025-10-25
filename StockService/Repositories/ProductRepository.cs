using Microsoft.EntityFrameworkCore;
using StockService.Data;
using StockService.Models;

namespace StockService.Repositories
{
    public class ProductRepository(StockDbContext ctx) : IProductRepository
    {
        private readonly StockDbContext _ctx = ctx;

        public async Task<Product> AddAsync(Product p)
        {
            _ctx.Products.Add(p);
            await _ctx.SaveChangesAsync();
            return p;
        }

        public Task<Product> GetByIdAsync(int id) =>
            _ctx.Products.FirstOrDefaultAsync(x => x.Id == id);

        public Task<List<Product>> GetAllAsync() =>
            _ctx.Products.ToListAsync();

        //NOVO MÉTODO: Buscar produtos por SellerId
        public Task<List<Product>> GetBySellerIdAsync(int sellerId) =>
            _ctx.Products.Where(p => p.SellerId == sellerId).ToListAsync();

        //NOVO MÉTODO: Atualizar produto apenas se for do vendedor
        public async Task<bool> UpdateAsync(Product p, int sellerId)
        {
            var existing = await _ctx.Products.FirstOrDefaultAsync(x => x.Id == p.Id && x.SellerId == sellerId);
            if (existing == null) return false;

            existing.Name = p.Name ?? existing.Name;
            existing.Description = p.Description ?? existing.Description;
            existing.Price = p.Price != 0 ? p.Price : existing.Price;
            existing.Quantity = p.Quantity != 0 ? p.Quantity : existing.Quantity;

            _ctx.Products.Update(existing);
            await _ctx.SaveChangesAsync();
            return true;
        }

        //NOVO MÉTODO: Deletar produto apenas se for do vendedor
        public async Task<bool> DeleteAsync(int id, int sellerId)
        {
            var existing = await _ctx.Products.FirstOrDefaultAsync(x => x.Id == id && x.SellerId == sellerId);
            if (existing == null) return false;

            _ctx.Products.Remove(existing);
            await _ctx.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> ReduceAsync(int productId, int quantity)
        {
            var product = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return false;
            if (product.Quantity < quantity) return false;

            product.Quantity -= quantity;

            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}