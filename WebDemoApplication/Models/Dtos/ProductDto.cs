using System.ComponentModel.DataAnnotations;

namespace WebDemoApplication.Models.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        [Required ,MaxLength(100)]
        public string Name { get; set; } = "";
        [Required]
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
