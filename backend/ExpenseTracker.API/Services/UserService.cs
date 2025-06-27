using AutoMapper;
using ExpenseTracker.API.DTOs.Users;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Repositories;
using ExpenseTracker.API.Interfaces.Services;

namespace ExpenseTracker.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserProfileDto>> GetUserProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result<UserProfileDto>.Failure("User not found.");
            }

            var userProfileDto = _mapper.Map<UserProfileDto>(user);
            return Result<UserProfileDto>.Success(userProfileDto);
        }

        public async Task<Result<UserProfileDto>> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequestDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result<UserProfileDto>.Failure("User not found.");
            }

            // AutoMapper will only map the non-null properties from the DTO to the entity
            _mapper.Map(updateDto, user);

            // EF Core will automatically track the changes, no need to call an Update method
            // The audit information will be set in the DbContext SaveChangesAsync override
            await _userRepository.SaveChangesAsync();

            var updatedUserProfileDto = _mapper.Map<UserProfileDto>(user);
            return Result<UserProfileDto>.Success(updatedUserProfileDto);
        }
    }
}