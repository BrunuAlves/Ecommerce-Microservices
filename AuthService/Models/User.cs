namespace AuthService.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Profile { get; set; } // "vendedor" ou "comprador"
    }
}