using Skeleton.Application.Common;
using Skeleton.Application.Feature.Employee.EmployeeDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface IEmployeeQueryRepository
{
    Task<EmployeeResponseDto?>               GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync(CancellationToken ct);
    Task<PagedResult<EmployeeResponseDto>>   GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyList<EmployeeResponseDto>> SearchAsync(string keyword, CancellationToken ct);
    Task<IReadOnlyList<EmployeeResponseDto>> GetByStatusAsync(bool isActive, CancellationToken ct);
}
