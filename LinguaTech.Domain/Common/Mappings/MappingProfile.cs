using System.Reflection;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using ProfileEntity = LinguaTech.Domain.Entities.Profile;

namespace LinguaTech.Domain.Common.Mappings;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        CreateCustomMappings();
    }

    private void CreateCustomMappings()
    {
        // User to GetUserDto mapping with custom logic
        CreateMap<User, GetUserDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Fullname : string.Empty))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty))
            .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile)); // <-- Explicit mapping for Profile property

        // User to GetAllUsersDto mapping
        CreateMap<User, GetAllUsersDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Fullname : string.Empty))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty));

        // User to GetUsersWithPaginationDto mapping
        CreateMap<User, GetUsersWithPaginationDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Fullname : string.Empty))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty));

        // User to UserDto mapping
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Fullname : string.Empty));

        // Profile to ProfileDto mapping
        CreateMap<ProfileEntity, ProfileDto>()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ?? string.Empty))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

        // CourseType mappings
        CreateMap<CourseType, CourseTypeDto>();
        CreateMap<CourseType, GetCourseTypeDto>();
        CreateMap<CourseType, GetAllCourseTypesDto>();
        CreateMap<CourseType, GetCourseTypesWithPaginationDto>();

        // CourseTag mappings
        CreateMap<CourseTag, CourseTagDto>();
        CreateMap<CourseTag, GetCourseTagDto>();
        CreateMap<CourseTag, GetAllCourseTagsDto>();
        CreateMap<CourseTag, GetCourseTagsWithPaginationDto>();

        // Course mappings with CourseType and Tags
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.CourseTypeName, opt => opt.MapFrom(src => src.CourseType != null ? src.CourseType.Name : null))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.CourseTags.Select(ct => ct.CourseTag).ToList()));

        CreateMap<Course, GetCourseDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
            .ForMember(dest => dest.CourseTypeName, opt => opt.MapFrom(src => src.CourseType != null ? src.CourseType.Name : null))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.CourseTags.Select(ct => ct.CourseTag).ToList()));

        CreateMap<Course, GetAllCoursesDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
            .ForMember(dest => dest.CourseTypeName, opt => opt.MapFrom(src => src.CourseType != null ? src.CourseType.Name : null))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.CourseTags.Select(ct => ct.CourseTag).ToList()));

        CreateMap<Course, GetCoursesWithPaginationDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
            .ForMember(dest => dest.CourseTypeName, opt => opt.MapFrom(src => src.CourseType != null ? src.CourseType.Name : null))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.CourseTags.Select(ct => ct.CourseTag).ToList()));
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mapFromType = typeof(IMapFrom<>);

        var mappingMethodName = nameof(IMapFrom<object>.Mapping);

        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

        var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        var argumentTypes = new Type[] { typeof(AutoMapper.Profile) };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, new object[] { this });
            }
            else
            {
                var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (var @interface in interfaces)
                    {
                        var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, new object[] { this });
                    }
                }
            }
        }
    }
}