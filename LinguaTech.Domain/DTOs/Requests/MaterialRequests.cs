namespace LinguaTech.Domain.DTOs.Requests;

public record CreateMaterialRequest
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long Size { get; set; }
}

public record UpdateMaterialRequest
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public string? FileType { get; set; }
    public long? Size { get; set; }
}

public record GetMaterialsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public string? FileType { get; set; }
    public long? MinSize { get; set; }
    public long? MaxSize { get; set; }
}