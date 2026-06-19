namespace Skeleton.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Handles Result<T> and converts to proper HTTP response with status code
        /// </summary>
        protected IActionResult HandleResult<T>(Result<T> result, bool isCreated = false)
        {
            if (result.IsSuccess)
            {
                if (isCreated)
                    return StatusCode(201, ApiResponse<T>
                        .Success(result.Message.OrDefault("Not Found Message"), result.Value!));
                // 201 Created

                return Ok(ApiResponse<T>
                    .Success(result.Message.OrDefault("Not Found Message"), result.Value!));
                // 200 OK
            }

            return result.Error!.Code switch
            {
                "Not Found" => NotFound(ApiResponse<T>.Failure(result.Error.Message, result.Error.Code)),
                "Validation Error" => BadRequest(ApiResponse<T>.Failure(result.Error.Message, result.Error.Code)),
                "Conflict Error" => Conflict(ApiResponse<T>.Failure(result.Error.Message, result.Error.Code)),
                _ => StatusCode(500, ApiResponse<T>.Failure(result.Error.Message, result.Error.Code))
            };
        }
    }
}