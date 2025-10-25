namespace SalesService.Models.DTOs
{
    public class OrderItemReadDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}