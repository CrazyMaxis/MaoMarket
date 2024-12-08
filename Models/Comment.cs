using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class Comment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; } = null!;

    [Required]
    public Guid PostId { get; set; }

    [ForeignKey(nameof(PostId))]
    public Post? Post { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Body { get; set; } = string.Empty;
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
