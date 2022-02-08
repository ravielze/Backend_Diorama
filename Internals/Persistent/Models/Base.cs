public class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class BaseEntitySoftDelete : BaseEntity
{
    public Nullable<DateTime> DeletedAt { get; set; } = null;
}

public interface IBeforeSave
{
    bool BeforeSave(IBeforeSave entity);
}