namespace QueryFiltersLab;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}

public class Blog : ISoftDelete
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string TenantId { get; set; }
    public bool IsDeleted { get; set; }
}

public class Post : ISoftDelete
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsDeleted { get; set; }
}
