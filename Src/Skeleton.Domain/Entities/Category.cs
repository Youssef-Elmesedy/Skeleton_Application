using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;

    // Navigation property
    public List<Product> Products { get; private set; } = new();

    public Category(string name, string description)
    {
        SetName(name, description);
    }

    public void Update(string name, string description)
    {
        SetName(name, description);
    }

    private void SetName(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException(
                ErrorType.Validation,
                "Category name is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessException(
                ErrorType.Validation,
                "Category Description is required.");

        Name = name.Trim();
        Description = description;
    }
}
