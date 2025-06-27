using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.DTOs.Users;
using ExpenseTracker.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    // The [Authorize] attribute is inherited from BaseApiController
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _userService.GetUserProfileAsync(userId);
            
            // This endpoint should always find the user if they are authenticated
            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequestDto updateDto)
        {
            var userId = GetCurrentUserId();
            var result = await _userService.UpdateUserProfileAsync(userId, updateDto);

            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        // NOTE: Even though this is in the UsersController, the logic for changing a password
        // is so tightly coupled with authentication (hashing, credential checking) that it's
        // better kept in the AuthService. We need to add the method there.
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto changePasswordDto)
        {
            var userId = GetCurrentUserId();
            
            // We need to add this method to our AuthService
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error });
            }

            return NoContent();
        }
    }
}