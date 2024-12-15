namespace api.Dto;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public bool IsBlocked { get; set; }
    public bool VerificationRequested { get; set; }
}