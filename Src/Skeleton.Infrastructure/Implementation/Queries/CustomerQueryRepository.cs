using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Common;
using Skeleton.Application.Feature.Customer.CustomerDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Common.Extensions;
using Skeleton.Infrastructure.Common.ProjectionsExtensions;
using Skeleton.Infrastructure.Persistence;

internal class CustomerQueryRepository : ICustomerQueryRepository
{
    private readonly AppDbContext _context;

    public CustomerQueryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CustomerResponseDto>> GetAllCustomersAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AsNoTracking()
            .Select(AsProjections.AsCustomerResponseDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerResponseDto?> GetCustomerByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(AsProjections.AsCustomerResponseDto)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerResponseDto>> SearchAsync(
    string? keyword,
    CancellationToken cancellationToken)
    {
        var query = _context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(c =>
                EF.Functions.Like(c.FullName, $"%{keyword}%") ||
                EF.Functions.Like(c.PhoneNumber, $"%{keyword}%"));
        }

        return await query
            .Select(AsProjections.AsCustomerResponseDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<CustomerResponseDto>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.FullName);

        return await query.ToPagedResultAsync(
            page,
            pageSize,
            AsProjections.AsCustomerResponseDto);
    }

    public async Task<IReadOnlyList<CustomerResponseDto>> GetCustomersByStatusAsync(
    bool isActive,
    CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.IsActive == isActive)
            .Select(AsProjections.AsCustomerResponseDto)
            .ToListAsync(cancellationToken);
    }
}