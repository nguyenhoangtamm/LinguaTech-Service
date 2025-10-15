using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Domain.DTOs.Responses;

public class QuestionResponse
{
}

public class QuestionDto : IMapFrom<Question>
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int QuestionTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class GetQuestionDto : IMapFrom<Question>
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int QuestionTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string QuestionTypeName { get; set; } = string.Empty;
    public string LessonTitle { get; set; } = string.Empty;
    public string ModuleTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

public class GetAllQuestionsDto : IMapFrom<Question>
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int QuestionTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string QuestionTypeName { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}

public class GetQuestionsWithPaginationDto : IMapFrom<Question>
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int QuestionTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string QuestionTypeName { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}