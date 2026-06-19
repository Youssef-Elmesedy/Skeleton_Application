using FluentValidation;
using Skeleton.Application.Feature.Employee.EmployeeDto;

namespace Skeleton.Application.Feature.Employee.Validators;

public class EmployeeCreateValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeCreateValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3).MaximumLength(200);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20)
            .Matches(@"^\+?[0-9\s\-]{7,20}$").WithMessage("Invalid phone number format.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zero.");
        RuleFor(x => x.Department).MaximumLength(100).When(x => x.Department is not null);
    }
}

public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3).MaximumLength(200);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20)
            .Matches(@"^\+?[0-9\s\-]{7,20}$").WithMessage("Invalid phone number format.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zero.");
    }
}
