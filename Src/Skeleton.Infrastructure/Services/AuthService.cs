using System.Security.Cryptography;
using System.Text;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Skeleton.Application.Feature.Auth.AuthDto;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IReadRepository<AppUser>  _read;
    private readonly IWriteRepository<AppUser> _write;
    private readonly IJwtService               _jwt;
    private readonly IEmailService             _email;
    private readonly IConfiguration            _config;

    public AuthService(
        IReadRepository<AppUser>  read,
        IWriteRepository<AppUser> write,
        IJwtService               jwt,
        IEmailService             email,
        IConfiguration            config)
    {
        _read   = read;
        _write  = write;
        _jwt    = jwt;
        _email  = email;
        _config = config;
    }

    // ─────────────────────────────────────────────────────────────
    //  Login
    // ─────────────────────────────────────────────────────────────
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var user = await _read.FirstOrDefaultAsync(u => u.Username == dto.Username.Trim(), ct)
                   ?? throw new BusinessException(ErrorType.Validation, "Auth.InvalidCredentials");

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new BusinessException(ErrorType.Validation, "Auth.InvalidCredentials");

        if (!user.IsEmailVerified)
            throw new BusinessException(ErrorType.Validation, "Auth.EmailNotVerified");

        return BuildResponse(user);
    }

    // ─────────────────────────────────────────────────────────────
    //  Register
    // ─────────────────────────────────────────────────────────────
    public async Task<LoginResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct)
    {
        if (await _read.AnyAsync(u => u.Username == dto.Username.Trim(), ct))
            throw new BusinessException(ErrorType.Conflict, "Auth.UsernameExists");

        if (await _read.AnyAsync(u => u.Email == dto.Email.Trim().ToLowerInvariant(), ct))
            throw new BusinessException(ErrorType.Conflict, "Auth.EmailExists");

        if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var role))
            throw new BusinessException(ErrorType.Validation, "Auth.InvalidRole");

        var user = new AppUser(dto.Username.Trim(), dto.Email.Trim(),
                               HashPassword(dto.Password), role,
                               dto.CustomerId, dto.EmployeeId);

        await _write.AddAsync(user);
        await _write.SaveChangesAsync(ct);

        // ── Send verification email via Hangfire ─────────────────
        BackgroundJob.Enqueue<IEmailService>(svc =>
            svc.SendVerificationEmailAsync(user.Email, user.Username, user.EmailVerificationToken!, CancellationToken.None));

        return BuildResponse(user);
    }

    // ─────────────────────────────────────────────────────────────
    //  Verify Email
    // ─────────────────────────────────────────────────────────────
    public async Task<MessageResponseDto> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken ct)
    {
        var user = await _read.FirstOrDefaultAsync(
            u => u.Email == dto.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new BusinessException(ErrorType.NotFound, "Auth.UserNotFound");

        if (user.IsEmailVerified)
            throw new BusinessException(ErrorType.Validation, "Auth.EmailAlreadyVerified");

        if (!user.VerifyEmail(dto.Token))
            throw new BusinessException(ErrorType.Validation, "Auth.InvalidVerificationToken");

        await _write.UpdateAsync(user);
        await _write.SaveChangesAsync(ct);

        return new MessageResponseDto("Auth.EmailVerified");
    }

    // ─────────────────────────────────────────────────────────────
    //  Resend Verification
    // ─────────────────────────────────────────────────────────────
    public async Task<MessageResponseDto> ResendVerificationAsync(ResendVerificationDto dto, CancellationToken ct)
    {
        var user = await _read.FirstOrDefaultAsync(
            u => u.Email == dto.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new BusinessException(ErrorType.NotFound, "Auth.UserNotFound");

        if (user.IsEmailVerified)
            throw new BusinessException(ErrorType.Validation, "Auth.EmailAlreadyVerified");

        user.GenerateEmailVerificationToken();
        await _write.UpdateAsync(user);
        await _write.SaveChangesAsync(ct);

        BackgroundJob.Enqueue<IEmailService>(svc =>
            svc.SendVerificationEmailAsync(user.Email, user.Username, user.EmailVerificationToken!, CancellationToken.None));

        return new MessageResponseDto("Auth.VerificationSent");
    }

    // ─────────────────────────────────────────────────────────────
    //  Forgot Password
    // ─────────────────────────────────────────────────────────────
    public async Task<MessageResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct)
    {
        // Always return success to prevent email enumeration
        var user = await _read.FirstOrDefaultAsync(
            u => u.Email == dto.Email.Trim().ToLowerInvariant(), ct);

        if (user is not null)
        {
            user.GeneratePasswordResetToken();
            await _write.UpdateAsync(user);
            await _write.SaveChangesAsync(ct);

            BackgroundJob.Enqueue<IEmailService>(svc =>
                svc.SendPasswordResetEmailAsync(user.Email, user.Username, user.PasswordResetToken!, CancellationToken.None));
        }

        return new MessageResponseDto("Auth.PasswordResetSent");
    }

    // ─────────────────────────────────────────────────────────────
    //  Reset Password
    // ─────────────────────────────────────────────────────────────
    public async Task<MessageResponseDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct)
    {
        if (dto.NewPassword != dto.ConfirmNewPassword)
            throw new BusinessException(ErrorType.Validation, "Auth.PasswordMismatch");

        var user = await _read.FirstOrDefaultAsync(
            u => u.Email == dto.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new BusinessException(ErrorType.NotFound, "Auth.UserNotFound");

        if (!user.IsPasswordResetTokenValid(dto.Token))
            throw new BusinessException(ErrorType.Validation, "Auth.InvalidResetToken");

        user.UpdatePassword(HashPassword(dto.NewPassword));
        await _write.UpdateAsync(user);
        await _write.SaveChangesAsync(ct);

        return new MessageResponseDto("Auth.PasswordReset");
    }

    // ─────────────────────────────────────────────────────────────
    //  Change Password (authenticated)
    // ─────────────────────────────────────────────────────────────
    public async Task<MessageResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct)
    {
        if (dto.NewPassword != dto.ConfirmNewPassword)
            throw new BusinessException(ErrorType.Validation, "Auth.PasswordMismatch");

        var user = await _read.GetByIdAsync(userId, ct)
                   ?? throw new BusinessException(ErrorType.NotFound, "Auth.UserNotFound");

        if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            throw new BusinessException(ErrorType.Validation, "Auth.WrongCurrentPassword");

        user.UpdatePassword(HashPassword(dto.NewPassword));
        await _write.UpdateAsync(user);
        await _write.SaveChangesAsync(ct);

        return new MessageResponseDto("Auth.PasswordChanged");
    }

    // ─────────────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────────────
    private static string HashPassword(string password)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

    private static bool VerifyPassword(string password, string hash)
        => HashPassword(password) == hash;

    private LoginResponseDto BuildResponse(AppUser user)
    {
        var token     = _jwt.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(
            int.Parse(_config["JwtSettings:ExpiresInMinutes"]!));

        return new LoginResponseDto(token, user.Username, user.Email,
                                    user.Role.ToString(), user.IsEmailVerified, expiresAt);
    }
}
