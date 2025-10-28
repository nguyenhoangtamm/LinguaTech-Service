namespace LinguaTech.Domain.DTOs.Requests;

public class CreateModuleRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public int CourseId { get; set; }
}

public class UpdateModuleRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
    public int? CourseId { get; set; }
}

public class GetModulesWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? CourseId { get; set; }
    public int? ParentId { get; set; }
}