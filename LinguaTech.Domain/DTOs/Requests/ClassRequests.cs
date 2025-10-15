namespace LinguaTech.Domain.DTOs.Requests;

public class CreateClassRequest
{
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Schedule { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int MaxStudents { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class UpdateClassRequest
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Schedule { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int MaxStudents { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class GetClassesWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? CourseId { get; set; }
    public string? Status { get; set; }
}