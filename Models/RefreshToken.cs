namespace api.Models;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
