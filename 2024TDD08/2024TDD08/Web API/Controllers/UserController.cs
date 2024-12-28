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
                return Ok(new UserBalanceResponse
                {
                    UserId = userId,
                    Balance = balance
                });
            }
            catch
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Failed to fetch balance."
                });
            }
        }

        // Add money to the user's balance
        [HttpPost("{userId}/balance/add")]
        public async Task<IActionResult> AddBalance(int userId, [FromBody] BalanceRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Amount must be greater than 0."
                });
            }

            try
            {
                await _userBalanceService.AddBalanceAsync(userId, request.Amount);
                var updatedBalance = await _userBalanceService.GetUserBalanceAsync(userId);

                return Ok(new BalanceUpdateResponse
                {
                    Message = "Balance updated successfully.",
                    UpdatedBalance = updatedBalance
                });
            }
            catch
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Failed to update balance."
                });
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
                    return NotFound(new ErrorResponse
                    {
                        Message = $"User with ID {userId} not found."
                    });
                }

                return Ok(user);
            }
            catch
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Failed to retrieve user."
                });
            }
        }
    }

    // DTO for balance updates
    public class BalanceRequest
    {
        public decimal Amount { get; set; }
    }

    public class UserBalanceResponse
    {
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }

    public class BalanceUpdateResponse
    {
        public string Message { get; set; }
        public decimal UpdatedBalance { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}
