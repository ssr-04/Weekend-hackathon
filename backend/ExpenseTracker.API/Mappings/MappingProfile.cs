using AutoMapper;
using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.DTOs.Categories;
using ExpenseTracker.API.DTOs.Expenses;
using ExpenseTracker.API.DTOs.Users;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.DTOs.Dashboard;

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

            CreateMap<Expense, HighestExpenseResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Period, opt => opt.Ignore()); // Set in service

            // Maps the internal CategoryExpense helper class to the public DTO.
            CreateMap<ExpenseTracker.API.Helpers.CategoryExpense, CategoryBreakdownItemDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())   // Set in service
                .ForMember(dest => dest.Percentage, opt => opt.Ignore()); // Set in service

            //  Maps the internal SpendingTrend helper class to the public DTO.
            CreateMap<ExpenseTracker.API.Helpers.SpendingTrend, SpendingTrendItemDto>();

            //  Maps the internal DailySpending helper class to the public DTO.
            CreateMap<ExpenseTracker.API.Helpers.DailySpending, DailySpendingItemDto>();



        }
    }
}