namespace api.Dto;

public class VerifyDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = null!;
}
