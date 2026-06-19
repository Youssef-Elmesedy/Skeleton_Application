using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Common;
using Skeleton.Application.Feature.Employee.EmployeeDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Common.Extensions;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Queries;

internal class EmployeeQueryRepository : IEmployeeQueryRepository
{
    private readonly AppDbContext _ctx;
    public EmployeeQueryRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<EmployeeResponseDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _ctx.Employees.AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeResponseDto(
                e.Id, e.FullName, e.PhoneNumber, e.Email,
                e.Position, e.Department, e.Salary,
                e.HireDate, e.IsActive, e.Notes,
                e.CreateDate, e.CreateBy))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync(CancellationToken ct)
        => await _ctx.Employees.AsNoTracking()
            .OrderBy(e => e.FullName)
            .Select(e => new EmployeeResponseDto(
                e.Id, e.FullName, e.PhoneNumber, e.Email,
                e.Position, e.Department, e.Salary,
                e.HireDate, e.IsActive, e.Notes,
                e.CreateDate, e.CreateBy))
            .ToListAsync(ct);

    public async Task<PagedResult<EmployeeResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        => await _ctx.Employees.AsNoTracking()
            .OrderBy(e => e.FullName)
            .ToPagedResultAsync(page, pageSize,
                e => new EmployeeResponseDto(
                    e.Id, e.FullName, e.PhoneNumber, e.Email,
                    e.Position, e.Department, e.Salary,
                    e.HireDate, e.IsActive, e.Notes,
                    e.CreateDate, e.CreateBy));

    public async Task<IReadOnlyList<EmployeeResponseDto>> SearchAsync(string keyword, CancellationToken ct)
    {
        var kw = keyword.Trim();
        return await _ctx.Employees.AsNoTracking()
            .Where(e => EF.Functions.Like(e.FullName,    $"%{kw}%")
                     || EF.Functions.Like(e.PhoneNumber, $"%{kw}%")
                     || EF.Functions.Like(e.Email,       $"%{kw}%")
                     || EF.Functions.Like(e.Position,    $"%{kw}%"))
            .Select(e => new EmployeeResponseDto(
                e.Id, e.FullName, e.PhoneNumber, e.Email,
                e.Position, e.Department, e.Salary,
                e.HireDate, e.IsActive, e.Notes,
                e.CreateDate, e.CreateBy))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<EmployeeResponseDto>> GetByStatusAsync(bool isActive, CancellationToken ct)
        => await _ctx.Employees.AsNoTracking()
            .Where(e => e.IsActive == isActive)
            .Select(e => new EmployeeResponseDto(
                e.Id, e.FullName, e.PhoneNumber, e.Email,
                e.Position, e.Department, e.Salary,
                e.HireDate, e.IsActive, e.Notes,
                e.CreateDate, e.CreateBy))
            .ToListAsync(ct);
}
