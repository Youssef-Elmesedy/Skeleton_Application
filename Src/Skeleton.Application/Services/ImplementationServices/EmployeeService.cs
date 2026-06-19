using Skeleton.Application.Feature.Employee.EmployeeDto;
using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.ImplementationServices;

public class EmployeeService : IEmployeeService
{
    private readonly IReadRepository<Employee> _read;
    private readonly IWriteRepository<Employee> _write;
    private readonly IEmployeeQueryRepository _query;
    private readonly EmployeeBusinessRules _rules;

    public EmployeeService(
        IReadRepository<Employee> read,
        IWriteRepository<Employee> write,
        IEmployeeQueryRepository query,
        EmployeeBusinessRules rules)
    {
        _read = read;
        _write = write;
        _query = query;
        _rules = rules;
    }

    public async Task<EmployeeResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var emp = await _query.GetByIdAsync(id, ct)
                  ?? throw new BusinessException(ErrorType.NotFound, "Employee.NotFound");
        return emp;
    }

    public async Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync(CancellationToken ct)
        => await _query.GetAllAsync(ct);

    public async Task<PagedResult<EmployeeResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        => await _query.GetPagedAsync(page, pageSize, ct);

    public async Task<IReadOnlyList<EmployeeResponseDto>> SearchAsync(string keyword, CancellationToken ct)
        => await _query.SearchAsync(keyword, ct);

    public async Task<IReadOnlyList<EmployeeResponseDto>> GetByStatusAsync(bool isActive, CancellationToken ct)
        => await _query.GetByStatusAsync(isActive, ct);

    public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto, CancellationToken ct)
    {
        await _rules.EnsureEmailIsUniqueAsync(dto.Email, ct);

        var emp = new Employee(dto.FullName, dto.PhoneNumber, dto.Email,
                               dto.Position, dto.Salary, dto.Department, dto.Notes);

        await _write.AddAsync(emp);
        await _write.SaveChangesAsync(ct);

        return (await _query.GetByIdAsync(emp.Id, ct))!;
    }

    public async Task<EmployeeResponseDto> UpdateAsync(EmployeeUpdateDto dto, CancellationToken ct)
    {
        await _rules.EnsureExistsAsync(dto.Id, ct);
        await _rules.EnsureEmailIsUniqueOnUpdateAsync(dto.Email, dto.Id, ct);

        var emp = await _read.GetByIdAsync(dto.Id, ct)!;
        emp!.Update(dto.FullName, dto.PhoneNumber, dto.Email,
                    dto.Position, dto.Salary, dto.Department, dto.Notes);

        await _write.UpdateAsync(emp);
        await _write.SaveChangesAsync(ct);

        return (await _query.GetByIdAsync(dto.Id, ct))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var emp = await _read.GetByIdAsync(id, ct)
                  ?? throw new BusinessException(ErrorType.NotFound, "Employee.NotFound");

        await _write.DeleteAsync(emp);
        await _write.SaveChangesAsync(ct);
    }

    public async Task ActivateAsync(Guid id, CancellationToken ct)
    {
        var emp = await _read.GetByIdAsync(id, ct)
                  ?? throw new BusinessException(ErrorType.NotFound, "Employee.NotFound");
        emp.Activate();
        await _write.UpdateAsync(emp);
        await _write.SaveChangesAsync(ct);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct)
    {
        var emp = await _read.GetByIdAsync(id, ct)
                  ?? throw new BusinessException(ErrorType.NotFound, "Employee.NotFound");
        emp.Deactivate();
        await _write.UpdateAsync(emp);
        await _write.SaveChangesAsync(ct);
    }
}
