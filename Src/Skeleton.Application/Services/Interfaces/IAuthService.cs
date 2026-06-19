using Skeleton.Application.Feature.Auth.AuthDto;

namespace Skeleton.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto>    LoginAsync(LoginDto dto, CancellationToken ct);
    Task<LoginResponseDto>    RegisterAsync(RegisterDto dto, CancellationToken ct);

    // ── Email Verification ───────────────────────────────────────
    Task<MessageResponseDto>  VerifyEmailAsync(VerifyEmailDto dto, CancellationToken ct);
    Task<MessageResponseDto>  ResendVerificationAsync(ResendVerificationDto dto, CancellationToken ct);

    // ── Password ─────────────────────────────────────────────────
    Task<MessageResponseDto>  ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct);
    Task<MessageResponseDto>  ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct);
    Task<MessageResponseDto>  ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct);
}
