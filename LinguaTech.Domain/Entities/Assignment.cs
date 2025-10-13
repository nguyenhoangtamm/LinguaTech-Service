using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Assignment : BaseAuditableEntity
{
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public double MaxScore { get; set; }

    // Navigation properties
    public virtual Lesson Lesson { get; set; } = null!;
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
