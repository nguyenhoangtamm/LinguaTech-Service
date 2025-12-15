using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Question : BaseAuditableEntity
{
    public int AssignmentId { get; set; }
    public int QuestionTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }

    // Navigation properties
    public virtual Assignment Assignment { get; set; } = null!;
    public virtual QuestionType QuestionType { get; set; } = null!;
    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
