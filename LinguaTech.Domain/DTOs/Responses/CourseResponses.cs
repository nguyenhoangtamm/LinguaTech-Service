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
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DetailedDescription { get; set; }
    public string Instructor { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Level { get; set; } // enum: 1|2|3 (beginner|intermediate|advanced)
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int StudentsCount { get; set; }
    public CourseCategoryType Category { get; set; } = new CourseCategoryType();
    public List<int> Tags { get; set; } = new List<int>();
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
    public int ModulesCount { get; set; }
    public int LessonsCount { get; set; }
}

public class CourseCategoryType : IMapFrom<CourseCategory>
{
    public string Id { get; set; } = string.Empty;
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
    public string? DetailedDescription { get; set; }
    public string Instructor { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int StudentsCount { get; set; }
    public CourseCategoryType Category { get; set; } = new CourseCategoryType();
    public List<string> Tags { get; set; } = new List<string>();
    public string ThumbnailUrl { get; set; } = string.Empty;
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
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CourseDetailType
{
    public CourseType Course { get; set; } = new CourseType();
    public int ModulesCount { get; set; }
    public int LessonsCount { get; set; }
    public List<ModuleWithLessonsType> Modules { get; set; } = new List<ModuleWithLessonsType>();
    public List<MaterialType> Materials { get; set; } = new List<MaterialType>();
    public InstructorDetailType Instructor { get; set; } = new InstructorDetailType();
    public List<CourseReviewType> Reviews { get; set; } = new List<CourseReviewType>();
    public List<CourseFaqType>? Faqs { get; set; }
}

public class InstructorDetailType
{
    public string Name { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public int Students { get; set; }
    public int Courses { get; set; }
    public double Rating { get; set; }
    public string Bio { get; set; } = string.Empty;
}

public class CourseReviewType
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int? Helpful { get; set; }
}

public class CourseFaqType
{
    public string Id { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}