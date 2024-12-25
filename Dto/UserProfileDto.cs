using System.ComponentModel.DataAnnotations;

public class UpdateProfileDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [Phone]
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? TelegramUsername { get; set; }
}
