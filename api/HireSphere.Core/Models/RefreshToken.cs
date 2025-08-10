namespace HireSphere.Core.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public RefreshToken()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public RefreshToken(string token, DateTime expiryDate, Guid userId)
    {
        Token = token;
        ExpiryDate = expiryDate;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public bool IsActive => !IsExpired && !IsRevoked;
}
