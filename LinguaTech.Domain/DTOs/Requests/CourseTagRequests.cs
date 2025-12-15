namespace LinguaTech.Domain.DTOs.Requests;

public record CreateCourseTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public record UpdateCourseTagRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public record GetCourseTagsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public bool? IsActive { get; set; }
}

public record UpdateCourseTagWithIdRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public record DeleteCourseTagRequest
{
    public int Id { get; set; }
}