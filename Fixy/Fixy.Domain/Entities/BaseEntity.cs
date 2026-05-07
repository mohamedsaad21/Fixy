namespace Fixy.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
