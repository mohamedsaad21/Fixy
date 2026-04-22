namespace Fixy.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
