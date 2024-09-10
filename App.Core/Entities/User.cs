namespace App.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string SaltPassword { get; set; }
        public string HashPassword { get; set; }
        public bool IsAdmin { get; set; }
    }
}
