namespace SalesService.Models.DTOs
{
    public class ProductAvailabilityDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}