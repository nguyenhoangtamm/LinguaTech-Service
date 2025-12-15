using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Domain.DTOs.Responses;

public class LessonType : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public bool IsCompleted { get; set; }
    public int ModuleId { get; set; }
    public int SectionsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string VideoUrl { get; set; }
}

public class LessonWithMaterialsType : LessonType
{
    public List<MaterialType> Materials { get; set; } = new List<MaterialType>();
}

public class GetLessonDto : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public int ModuleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GetAllLessonsDto : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public int ModuleId { get; set; }
}

public class GetLessonsWithPaginationDto : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public int ModuleId { get; set; }
    public DateTime CreatedAt { get; set; }
}