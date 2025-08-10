using HireSphere.Core.DTO.Responses;
using HireSphere.Core.Models;

namespace HireSphere.Core.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    RefreshToken CreateRefreshToken(Guid userId);
    bool ValidateAccessToken(string token, out string? userId);
    bool ValidateRefreshToken(string token);
    Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
