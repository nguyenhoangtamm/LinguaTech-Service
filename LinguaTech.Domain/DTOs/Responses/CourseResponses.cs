using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Responses;

public class CourseDetailResType
{
    public CourseType Data { get; set; } = new CourseType();
    public string Message { get; set; } = string.Empty;
}

public class CourseType : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Level { get; set; } = string.Empty; // enum: beginner|intermediate|advanced
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int StudentsCount { get; set; }
    public CourseCategoryType Category { get; set; } = new CourseCategoryType();
    public List<string> Tags { get; set; } = new List<string>();
    public string Thumbnail { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
}

public class CourseCategoryType : IMapFrom<CourseCategory>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GetCourseDto : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int StudentsCount { get; set; }
    public CourseCategoryType Category { get; set; } = new CourseCategoryType();
    public List<string> Tags { get; set; } = new List<string>();
    public string Thumbnail { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
}

public class GetAllCoursesDto : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
}

public class GetCoursesWithPaginationDto : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}