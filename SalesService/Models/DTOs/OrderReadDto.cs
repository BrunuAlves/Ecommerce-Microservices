namespace SalesService.Models.DTOs
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public List<OrderItemReadDto> Items { get; set; }
    }
}