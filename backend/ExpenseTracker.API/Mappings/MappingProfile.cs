using AutoMapper;
using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.DTOs.Categories;
using ExpenseTracker.API.DTOs.Expenses;
using ExpenseTracker.API.DTOs.Users;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;

namespace ExpenseTracker.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            // User & Auth Mappings
            CreateMap<User, UserProfileDto>();

            CreateMap<UpdateUserProfileRequestDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Note: RegisterRequestDto and LoginRequestDto are mapped manually in AuthService Initially.

            
            // Category Mappings
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryCreateRequestDto, Category>();
            CreateMap<CategoryUpdateRequestDto, Category>()
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            // Expense Mappings

            // Mapping from Entity to Response DTO
            CreateMap<Expense, ExpenseResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                // **TIMEZONE CONVERSION: UTC (database) to IST String (response)**
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcToIst(src.Date)));

            // Mapping from Create DTO to Entity
            CreateMap<ExpenseCreateRequestDto, Expense>()
                .ForMember(dest => dest.Date, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore());


            // Mapping from Update DTO to Entity
            CreateMap<ExpenseUpdateRequestDto, Expense>()
                .ForMember(dest => dest.Date, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()) // Ignore both
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



        }
    }
}