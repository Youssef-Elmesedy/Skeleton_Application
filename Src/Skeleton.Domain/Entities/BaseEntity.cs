namespace Skeleton.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; private set; }
    public DateTime? CreateDate { get; private set; }
    public string? CreateBy { get; private set; } = string.Empty;
    public DateTime? ModifiedDate { get; private set; }
    public string? ModifiedBy { get; private set; } = string.Empty;

    // Constructor for creating a new entity
    public BaseEntity()
    {
        Id = Guid.NewGuid();
        CreateDate = DateTime.UtcNow;

    }

    // Method to set the creator of the entity
    public void SetCreateBy(string createBy)
    {
        if (string.IsNullOrWhiteSpace(createBy))
            throw new ArgumentException("CreateBy is required");
        CreateBy = createBy;
    }

    // Method to set the modifier of the entity
    public void SetModifideBy(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy is required");

        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }
}
