namespace LinguaTech.Domain.DTOs.Requests;

public record CreateModuleRequest
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }
}

public record UpdateModuleRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int? Order { get; set; }
    public int? ParentId { get; set; }
}

public record GetModulesWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? CourseId { get; set; }
    public int? ParentId { get; set; }
}