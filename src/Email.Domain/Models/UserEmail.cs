namespace Email.Domain.Models
{
    public class UserEmail
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime DateInserted { get; init; } = DateTime.UtcNow;
    }
}
