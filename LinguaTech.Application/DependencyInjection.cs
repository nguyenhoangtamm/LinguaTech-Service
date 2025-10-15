using System.Reflection;
using FluentValidation;
using LinguaTech.Application.Services;
using LinguaTech.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LinguaTech.Domain.Common.Mappings;

namespace LinguaTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AutoMapper - include Domain assembly so DTO mapping profiles are discovered
        services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(MappingProfile).Assembly);

        // Register HTTP Context Accessor
        services.AddHttpContextAccessor();

        // Register application services
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IModuleService, ModuleService>();
        services.AddTransient<ILessonService, LessonService>();
        services.AddTransient<IAssignmentService, AssignmentService>();
        services.AddTransient<IQuestionService, QuestionService>();
        services.AddTransient<IMaterialService, MaterialService>();
        services.AddTransient<IEnrollmentService, EnrollmentService>();

        // Register new services
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IAnswerService, AnswerService>();
        services.AddTransient<IClassService, ClassService>();
        services.AddTransient<IProfileService, ProfileService>();
        services.AddTransient<ISubmissionService, SubmissionService>();

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
