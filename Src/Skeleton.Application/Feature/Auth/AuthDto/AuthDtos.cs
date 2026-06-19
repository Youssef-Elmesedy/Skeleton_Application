namespace Skeleton.Application.Feature.Auth.AuthDto;

// ── Login ────────────────────────────────────────────────────────
public record LoginDto(string Username, string Password);

public record LoginResponseDto(
    string   Token,
    string   Username,
    string   Email,
    string   Role,
    bool     IsEmailVerified,
    DateTime ExpiresAt
);

// ── Register ─────────────────────────────────────────────────────
public record RegisterDto(
    string  Username,
    string  Email,
    string  Password,
    string  Role,              // "Admin" | "Employee" | "Customer"
    Guid?   CustomerId  = null,
    Guid?   EmployeeId  = null
);

// ── Change Password ───────────────────────────────────────────────
public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);

// ── Forgot Password ───────────────────────────────────────────────
public record ForgotPasswordDto(string Email);

// ── Reset Password ────────────────────────────────────────────────
public record ResetPasswordDto(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmNewPassword
);

// ── Email Verification ────────────────────────────────────────────
public record VerifyEmailDto(string Email, string Token);
public record ResendVerificationDto(string Email);

// ── Generic success message ───────────────────────────────────────
public record MessageResponseDto(string Message);
