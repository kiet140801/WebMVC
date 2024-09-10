namespace WebDemoApplication.Models.Dtos;

public class OrderDto
{
    public int OrderId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedDate { get; set;}
}
