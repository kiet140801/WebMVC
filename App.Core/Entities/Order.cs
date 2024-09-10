namespace App.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }
}
