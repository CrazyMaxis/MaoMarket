using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;
public class Post
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [Required]
    public List<string> Hashtags { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}