using FluentValidation;
using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature;
using Skeleton.Infrastructure;
using Skeleton.Infrastructure.BackgroundJobs;
using Skeleton.Infrastructure.Hubs;
using Skeleton.Middleware;
using System.Reflection;
using System.Text.Json;

namespace Skeleton;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ── Configuration ──────────────────────────────────────────
        builder.Configuration
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile("appsettings.DataBase.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();

        // ── Controllers ────────────────────────────────────────────
        builder.Services.AddControllers()
               .AddJsonOptions(o =>
               {
                   o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                   o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
               });

        // ── CORS ───────────────────────────────────────────────────
        builder.Services.AddCors(o => o.AddPolicy("AllowAll", p =>
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        // ── Localization ───────────────────────────────────────────
        builder.Services.AddLocalizationServices();
        builder.Services.Configure<RequestLocalizationOptions>(o =>
        {
            var supported = new[] { "en", "ar" };
            o.SetDefaultCulture("en")
             .AddSupportedCultures(supported)
             .AddSupportedUICultures(supported);
            o.ApplyCurrentCultureToResponseHeaders = true;
        });

        // ── Mapster ────────────────────────────────────────────────
        MapsterConfig.RegisterMappings();
        builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        builder.Services.AddScoped<IMapper, ServiceMapper>();

        // ── MediatR ────────────────────────────────────────────────
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(Skeleton.Application.AssemblyReference).Assembly));

        // ── FluentValidation ───────────────────────────────────────
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.Configure<ApiBehaviorOptions>(o =>
            o.SuppressModelStateInvalidFilter = true);
        builder.Services.AddValidatorsFromAssembly(
            typeof(Skeleton.Application.AssemblyReference).Assembly);

        // ── JWT ────────────────────────────────────────────────────
        builder.Services.AddJwtAuthentication(builder.Configuration);

        // ── Rate Limiting ──────────────────────────────────────────
        builder.Services.AddRateLimitingPolicies();

        // ── Caching ────────────────────────────────────────────────
        builder.Services.AddCachingServices(builder.Configuration);

        // ── Hangfire ───────────────────────────────────────────────
        builder.Services.AddHangfireServices(builder.Configuration);

        // ── SignalR ────────────────────────────────────────────────
        builder.Services.AddSignalRServices(builder.Configuration);

        // ── Health Checks ──────────────────────────────────────────
        builder.Services.AddHealthCheckServices(builder.Configuration);

        // ── HTTP Clients (gateways) ────────────────────────────────
        builder.Services.AddHttpClients(builder.Configuration);

        // ── Swagger ────────────────────────────────────────────────
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Skeleton E-Commerce API",
                Version = "v1",
                Description = """
                    ## Skeleton E-Commerce API

                    ### 🌐 Localization
                    `Accept-Language: ar` → Arabic | `Accept-Language: en` → English (default)

                    ### 🔐 Auth Flow
                    1. `POST /api/auth/register` → get token + receive verification email
                    2. `GET /api/auth/verify-email?email=&token=` → verify
                    3. `POST /api/auth/login` → get JWT
                    4. Click **Authorize** → `Bearer {token}`

                    ### 💳 Payment Flow
                    ```
                    POST /api/payments/initiate  → creates payment, calls gateway
                    POST /api/payments/confirm   → captures payment
                    POST /api/payments/webhook   → async gateway callback
                    POST /api/payments/refund    → full or partial refund
                    ```

                    ### 🔔 Real-time Notifications (SignalR)
                    ```js
                    const conn = new signalR.HubConnectionBuilder()
                      .withUrl("/hubs/notifications", { accessTokenFactory: () => token })
                      .withAutomaticReconnect().build();
                    conn.on("ReceiveNotification", n => console.log(n));
                    await conn.start();
                    ```

                    ### ⚡ Rate Limits
                    | Policy | Limit |
                    |--------|-------|
                    | Global | 100 req/min |
                    | Auth | 10 req/min |
                    | Write | 30 req/min |

                    ### 🏥 Health Check
                    `GET /health` → system status
                    """
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
        });

        // ── Database + DI ──────────────────────────────────────────
        builder.Services.AddDataBaseService(builder.Configuration);
        builder.Services.AddInjecationServices();

        // ─────────────────────────────────────────────────────────
        var app = builder.Build();
        // ─────────────────────────────────────────────────────────

        // ── Swagger ────────────────────────────────────────────────
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Skeleton API v1");
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.DocumentTitle = "Skeleton E-Commerce API";
            });
        }

        // ── Health Checks ──────────────────────────────────────────
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (ctx, report) =>
            {
                ctx.Response.ContentType = "application/json";
                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        message = e.Value.Description
                    }),
                    duration = report.TotalDuration
                };
                await ctx.Response.WriteAsJsonAsync(result);
            }
        });

        // ── Hangfire Dashboard ─────────────────────────────────────
        app.UseHangfireDashboard(
            app.Configuration["Hangfire:DashboardPath"] ?? "/hangfire",
            new DashboardOptions
            {
                Authorization = Array.Empty<Hangfire.Dashboard.IDashboardAuthorizationFilter>()
            });

        // ── Register Recurring Jobs ────────────────────────────────
        RecurringJobsSetup.RegisterAll(app.Configuration);

        app.UseCors("AllowAll");
        app.UseHttpsRedirection();

        // ── Middleware pipeline ────────────────────────────────────
        app.UseMiddleware<LocalizationMiddleware>();
        app.UseRequestLocalization();
        app.UseMiddleware<RateLimitingMiddleware>();   // custom sliding-window
        app.UseRateLimiter();                         // named policies

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<GlobalExceptionMiddleware>();

        // ── SignalR Hubs ───────────────────────────────────────────
        app.MapHub<NotificationHub>("/hubs/notifications");

        app.MapControllers();

        app.Run();
    }
}
