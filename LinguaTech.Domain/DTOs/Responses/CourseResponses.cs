using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Responses;

public class CourseResponse
{
}

public class CourseDto : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public int Level { get; set; }
    public CourseStatus Status { get; set; }
    public int Duration { get; set; }
    public string? CourseTypeName { get; set; }
    public List<CourseTagDto> Tags { get; set; } = new List<CourseTagDto>();
}

public class GetCourseDto : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int Level { get; set; }
    public CourseStatus Status { get; set; }
    public int Duration { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? CourseTypeId { get; set; }
    public string? CourseTypeName { get; set; }
    public List<CourseTagDto> Tags { get; set; } = new List<CourseTagDto>();
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

public class GetAllCoursesDto : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public int Level { get; set; }
    public CourseStatus Status { get; set; }
    public int Duration { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? CourseTypeName { get; set; }
    public List<CourseTagDto> Tags { get; set; } = new List<CourseTagDto>();
    public DateTime? CreatedDate { get; set; }
}

public class GetCoursesWithPaginationDto : IMapFrom<Course>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int Level { get; set; }
    public CourseStatus Status { get; set; }
    public int Duration { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? CourseTypeName { get; set; }
    public List<CourseTagDto> Tags { get; set; } = new List<CourseTagDto>();
    public DateTime? CreatedDate { get; set; }
}