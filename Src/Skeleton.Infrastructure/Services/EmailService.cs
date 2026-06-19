using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Skeleton.Application.Services.Interfaces;

namespace Skeleton.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────
    //  Email Verification
    // ─────────────────────────────────────────────────────────────
    public async Task SendVerificationEmailAsync(
        string toEmail, string username, string token,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = _config["AppSettings:BaseUrl"] ?? "https://localhost:7001";
        var link    = $"{baseUrl}/api/auth/verify-email?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(token)}";

        var html = $"""
            <div dir="ltr" style="font-family:Arial,sans-serif;max-width:600px;margin:auto">
              <h2 style="color:#2563EB">Email Verification</h2>
              <p>Hello <strong>{username}</strong>,</p>
              <p>Click the button below to verify your email address. This link expires in <strong>24 hours</strong>.</p>
              <a href="{link}"
                 style="display:inline-block;padding:12px 24px;background:#2563EB;color:#fff;
                        text-decoration:none;border-radius:6px;font-weight:bold">
                Verify Email
              </a>
              <p style="color:#6B7280;font-size:12px;margin-top:24px">
                If you did not register, please ignore this email.
              </p>
            </div>
            """;

        await SendEmailAsync(toEmail, "Verify Your Email — Skeleton E-Commerce", html, cancellationToken);
    }

    // ─────────────────────────────────────────────────────────────
    //  Password Reset
    // ─────────────────────────────────────────────────────────────
    public async Task SendPasswordResetEmailAsync(
        string toEmail, string username, string token,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = _config["AppSettings:BaseUrl"] ?? "https://localhost:7001";
        var link    = $"{baseUrl}/api/auth/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(token)}";

        var html = $"""
            <div dir="ltr" style="font-family:Arial,sans-serif;max-width:600px;margin:auto">
              <h2 style="color:#DC2626">Password Reset</h2>
              <p>Hello <strong>{username}</strong>,</p>
              <p>Click the button below to reset your password. This link expires in <strong>1 hour</strong>.</p>
              <a href="{link}"
                 style="display:inline-block;padding:12px 24px;background:#DC2626;color:#fff;
                        text-decoration:none;border-radius:6px;font-weight:bold">
                Reset Password
              </a>
              <p style="color:#6B7280;font-size:12px;margin-top:24px">
                If you did not request this, please ignore this email and your password will remain unchanged.
              </p>
            </div>
            """;

        await SendEmailAsync(toEmail, "Password Reset Request — Skeleton E-Commerce", html, cancellationToken);
    }

    // ─────────────────────────────────────────────────────────────
    //  Generic Send
    // ─────────────────────────────────────────────────────────────
    public async Task SendEmailAsync(
        string toEmail, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var smtp     = _config.GetSection("EmailSettings");
        var host     = smtp["Host"]     ?? "smtp.gmail.com";
        var port     = int.Parse(smtp["Port"] ?? "587");
        var user     = smtp["Username"] ?? "";
        var pass     = smtp["Password"] ?? "";
        var fromName = smtp["FromName"] ?? "Skeleton E-Commerce";

        using var client  = new SmtpClient(host, port);
        client.EnableSsl  = true;
        client.Credentials = new NetworkCredential(user, pass);

        using var message = new MailMessage();
        message.From    = new MailAddress(user, fromName);
        message.To.Add(toEmail);
        message.Subject    = subject;
        message.Body       = htmlBody;
        message.IsBodyHtml = true;

        try
        {
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent to {Email}: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }
}
