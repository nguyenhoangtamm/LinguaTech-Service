using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Domain.DTOs.Responses;

public class ModuleResponse
{
}

public class ModuleDto : IMapFrom<Module>
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }
}

public class GetModuleDto : IMapFrom<Module>
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? ParentTitle { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

public class GetAllModulesDto : IMapFrom<Module>
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? ParentTitle { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class GetModulesWithPaginationDto : IMapFrom<Module>
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? ParentTitle { get; set; }
    public DateTime? CreatedDate { get; set; }
}