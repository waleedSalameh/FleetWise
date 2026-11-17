namespace FleetWise.BLL.DTOs.Identity;

public sealed class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}
