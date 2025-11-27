using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Requests;

public class GetCoursesWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? Category { get; set; }
    public int? Level { get; set; } // enum: beginner|intermediate|advanced
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public double? Rating { get; set; }
    public List<int>? Tags { get; set; }
    public string? SortBy { get; set; } // title|price|rating|createdAt|studentsCount
    public string? SortOrder { get; set; } = "desc"; // asc|desc
    public string? ThumbnailUrl { get; set; }
}

public class CreateCourseRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string? DetailedDescription { get; set; }
    public string Instructor { get; set; }
    public int Duration { get; set; }
    public int Level { get; set; } // enum: beginner|intermediate|advanced
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public List<int> Tags { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? VideoUrl { get; set; }
}

public class UpdateCourseRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? DetailedDescription { get; set; }
    public string? Instructor { get; set; }
    public int? Duration { get; set; }
    public int? Level { get; set; }
    public decimal? Price { get; set; }
    public int? CategoryId { get; set; }
    public List<int>? Tags { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? VideoUrl { get; set; }
}