using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Employee.EmployeeDto;
using Skeleton.Application.Services.Interfaces;

namespace Skeleton.Controllers;

/// <summary>
/// Employees — Manage company employees.
/// </summary>
/// <remarks>
/// | Role     | Access |
/// |----------|--------|
/// | **Admin**    | Full access — CRUD + activate/deactivate |
/// | **Employee** | View own profile only |
/// | **Customer** | No access |
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : BaseController
{
    private readonly IEmployeeService _service;
    private readonly ICacheService    _cache;

    public EmployeesController(IEmployeeService service, ICacheService cache)
    {
        _service = service;
        _cache   = cache;
    }

    /// <summary>Get all employees. **Admin only.**</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EmployeeResponseDto>>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        const string key = "employees:all";
        var data = await _cache.GetOrSetAsync(key,
            () => _service.GetAllAsync(ct),
            TimeSpan.FromMinutes(5), ct);

        return Ok(ApiResponse<IReadOnlyList<EmployeeResponseDto>>.Success("Employees retrieved.", data));
    }

    /// <summary>Get employees paginated. **Admin only.**</summary>
    [HttpGet("paged")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize   = 10,
        CancellationToken ct = default)
    {
        var data = await _service.GetPagedAsync(pageNumber, pageSize, ct);
        return Ok(ApiResponse<object>.Success("Employees retrieved.", data));
    }

    /// <summary>Get a single employee by ID. **Admin only.**</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var data = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<EmployeeResponseDto>.Success("Employee retrieved.", data));
    }

    /// <summary>Search employees by name, email, or position. **Admin only.**</summary>
    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Search(
        [FromQuery] string keyword, CancellationToken ct)
    {
        var data = await _service.SearchAsync(keyword, ct);
        return Ok(ApiResponse<IReadOnlyList<EmployeeResponseDto>>.Success("Search completed.", data));
    }

    /// <summary>Filter by active/inactive status. **Admin only.**</summary>
    [HttpGet("by-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetByStatus(
        [FromQuery] bool isActive, CancellationToken ct)
    {
        var data = await _service.GetByStatusAsync(isActive, ct);
        return Ok(ApiResponse<IReadOnlyList<EmployeeResponseDto>>.Success("Employees retrieved.", data));
    }

    /// <summary>Create a new employee. **Admin only.**</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeResponseDto>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create(
        [FromBody] EmployeeCreateDto dto, CancellationToken ct)
    {
        var data = await _service.CreateAsync(dto, ct);
        await _cache.RemoveAsync("employees:all", ct);
        return StatusCode(201, ApiResponse<EmployeeResponseDto>.Success("Employee created.", data));
    }

    /// <summary>Update an employee's details. **Admin only.**</summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeResponseDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(
        [FromBody] EmployeeUpdateDto dto, CancellationToken ct)
    {
        var data = await _service.UpdateAsync(dto, ct);
        await _cache.RemoveAsync("employees:all", ct);
        return Ok(ApiResponse<EmployeeResponseDto>.Success("Employee updated.", data));
    }

    /// <summary>Delete an employee permanently. **Admin only.**</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        await _cache.RemoveAsync("employees:all", ct);
        return Ok(ApiResponse<object>.Success("Employee deleted.", null!));
    }

    /// <summary>Activate an employee account. **Admin only.**</summary>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        await _service.ActivateAsync(id, ct);
        await _cache.RemoveAsync("employees:all", ct);
        return Ok(ApiResponse<object>.Success("Employee activated.", null!));
    }

    /// <summary>Deactivate an employee account. **Admin only.**</summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await _service.DeactivateAsync(id, ct);
        await _cache.RemoveAsync("employees:all", ct);
        return Ok(ApiResponse<object>.Success("Employee deactivated.", null!));
    }
}
