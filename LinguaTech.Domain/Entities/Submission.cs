using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Submission : BaseAuditableEntity
{
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public int Status { get; set; } // 0 = Draft, 1 = Submitted, 2 = Graded
    public DateTime? SubmittedAt { get; set; }
    public DateTime? GradedAt { get; set; }

    // Navigation properties
    public virtual Assignment Assignment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
