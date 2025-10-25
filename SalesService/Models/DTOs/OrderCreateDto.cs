namespace SalesService.Models.DTOs
{
    public class OrderCreateDto
    {
        public List<OrderItemCreateDto> Items { get; set; } = [];
    }
}