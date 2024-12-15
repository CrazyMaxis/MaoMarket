using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class VerificationCode
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(6)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiryTime { get; set; }
}