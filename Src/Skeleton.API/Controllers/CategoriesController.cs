using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Skeleton.Controllers;

/// <summary>
/// Categories — Browse and manage product categories.
/// </summary>
/// <remarks>
/// **Read operations**: accessible to everyone (anonymous).
/// **Write operations**: **Admin only**.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : BaseController
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get all categories (paginated).</summary>
    [HttpGet("paged")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetpagedCategoriesQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Get all categories.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Get a single category by ID.</summary>
    [HttpGet("{categoryId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Search categories by keyword.</summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string keyword,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchCategoryQuery(keyword), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Create a new category. **Admin only.**</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCategoryCommand(dto), cancellationToken);
        return HandleResult(result, isCreated: true);
    }

    /// <summary>Update an existing category. **Admin only.**</summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCategoryCommand(dto), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Delete a category by ID. **Admin only.**</summary>
    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
        return HandleResult(result);
    }
}
