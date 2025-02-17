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

    public bool IsBlocked { get; set; } = false; 
    public bool VerificationRequested { get; set; } = false;
    public bool IsEmailVerified { get; set; } = false;

    [Phone]
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? TelegramUsername { get; set; }
}

