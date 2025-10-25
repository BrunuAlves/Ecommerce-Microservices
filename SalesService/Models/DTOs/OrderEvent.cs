namespace SalesService.Models.DTOs
{
    public class OrderEvent
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public OrderItemEvent[] Items { get; set; }
    }

    public class OrderItemEvent
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}