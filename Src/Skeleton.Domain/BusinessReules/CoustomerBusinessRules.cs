using Skeleton.Domain.Entities;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessRules;

public class CustomerBusinessRules : EntityBaseBusinessRules<Customer>
{
    public CustomerBusinessRules(
        IReadRepository<Customer> customerRepository)
        : base(customerRepository)
    {
    }

    public async Task EnsureEmailUnique(
        string email,
        CancellationToken cancellationToken)
    {
        await EnsureUniqueAsync(
            c => c.Email == email,
            "Customer email already exists",
            cancellationToken);
    }

    public async Task EnsurePhoneUnique(
        string phone,
        CancellationToken cancellationToken)
    {
        await EnsureUniqueAsync(
            c => c.PhoneNumber == phone,
            "Customer phone already exists",
            cancellationToken);
    }
}
