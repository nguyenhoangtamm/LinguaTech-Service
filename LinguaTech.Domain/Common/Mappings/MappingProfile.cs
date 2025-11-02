using System.Reflection;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using ProfileEntity = LinguaTech.Domain.Entities.Profile;
using CourseTypeEntity = LinguaTech.Domain.Entities.CourseType;

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

        // Course mappings
        CreateMap<Course, LinguaTech.Domain.DTOs.Responses.CourseType>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.CourseTags.Select(ct => ct.CourseTag.Name)))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<CourseCategory, CourseCategoryType>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString())); // Updated mapping to convert Id to string

        // Lesson mappings
        CreateMap<Lesson, LessonType>();

        CreateMap<Lesson, LessonWithMaterialsType>();

        // Section mappings
        CreateMap<Section, SectionType>();

        // Module mappings
        CreateMap<Entities.Module, ModuleType>();

        CreateMap<Entities.Module, ModuleWithLessonsType>();

        // Material mappings
        CreateMap<Material, MaterialType>();

        // Enrollment mappings
        CreateMap<Enrollment, EnrollmentType>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString().ToLower()))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.Progress));

        CreateMap<Enrollment, UserEnrollmentType>()
            .ForMember(dest => dest.Enrollment, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course));

        CreateMap<EnrollmentProgress, EnrollmentProgressType>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));

        // Submission mappings
        CreateMap<Submission, SubmissionResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.AssignmentId, opt => opt.MapFrom(src => src.AssignmentId.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
            .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(src => src.SubmittedAt.HasValue ? src.SubmittedAt.Value.ToString("O") : null))
            .ForMember(dest => dest.GradedAt, opt => opt.MapFrom(src => src.GradedAt.HasValue ? src.GradedAt.Value.ToString("O") : null))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

        CreateMap<Answer, AnswerResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId.ToString()))
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.AnswerText))
            .ForMember(dest => dest.SelectedOptionId, opt => opt.MapFrom(src => src.SelectedOptionId.HasValue ? src.SelectedOptionId.Value.ToString() : null));

        // Question and QuestionOption mappings for assignment details
        CreateMap<QuestionOption, QuestionOptionDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.OptionText))
            .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect));

        CreateMap<Question, QuestionDetailDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedDate))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(dest => dest.AssignmentId, opt => opt.MapFrom(src => src.AssignmentId))
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.QuestionType != null ? src.QuestionType.Name : "Unknown"))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => (string)null))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.QuestionOptions.Where(qo => !qo.IsDeleted).ToList()));

        // Assignment mapping with questions
        CreateMap<Assignment, GetAssignmentDto>()
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions.Where(q => !q.IsDeleted).ToList()));
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