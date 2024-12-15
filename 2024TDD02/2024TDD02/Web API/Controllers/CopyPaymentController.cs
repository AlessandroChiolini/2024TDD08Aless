using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Business.Services;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CopyPaymentController : ControllerBase
    {
        private readonly ICopyPaymentService _copyPaymentService;

        public CopyPaymentController(ICopyPaymentService copyPaymentService)
        {
            _copyPaymentService = copyPaymentService;
        }

        [HttpGet("GetUserCopyTransactions")]
        public IActionResult GetUserCopyTransactions(int userId)
        {
            try
            {
                var copyTransactions = _copyPaymentService.GetUserCopyTransactions(userId);
                return Ok(copyTransactions);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ProcessCopyPayment")]
        public async Task<IActionResult> ProcessCopyPayment(int userId, int numberOfCopies)
        {
            try
            {
                var result = await _copyPaymentService.ProcessCopyPaymentAsync(userId, numberOfCopies);
                if (result)
                {
                    return Ok("Payment processed successfully.");
                }
                else
                {
                    return BadRequest("Your account does not have sufficient funds for the payment. Press 4 to add balance.");
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
