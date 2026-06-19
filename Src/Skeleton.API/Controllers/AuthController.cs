using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Auth.AuthDto;
using Skeleton.Application.Services.Interfaces;
using System.Security.Claims;

namespace Skeleton.Controllers;

/// <summary>
/// Authentication — Login, Register, Email Verification, Password Management
/// </summary>
/// <remarks>
/// جميع عمليات التحقق من الهوية.
/// استخدم `Accept-Language: ar` للردود بالعربية.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : BaseController
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    // ── Login ────────────────────────────────────────────────────

    /// <summary>Login to receive a JWT token.</summary>
    /// <remarks>تسجيل الدخول للحصول على JWT Token.</remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(dto, ct);
        return Ok(ApiResponse<LoginResponseDto>.Success("Login successful.", result));
    }

    // ── Register ─────────────────────────────────────────────────

    /// <summary>Register a new user account.</summary>
    /// <remarks>
    /// إنشاء حساب جديد. سيتم إرسال بريد تحقق تلقائياً.
    /// Role: Admin | Employee | Customer
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await _auth.RegisterAsync(dto, ct);
        return StatusCode(201, ApiResponse<LoginResponseDto>.Success("Registered. Please verify your email.", result));
    }

    // ── Email Verification ────────────────────────────────────────

    /// <summary>Verify email using the token sent to your inbox.</summary>
    /// <remarks>التحقق من البريد الإلكتروني باستخدام الرمز المُرسَل.</remarks>
    [HttpGet("verify-email")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> VerifyEmail(
        [FromQuery] string email, [FromQuery] string token, CancellationToken ct)
    {
        var result = await _auth.VerifyEmailAsync(new VerifyEmailDto(email, token), ct);
        return Ok(ApiResponse<MessageResponseDto>.Success(result.Message, result));
    }

    /// <summary>Resend the email verification link.</summary>
    /// <remarks>إعادة إرسال رابط التحقق من البريد الإلكتروني.</remarks>
    [HttpPost("resend-verification")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), 200)]
    public async Task<IActionResult> ResendVerification(
        [FromBody] ResendVerificationDto dto, CancellationToken ct)
    {
        var result = await _auth.ResendVerificationAsync(dto, ct);
        return Ok(ApiResponse<MessageResponseDto>.Success(result.Message, result));
    }

    // ── Password ──────────────────────────────────────────────────

    /// <summary>Request a password reset link via email.</summary>
    /// <remarks>
    /// طلب رابط إعادة تعيين كلمة المرور.
    /// يُرسَل دائماً رد نجاح لمنع تعداد البريد الإلكتروني.
    /// </remarks>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), 200)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto dto, CancellationToken ct)
    {
        var result = await _auth.ForgotPasswordAsync(dto, ct);
        return Ok(ApiResponse<MessageResponseDto>.Success(result.Message, result));
    }

    /// <summary>Reset password using the token from email.</summary>
    /// <remarks>إعادة تعيين كلمة المرور باستخدام الرمز المُرسَل للبريد.</remarks>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto dto, CancellationToken ct)
    {
        var result = await _auth.ResetPasswordAsync(dto, ct);
        return Ok(ApiResponse<MessageResponseDto>.Success(result.Message, result));
    }

    /// <summary>Change password for the currently authenticated user.</summary>
    /// <remarks>تغيير كلمة المرور للمستخدم الحالي. يتطلب تسجيل دخول.</remarks>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _auth.ChangePasswordAsync(userId, dto, ct);
        return Ok(ApiResponse<MessageResponseDto>.Success(result.Message, result));
    }
}
