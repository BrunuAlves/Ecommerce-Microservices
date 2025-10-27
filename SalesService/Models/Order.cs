using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SalesService.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pendente"; 
        public int UserId { get; set; }
        public List<OrderItem> Items { get; set; } = [];
    }

    public class OrderItem
    {   
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public int OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        [JsonIgnore]
        public Order Order { get; set; }
    }
}
