using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class Breed
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
