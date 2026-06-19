namespace Skeleton.Application.Services.Interfaces;

public interface IEmailService
{
    // ── Email Verification ───────────────────────────────────────
    Task SendVerificationEmailAsync(string toEmail, string username, string token, CancellationToken cancellationToken = default);

    // ── Password Reset ───────────────────────────────────────────
    Task SendPasswordResetEmailAsync(string toEmail, string username, string token, CancellationToken cancellationToken = default);

    // ── Generic ──────────────────────────────────────────────────
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
