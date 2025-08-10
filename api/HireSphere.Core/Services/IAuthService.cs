using HireSphere.Core.DTO.Requests;
using HireSphere.Core.DTO.Responses;

namespace HireSphere.Core.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
}
