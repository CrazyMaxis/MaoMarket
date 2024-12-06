using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "User";

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
