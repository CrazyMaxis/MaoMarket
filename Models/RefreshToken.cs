using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiryTime { get; set; }

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
