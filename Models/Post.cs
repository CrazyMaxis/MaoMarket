using System.ComponentModel.DataAnnotations;

namespace api.Models;
public class Post
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [Required]
    public List<string> Hashtags { get; set; } = new();
    
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public string Image { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
