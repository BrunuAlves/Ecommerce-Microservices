using SalesService.Models;

namespace SalesService.Repositories
{
    public interface IOrderValidator
    {
        Task<(bool IsValid, string Error)> ValidateStockAsync(Order order);
    }
}