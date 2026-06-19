using Microsoft.EntityFrameworkCore;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── Auth ─────────────────────────────────────────────────────
    public DbSet<AppUser> AppUsers { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;

    // ── Notifications ─────────────────────────────────────────────
    public DbSet<Notification> Notifications { get; set; } = null!;

    // ── Core ─────────────────────────────────────────────────────
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
    public DbSet<AccountTransaction> AccountTransactions { get; set; } = null!;

    // ── E-Commerce ───────────────────────────────────────────────
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Discount> Discounts { get; set; } = null!;
    public DbSet<Domain.Entities.Payment> Payments { get; set; } = null!;
    public DbSet<ProductReview> ProductReviews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
