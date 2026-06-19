using MediatR;

namespace Skeleton.Application.Feature.Product.Commands.DeleteProduct;

public record DeleteProductByIdCommand(Guid Id) : IRequest<Result<string>>;
