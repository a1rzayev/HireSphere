namespace HireSphere.Core.DTO.Responses;

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public UserResponseDto? User { get; set; }

    public static AuthResponseDto SuccessResponse(string accessToken, string refreshToken, DateTime accessTokenExpiry, DateTime refreshTokenExpiry, UserResponseDto user)
    {
        return new AuthResponseDto
        {
            Success = true,
            Message = "Authentication successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = accessTokenExpiry,
            RefreshTokenExpiry = refreshTokenExpiry,
            User = user
        };
    }

    public static AuthResponseDto FailureResponse(string message)
    {
        return new AuthResponseDto
        {
            Success = false,
            Message = message
        };
    }
}
