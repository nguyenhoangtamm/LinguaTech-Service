using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Domain.DTOs.Responses;

public class LessonResponse
{
}

public class LessonDto : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class GetLessonDto : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public string ModuleTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

public class GetAllLessonsDto : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Order { get; set; }
    public string ModuleTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}

public class GetLessonsWithPaginationDto : IMapFrom<Lesson>
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Order { get; set; }
    public string ModuleTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}