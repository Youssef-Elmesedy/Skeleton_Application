using MediatR;
using Skeleton.Application.Feature.Customer.CustomerDto;


namespace Skeleton.Application.Feature.Customer.Queries.Search;

public record SearchCustomersQuery(string Keyword) : IRequest<Result<IReadOnlyList<CustomerResponseDto>>>;

public sealed class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, Result<IReadOnlyList<CustomerResponseDto>>>
{
    private readonly ICustomerService _customerService;
    public SearchCustomersQueryHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<IReadOnlyList<CustomerResponseDto>>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerService.SearchCustomersAsync(request.Keyword, cancellationToken);
        return Result<IReadOnlyList<CustomerResponseDto>>.Success("Search completed successfully.", customers);
    }
}

public class SearchCustomersValidator : AbstractValidator<SearchCustomersQuery>
{
    public SearchCustomersValidator()
    {
        RuleFor(x => x.Keyword).NotEmpty().MinimumLength(2).WithMessage("Search keyword must be at least 2 characters.");
    }
}