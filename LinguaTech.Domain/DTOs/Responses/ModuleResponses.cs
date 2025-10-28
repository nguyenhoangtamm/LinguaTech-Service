using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Domain.DTOs.Responses;

public class ModuleType : IMapFrom<Module>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ModuleWithLessonsType : ModuleType
{
    public List<LessonWithMaterialsType> Lessons { get; set; } = new List<LessonWithMaterialsType>();
}

public class GetModuleDto : IMapFrom<Module>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GetAllModulesDto : IMapFrom<Module>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public int CourseId { get; set; }
}

public class GetModulesWithPaginationDto : IMapFrom<Module>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
}