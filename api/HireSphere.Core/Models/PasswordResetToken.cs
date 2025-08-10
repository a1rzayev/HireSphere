namespace HireSphere.Core.Models;

public class PasswordResetToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public PasswordResetToken()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public PasswordResetToken(string token, DateTime expiryDate, Guid userId)
    {
        Token = token;
        ExpiryDate = expiryDate;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public bool IsValid => !IsExpired && !IsUsed;
}
