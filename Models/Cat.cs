using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.Models;

public class Cat
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(6)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [ForeignKey("Breed")]
    public Guid BreedId { get; set; }
    public Breed Breed { get; set; } = null!;

    [Required]
    public DateTime BirthDate { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? FatherId { get; set; }
    public Guid? MotherId { get; set; }

    [JsonIgnore]
    public List<CatPhoto> Photos { get; set; } = new();
}
