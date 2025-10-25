using SalesService.Services;
using SalesService.Models;

namespace SalesService.Repositories
{
    public class OrderValidator : IOrderValidator
    {
        private readonly InventoryClient _inventory;

        public OrderValidator(InventoryClient inventory)
        {
            _inventory = inventory;
        }

        public async Task<(bool IsValid, string Error)> ValidateStockAsync(Order order)
        {
            var agrupados = order.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity),
                    Items = g.ToList()
                });

            foreach (var grupo in agrupados)
            {
                var produto = await _inventory.GetAvailabilityAsync(grupo.ProductId);
                if (produto == null)
                    return (false, $"Produto {grupo.ProductId} não encontrado no estoque.");

                if (produto.Quantity < grupo.TotalQuantity)
                    return (false, $"Estoque insuficiente para o produto {grupo.ProductId}. Requisitado: {grupo.TotalQuantity}, disponível: {produto.Quantity}");

                // Atribui o preço real do produto
                foreach (var item in grupo.Items)
                {
                    item.Price = produto.Price;
                }
            }

            return (true, null);
        }
    }
}