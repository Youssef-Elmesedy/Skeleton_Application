using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Application.Services.ImplementationServices;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.BusinessReules;
using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Interfaces.InterfacesRepository;
using Skeleton.Infrastructure.BackgroundJobs;
using Skeleton.Infrastructure.HealthChecks;
using Skeleton.Infrastructure.Implementation.Queries;
using Skeleton.Infrastructure.Implementation.Repositories;
using Skeleton.Infrastructure.Payment;
using Skeleton.Infrastructure.Payment.Gateways;
using Skeleton.Infrastructure.Persistence;
using Skeleton.Infrastructure.Services;
using System.Text;
using System.Threading.RateLimiting;

namespace Skeleton.Infrastructure;

public static class ServiceRegistration
{
    // ══════════════════════════════════════════════════════════════
    //  Database
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddDataBaseService(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
               .EnableSensitiveDataLogging());
        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  JWT Authentication
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection("JwtSettings");
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(jwt["SecretKey"]!)),
                ClockSkew = TimeSpan.Zero
            };

            // Allow JWT via query string for SignalR connections
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    var token = ctx.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(token) &&
                        ctx.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        ctx.Token = token;
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();
        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  Rate Limiting
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddRateLimitingPolicies(
        this IServiceCollection services)
    {
        services.AddRateLimiter(opt =>
        {
            opt.RejectionStatusCode = 429;

            opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                ctx => RateLimitPartition.GetFixedWindowLimiter(
                    ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 5
                    }));

            opt.AddFixedWindowLimiter("AuthPolicy", o =>
            {
                o.PermitLimit = 10; o.Window = TimeSpan.FromMinutes(1);
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; o.QueueLimit = 0;
            });

            opt.AddFixedWindowLimiter("WritePolicy", o =>
            {
                o.PermitLimit = 30; o.Window = TimeSpan.FromMinutes(1);
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; o.QueueLimit = 2;
            });
        });
        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  Caching — Redis + Memory
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddCachingServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        var redis = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(redis))
            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = redis;
                o.InstanceName = configuration["Redis:InstanceName"] ?? "SkeletonEcom:";
            });
        else
            services.AddDistributedMemoryCache();

        services.AddScoped<ICacheService, CacheService>();
        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  Hangfire
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddHangfireServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection")!;
        var workers = int.Parse(configuration["Hangfire:WorkerCount"] ?? "5");

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(conn, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        services.AddHangfireServer(o => o.WorkerCount = workers);
        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  SignalR
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddSignalRServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var signalRBuilder = services.AddSignalR(o =>
        {
            o.EnableDetailedErrors = true;
            o.MaximumReceiveMessageSize = 32 * 1024;  // 32 KB
        });

        // Scale-out with Redis backplane (optional)
        var redis = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(redis))
            signalRBuilder.AddStackExchangeRedis(redis, o =>
                o.Configuration.ChannelPrefix = "SkeletonEcom");

        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  Health Checks
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var checks = services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "db" })
            .AddCheck<RedisHealthCheck>("redis", tags: new[] { "cache" });

        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  Localization
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddLocalizationServices(
        this IServiceCollection services)
    {
        services.AddLocalization(o => o.ResourcesPath = "Resources");
        services.AddScoped<ILocalizationService, LocalizationService>();
        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  HTTP Clients (for gateways)
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddHttpClients(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("Fawry", c =>
        {
            c.BaseAddress = new Uri(
                configuration["Fawry:BaseUrl"] ?? "https://atfawry.fawrystaging.com");
            c.DefaultRequestHeaders.Add("Accept", "application/json");
            c.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("Stripe", c =>
        {
            c.BaseAddress = new Uri("https://api.stripe.com");
            c.DefaultRequestHeaders.Add("Accept", "application/json");
            c.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }

    // ══════════════════════════════════════════════════════════════
    //  DI — All Application Services
    // ══════════════════════════════════════════════════════════════
    public static IServiceCollection AddInjecationServices(
        this IServiceCollection services)
    {
        // Generic Repositories
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>))
                .AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        // ── Auth ────────────────────────────────────────────────
        services.AddScoped<IJwtService, JwtService>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IEmailService, EmailService>();

        // ── Cache ────────────────────────────────────────────────
        services.AddScoped<ICacheService, CacheService>();

        // ── Notifications ─────────────────────────────────────────
        services.AddScoped<INotificationService, NotificationService>();

        // ── Localization ─────────────────────────────────────────
        services.AddScoped<ILocalizationService, LocalizationService>();

        // ── Payment Gateways ─────────────────────────────────────
        services.AddScoped<IPaymentGatewayService, CashGatewayService>()
                .AddScoped<IPaymentGatewayService, StripeGatewayService>()
                .AddScoped<IPaymentGatewayService, FawryGatewayService>()
                .AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();

        // ── Background Jobs ──────────────────────────────────────
        services.AddScoped<PaymentJobService>()
                .AddScoped<InventoryJobService>()
                .AddScoped<CleanupJobService>()
                .AddScoped<ReportJobService>();

        // ── Employee ─────────────────────────────────────────────
        services.AddScoped<IEmployeeQueryRepository, EmployeeQueryRepository>()
                .AddScoped<IEmployeeService, EmployeeService>()
                .AddScoped<EmployeeBusinessRules>();

        // ── Product ──────────────────────────────────────────────
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<ProductBusinessRules>();

        // ── Category ─────────────────────────────────────────────
        services.AddScoped<ICategoryQueryRepository, CategoryQueryRepository>()
                .AddScoped<ICategoryService, CategoryService>()
                .AddScoped<CategoryBusinessRules>();

        // ── Customer ─────────────────────────────────────────────
        services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>()
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<CustomerBusinessRules>();

        // ── CustomerAccount ──────────────────────────────────────
        services.AddScoped<ICustomerAccountQueryRepository, CustomerAccountQueryRepository>()
                .AddScoped<ICustomerAccountService, CustomerAccountService>()
                .AddScoped<CustomerAccountBusinessRules>();

        // ── Cart ─────────────────────────────────────────────────
        services.AddScoped<ICartQueryRepository, CartQueryRepository>()
                .AddScoped<ICartService, CartService>()
                .AddScoped<CartBusinessRules>();

        // ── Order ────────────────────────────────────────────────
        services.AddScoped<IOrderQueryRepository, OrderQueryRepository>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<OrderBusinessRules>();

        // ── Discount ─────────────────────────────────────────────
        services.AddScoped<IDiscountQueryRepository, DiscountQueryRepository>()
                .AddScoped<IDiscountService, DiscountService>()
                .AddScoped<DiscountBusinessRules>();

        // ── Payment ──────────────────────────────────────────────
        services.AddScoped<IPaymentQueryRepository, PaymentQueryRepository>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped<PaymentBusinessRules>();

        // ── ProductReview ─────────────────────────────────────────
        services.AddScoped<IProductReviewQueryRepository, ProductReviewQueryRepository>()
                .AddScoped<IProductReviewService, ProductReviewService>()
                .AddScoped<ProductReviewBusinessRules>();

        return services;
    }
}
