using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Domain.DTOs.Responses;

public class AssignmentResponse
{
}

public class AssignmentDto : IMapFrom<Assignment>
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public double MaxScore { get; set; }
}

public class GetAssignmentDto : IMapFrom<Assignment>
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public double MaxScore { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public string ModuleTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

public class GetAllAssignmentsDto : IMapFrom<Assignment>
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public double MaxScore { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public string ModuleTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}

public class GetAssignmentsWithPaginationDto : IMapFrom<Assignment>
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public double MaxScore { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public string ModuleTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}