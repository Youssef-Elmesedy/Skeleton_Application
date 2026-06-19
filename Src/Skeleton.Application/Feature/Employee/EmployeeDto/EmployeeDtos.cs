namespace Skeleton.Application.Feature.Employee.EmployeeDto;

public record EmployeeResponseDto(
    Guid      Id,
    string    FullName,
    string    PhoneNumber,
    string    Email,
    string    Position,
    string?   Department,
    decimal   Salary,
    DateTime  HireDate,
    bool      IsActive,
    string?   Notes,
    DateTime? CreateDate,
    string?   CreateBy
);

public record EmployeeCreateDto(
    string   FullName,
    string   PhoneNumber,
    string   Email,
    string   Position,
    decimal  Salary,
    string?  Department = null,
    string?  Notes      = null
);

public record EmployeeUpdateDto(
    Guid     Id,
    string   FullName,
    string   PhoneNumber,
    string   Email,
    string   Position,
    decimal  Salary,
    string?  Department = null,
    string?  Notes      = null
);

/// <summary>
/// إنشاء موظف + حساب مستخدم له في خطوة واحدة
/// </summary>
public record CreateEmployeeWithUserDto(
    // بيانات الموظف
    string   FullName,
    string   PhoneNumber,
    string   Email,
    string   Position,
    decimal  Salary,
    string?  Department = null,
    string?  Notes      = null,
    // بيانات الـ User
    string   Username   = "",
    string   Password   = ""
);
