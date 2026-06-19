using Skeleton.Application.Feature.Customer.CustomerDto;
using Skeleton.Domain.BusinessRules;

public class CustomerService : ICustomerService
{
    private readonly IReadRepository<Customer> _readRepository;
    private readonly IWriteRepository<Customer> _writeRepository;
    private readonly ICustomerQueryRepository _queryRepository;
    private readonly CustomerBusinessRules _customerBusinessRules;
    private readonly IMapper _mapper;

    public CustomerService(
        IReadRepository<Customer> readRepository,
        IWriteRepository<Customer> writeRepository,
        ICustomerQueryRepository queryRepository,
        CustomerBusinessRules customerBusinessRules,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _queryRepository = queryRepository;
        _customerBusinessRules = customerBusinessRules;
        _mapper = mapper;
    }
    // Query Methods

    public Task<IReadOnlyList<CustomerResponseDto>> GetAllCustomersAsync(
       CancellationToken cancellationToken)
       => _queryRepository.GetAllCustomersAsync(cancellationToken);

    public async Task<CustomerResponseDto> GetCustomerByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _queryRepository.GetCustomerByIdAsync(id, cancellationToken);

        DtoBusinessRules.EnsureExists(customer, id, nameof(Customer));

        return customer!;
    }

    public Task<PagedResult<CustomerResponseDto>> GetPagedCustomersAsync(
        int page,
        int size,
        CancellationToken cancellationToken)
        => _queryRepository.GetPagedAsync(page, size, cancellationToken);

    public Task<IReadOnlyList<CustomerResponseDto>> SearchCustomersAsync(
        string keyword,
        CancellationToken cancellationToken)
        => _queryRepository.SearchAsync(keyword, cancellationToken);

    public Task<IReadOnlyList<CustomerResponseDto>> GetCustomersByStatusAsync(bool isActive, CancellationToken cancellationToken)
    => _queryRepository.GetCustomersByStatusAsync(isActive, cancellationToken);

    // Command Methods

    public async Task<CustomerResponseDto> AddCustomerAsync(
        CustomerCreateDto dto,
        CancellationToken cancellationToken)
    {
        await _customerBusinessRules.EnsureUniqueAsync(p => p.FullName == dto.FullName,
           "Customer name already exists.", cancellationToken);

        await _customerBusinessRules.EnsurePhoneUnique(dto.PhoneNumber, cancellationToken);

        if (!string.IsNullOrEmpty(dto.Email))
            await _customerBusinessRules.EnsureEmailUnique(dto.Email, cancellationToken);

        var customer = new Customer(
            dto.FullName,
            dto.PhoneNumber,
            dto.Email,
            dto.Address,
            dto.Notes);

        await _writeRepository.AddAsync(customer);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<CustomerResponseDto> UpdateCustomerAsync(
        CustomerUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var customer = await _readRepository.GetByIdAsync(dto.Id, cancellationToken);

        var existingCustomer = _customerBusinessRules.EnsureExists(customer, dto.Id, nameof(Customer));

        if (!string.IsNullOrWhiteSpace(dto.FullName))
            await _customerBusinessRules.EnsureUniqueAsync(p => p.FullName == dto.FullName
              && p.Id != dto.Id, "Customer name already exists.", cancellationToken);

        if (string.IsNullOrEmpty(dto.PhoneNumber) is false && dto.PhoneNumber != existingCustomer.PhoneNumber)
            await _customerBusinessRules.EnsurePhoneUnique(dto.PhoneNumber, cancellationToken);

        if (!string.IsNullOrEmpty(dto.Email))
            await _customerBusinessRules.EnsureEmailUnique(dto.Email, cancellationToken);

        existingCustomer.Update(
            dto.FullName,
            dto.PhoneNumber,
            dto.Email,
            dto.Address,
            dto.Notes);

        await _writeRepository.UpdateAsync(existingCustomer);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerResponseDto>(existingCustomer);
    }

    public async Task DeleteCustomerAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _readRepository.GetByIdAsync(id, cancellationToken);

        var existingCustomer = _customerBusinessRules.EnsureExists(
            customer,
            id,
            nameof(Customer));

        await _writeRepository.DeleteAsync(existingCustomer);
        await _writeRepository.SaveChangesAsync(cancellationToken);
    }
}