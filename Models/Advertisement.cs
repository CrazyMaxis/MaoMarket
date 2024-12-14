using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class Advertisement
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public decimal Price { get; set; }

    [Required]
    [ForeignKey("Cat")]
    public Guid CatId { get; set; }
    public Cat Cat { get; set; } = null!;

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
