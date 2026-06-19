using Skeleton.Domain.Eunm;

namespace Skeleton.Domain.Entities;

/// <summary>
/// يمثل مستخدم النظام — Admin / Employee / Customer
/// </summary>
public class AppUser : BaseEntity
{
    public string Username     { get; private set; } = null!;
    public string Email        { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role       { get; private set; }

    // ── Email Verification ──────────────────────────────────────
    public bool    IsEmailVerified          { get; private set; } = false;
    public string? EmailVerificationToken   { get; private set; }
    public DateTime? EmailVerificationExpiry{ get; private set; }

    // ── Password Reset ──────────────────────────────────────────
    public string? PasswordResetToken       { get; private set; }
    public DateTime? PasswordResetExpiry    { get; private set; }

    // ── Relations ───────────────────────────────────────────────
    /// <summary>مرتبط بـ Customer entity لو الـ Role = Customer</summary>
    public Guid? CustomerId  { get; private set; }
    /// <summary>مرتبط بـ Employee entity لو الـ Role = Employee</summary>
    public Guid? EmployeeId  { get; private set; }

    public AppUser(string username, string email, string passwordHash,
                   UserRole role, Guid? customerId = null, Guid? employeeId = null)
    {
        Username     = username.Trim();
        Email        = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role         = role;
        CustomerId   = customerId;
        EmployeeId   = employeeId;

        // Auto-generate verification token on creation
        GenerateEmailVerificationToken();
    }

    // ── Methods ─────────────────────────────────────────────────
    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        ClearPasswordResetToken();
    }

    public void GenerateEmailVerificationToken()
    {
        EmailVerificationToken  = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                                         .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        EmailVerificationExpiry = DateTime.UtcNow.AddHours(24);
        IsEmailVerified         = false;
    }

    public bool VerifyEmail(string token)
    {
        if (EmailVerificationToken != token) return false;
        if (EmailVerificationExpiry < DateTime.UtcNow) return false;

        IsEmailVerified          = true;
        EmailVerificationToken   = null;
        EmailVerificationExpiry  = null;
        return true;
    }

    public void GeneratePasswordResetToken()
    {
        PasswordResetToken  = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                                     .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        PasswordResetExpiry = DateTime.UtcNow.AddHours(1);
    }

    public bool IsPasswordResetTokenValid(string token)
        => PasswordResetToken == token && PasswordResetExpiry > DateTime.UtcNow;

    private void ClearPasswordResetToken()
    {
        PasswordResetToken  = null;
        PasswordResetExpiry = null;
    }

    public void UpdateEmail(string newEmail)
    {
        Email = newEmail.Trim().ToLowerInvariant();
        GenerateEmailVerificationToken();
    }
}
