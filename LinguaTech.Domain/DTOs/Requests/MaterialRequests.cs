namespace LinguaTech.Domain.DTOs.Requests;

public class GetMaterialsWithPaginationQuery
{
    public int? LessonId { get; set; }
    public string? Type { get; set; } // enum: pdf|video|image|document|audio
    public string? Keyword { get; set; }
    public string? FileType { get; set; }
    public long? MinSize { get; set; }
    public long? MaxSize { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CreateMaterialRequest
{
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // enum: pdf|video|image|document|audio
    public long Size { get; set; }
    public int LessonId { get; set; }
}

public class UpdateMaterialRequest
{
    public string? Title { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public string? FileType { get; set; }
    public long? Size { get; set; }
    public int? LessonId { get; set; }
}