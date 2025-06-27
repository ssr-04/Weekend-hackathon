using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequestDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.IsSuccess)
            {
                // Using Conflict (409) for existing user, otherwise BadRequest (400)
                if (result.Error!.Contains("exists"))
                    return Conflict(new { message = result.Error });

                return BadRequest(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginRequestDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto requestDto)
        {
            var result = await _authService.RefreshTokenAsync(requestDto.RefreshToken);
            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto requestDto)
        {
            var result = await _authService.LogoutAsync(requestDto.RefreshToken);

            return NoContent();
        }
    }
}