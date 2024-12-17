using Business.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserBalanceService _userBalanceService;

        public UserController(IUserBalanceService userBalanceService)
        {
            _userBalanceService = userBalanceService;
        }

        // Get the user's balance
        [HttpGet("{userId}/balance")]
        public async Task<IActionResult> GetUserBalance(int userId)
        {
            try
            {
                var balance = await _userBalanceService.GetUserBalanceAsync(userId);
                return Ok(new { UserId = userId, Balance = balance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to fetch balance", Error = ex.Message });
            }
        }

        // Add money to the user's balance
        [HttpPost("{userId}/balance/add")]
        public async Task<IActionResult> AddBalance(int userId, [FromBody] BalanceRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new { Message = "Amount must be greater than 0." });
            }

            try
            {
                await _userBalanceService.AddBalanceAsync(userId, request.Amount);
                var updatedBalance = await _userBalanceService.GetUserBalanceAsync(userId);
                return Ok(new { Message = "Balance updated successfully", UpdatedBalance = updatedBalance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to update balance", Error = ex.Message });
            }
        }

        // Retrieve user information by ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userBalanceService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {userId} not found." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to retrieve user", Error = ex.Message });
            }
        }
    }

    // DTO for balance updates
    public class BalanceRequest
    {
        public decimal Amount { get; set; }
    }
}
