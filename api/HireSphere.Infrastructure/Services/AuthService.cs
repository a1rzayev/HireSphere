using BCrypt.Net;
using HireSphere.Core.DTO.Requests;
using HireSphere.Core.DTO.Responses;
using HireSphere.Core.Enums;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;
using HireSphere.Core.Services;
using HireSphere.Infrastructure.ORM;
using Microsoft.EntityFrameworkCore;

namespace HireSphere.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserEfCoreRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly HireSphereDbContext _context;

    public AuthService(IUserEfCoreRepository userRepository, IJwtService jwtService, HireSphereDbContext context)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequest.Email);
        
        if (user == null)
        {
            return AuthResponseDto.FailureResponse("Invalid email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            return AuthResponseDto.FailureResponse("Invalid email or password");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.CreateRefreshToken(user.Id);

        var userResponse = new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Phone = user.Phone,
            Role = (int)user.Role,
            IsEmailConfirmed = user.IsEmailConfirmed,
            CreatedAt = user.CreatedAt
        };

        return AuthResponseDto.SuccessResponse(
            accessToken,
            refreshToken.Token,
            DateTime.UtcNow.AddMinutes(15), 
            refreshToken.ExpiryDate,
            userResponse
        );
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerRequest.Email);
        if (existingUser != null)
        {
            return AuthResponseDto.FailureResponse("User with this email already exists");
        }

        if (!IsPasswordComplex(registerRequest.Password))
        {
            return AuthResponseDto.FailureResponse("Password does not meet complexity requirements");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

        var user = new User(
            Guid.NewGuid(),
            registerRequest.Email,
            passwordHash,
            registerRequest.Role,
            registerRequest.Name,
            registerRequest.Surname,
            registerRequest.Phone
        );

        await _userRepository.AddAsync(user);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.CreateRefreshToken(user.Id);

        var userResponse = new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Phone = user.Phone,
            Role = (int)user.Role,
            IsEmailConfirmed = user.IsEmailConfirmed,
            CreatedAt = user.CreatedAt
        };

        return AuthResponseDto.SuccessResponse(
            accessToken,
            refreshToken.Token,
            DateTime.UtcNow.AddMinutes(15),
            refreshToken.ExpiryDate,
            userResponse
        );
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var result = await _jwtService.RefreshTokenAsync(accessToken, refreshToken);
        
        if (result == null)
        {
            return AuthResponseDto.FailureResponse("Invalid or expired refresh token");
        }

        return result;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        await _jwtService.RevokeRefreshTokenAsync(refreshToken);
        return true;
    }

    private bool IsPasswordComplex(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;
        
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;

        return true;
    }
}
