using Skeleton.Application.Feature.Employee.EmployeeDto;
using Skeleton.Application.Common;

namespace Skeleton.Application.Services.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeResponseDto>              GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync(CancellationToken ct);
    Task<PagedResult<EmployeeResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyList<EmployeeResponseDto>> SearchAsync(string keyword, CancellationToken ct);
    Task<IReadOnlyList<EmployeeResponseDto>> GetByStatusAsync(bool isActive, CancellationToken ct);
    Task<EmployeeResponseDto>              CreateAsync(EmployeeCreateDto dto, CancellationToken ct);
    Task<EmployeeResponseDto>              UpdateAsync(EmployeeUpdateDto dto, CancellationToken ct);
    Task                                   DeleteAsync(Guid id, CancellationToken ct);
    Task                                   ActivateAsync(Guid id, CancellationToken ct);
    Task                                   DeactivateAsync(Guid id, CancellationToken ct);
}
