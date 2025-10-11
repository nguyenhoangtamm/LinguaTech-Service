using AutoMapper;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using ProfileEntity = LinguaTech.Domain.Entities.Profile;

namespace LinguaTech.Application.Common.Mappings;

public class AutoMapperProfile : AutoMapper.Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, GetUserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

        CreateMap<User, GetAllUsersDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Fullname : string.Empty));

        CreateMap<User, GetUsersWithPaginationDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Fullname : string.Empty));

        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)) // Will be hashed in service
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"));

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)) // Will be hashed in service
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Profile mappings
        CreateMap<ProfileEntity, ProfileDto>();
        
        CreateMap<CreateUserRequest, ProfileEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateTime.MinValue))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => string.Empty));
    }
}