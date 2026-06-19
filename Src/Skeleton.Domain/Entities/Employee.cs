namespace Skeleton.Domain.Entities;

/// <summary>
/// موظف في الشركة — مرتبط بـ AppUser عشان تسجيل الدخول
/// </summary>
public class Employee : BaseEntity
{
    public string  FullName    { get; private set; } = null!;
    public string  PhoneNumber { get; private set; } = null!;
    public string  Email       { get; private set; } = null!;
    public string  Position    { get; private set; } = null!;   // مسمى وظيفي
    public string? Department  { get; private set; }
    public decimal Salary      { get; private set; }
    public DateTime HireDate   { get; private set; }
    public bool    IsActive    { get; private set; } = true;
    public string? Notes       { get; private set; }

    // Navigation — one AppUser per Employee
    public AppUser? User { get; private set; }

    public Employee(string fullName, string phoneNumber, string email,
                    string position, decimal salary,
                    string? department = null, string? notes = null)
    {
        FullName    = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email       = email.Trim().ToLowerInvariant();
        Position    = position.Trim();
        Salary      = salary;
        Department  = department?.Trim();
        Notes       = notes?.Trim();
        HireDate    = DateTime.UtcNow;
    }

    public void Update(string fullName, string phoneNumber, string email,
                       string position, decimal salary,
                       string? department, string? notes)
    {
        FullName    = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email       = email.Trim().ToLowerInvariant();
        Position    = position.Trim();
        Salary      = salary;
        Department  = department?.Trim();
        Notes       = notes?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate()   => IsActive = true;
}
