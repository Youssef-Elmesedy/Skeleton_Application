using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;

    // الحساب المالي للعميل
    public CustomerAccount Account { get; private set; } = null!;

    public Customer(string fullName, string phoneNumber, string? email, string? address, string? notes)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new BusinessException(ErrorType.Validation, "Customer name is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BusinessException(ErrorType.Validation, "Phone number is required.");

        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Notes = notes;

        //Create Customer Account
        Account = new CustomerAccount(Id);
    }

    // Partial Update Customer
    public void Update(
    string? fullName = null,
    string? phoneNumber = null,
    string? email = null,
    string? address = null,
    string? notes = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = fullName;

        if (!string.IsNullOrWhiteSpace(phoneNumber))
            PhoneNumber = phoneNumber;

        if (email != null)
            Email = email;

        if (address != null)
            Address = address;

        if (notes != null)
            Notes = notes;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}