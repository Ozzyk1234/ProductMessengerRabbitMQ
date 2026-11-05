using System.ComponentModel.DataAnnotations;

namespace ProductMessengerRabbitMQ.API.Models
{
    public class ProductModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [MinLength(3)]
        [MaxLength(100)]
        public string? Description { get; set; }
        [Required]
        [Range(1,100000)]
        public decimal Price { get; set; }
    }
}
