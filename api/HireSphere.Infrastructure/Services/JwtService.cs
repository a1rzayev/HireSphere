using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HireSphere.Core.DTO.Responses;
using HireSphere.Core.Enums;
using HireSphere.Core.Models;
using HireSphere.Core.Services;
using HireSphere.Infrastructure.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HireSphere.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly HireSphereDbContext _context;

    public JwtService(IOptions<JwtSettings> jwtSettings, HireSphereDbContext context)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("UserId", user.Id.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public RefreshToken CreateRefreshToken(Guid userId)
    {
        var refreshToken = new RefreshToken(
            GenerateRefreshToken(),
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            userId
        );

        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        return refreshToken;
    }

    public bool ValidateAccessToken(string token, out string? userId)
    {
        userId = null;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ValidateRefreshToken(string token)
    {
        var refreshToken = _context.RefreshTokens
            .FirstOrDefault(rt => rt.Token == token);

        return refreshToken?.IsActive == true;
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        if (!ValidateAccessToken(accessToken, out var userId) || !ValidateRefreshToken(refreshToken))
        {
            return null;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId!));

        if (user == null)
        {
            return null;
        }

        var existingRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (existingRefreshToken == null || !existingRefreshToken.IsActive)
        {
            return null;
        }

        existingRefreshToken.IsRevoked = true;
        _context.RefreshTokens.Update(existingRefreshToken);

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = CreateRefreshToken(user.Id);

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
            newAccessToken,
            newRefreshToken.Token,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            newRefreshToken.ExpiryDate,
            userResponse
        );
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
