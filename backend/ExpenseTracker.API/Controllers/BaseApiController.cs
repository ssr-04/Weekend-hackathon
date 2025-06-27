using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Authorize] // All controllers inheriting from this will require authentication by default
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            // This should not happen if the [Authorize] attribute is present,
            // as the JWT middleware validates the token.
            throw new InvalidOperationException("User ID not found in token.");
        }
    }
}