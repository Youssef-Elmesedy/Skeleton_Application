using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessRules;

public class EmployeeBusinessRules : EntityBaseBusinessRules<Employee>
{
    private readonly IReadRepository<Employee> _repo;

    public EmployeeBusinessRules(IReadRepository<Employee> repo) : base(repo)
        => _repo = repo;

    public async Task EnsureExistsAsync(Guid id, CancellationToken ct)
    {
        if (!await _repo.AnyAsync(e => e.Id == id, ct))
            throw new BusinessException(ErrorType.NotFound, "Employee.NotFound");
    }

    public async Task EnsureEmailIsUniqueAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        if (await _repo.AnyAsync(e => e.Email == normalized, ct))
            throw new BusinessException(ErrorType.Conflict, "Employee.EmailExists");
    }

    public async Task EnsureEmailIsUniqueOnUpdateAsync(string email, Guid id, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        if (await _repo.AnyAsync(e => e.Email == normalized && e.Id != id, ct))
            throw new BusinessException(ErrorType.Conflict, "Employee.EmailExists");
    }
}
