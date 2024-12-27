using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.Models;

public class CatPhoto
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey("Cat")]
    public Guid CatId { get; set; }
    
    [JsonIgnore]
    public Cat Cat { get; set; } = null!;

    [Required]
    public string Image { get; set; } = string.Empty;
}
