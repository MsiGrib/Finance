using DataModel.ModelsRequest;
using DataModel.ModelsResponse;
using InternalApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;
        private readonly BasicConfiguration _basicConfiguration;
        private readonly FinanceService _financeService;

        public FinanceController(ILogger<AuthController> logger, IUserService userService, BasicConfiguration basicConfiguration, FinanceService financeService)
        {
            _logger = logger;
            _userService = userService;
            _basicConfiguration = basicConfiguration;
            _financeService = financeService;
        }

        [Authorize]
        [HttpGet("MainBoard")]
        public async Task<IActionResult> GetMainBoard()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userGuid))
            {
                return BadRequest(new BaseResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Не прошли авторизацию!"
                });
            }


        }
    }
}
