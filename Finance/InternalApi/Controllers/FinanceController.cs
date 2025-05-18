using Cryptograf;
using DataModel.DataStructures;
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
        private readonly ITableService _tableService;
        private readonly BasicConfiguration _basicConfiguration;
        private readonly FinanceService _financeService;
        private readonly ICryptoService _cryptoService;

        public FinanceController(ILogger<AuthController> logger,
            IUserService userService, ITableService tableService,
            BasicConfiguration basicConfiguration, FinanceService financeService,
            ICryptoService cryptoService)
        {
            _logger = logger;
            _userService = userService;
            _tableService = tableService;
            _basicConfiguration = basicConfiguration;
            _financeService = financeService;
            _cryptoService = cryptoService;
        }

        [Authorize]
        [HttpGet("MainBoard")]
        public async Task<IActionResult> GetMainBoard()
        {
            try
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

                var result = new List<Tuple<int, TableView>>();
                var mainBoaeds = await _financeService.GetFinanceViewsAsync();

                foreach (var item in mainBoaeds)
                {
                    var currentTable = await _tableService.GetTableByIdAsync(item.Second.Id);

                    if (currentTable == null)
                        continue;

                    var tmpTableView = new TableView
                    {
                        Name = _cryptoService.EncryptString(currentTable.Name),
                        SubName = _cryptoService.EncryptString(currentTable.SubName),
                        Currency = _cryptoService.EncryptString(currentTable.Currency),
                        ImageBase64 = _cryptoService.EncryptString(await _financeService.ConvertImageToBase64(currentTable.ImagePath)),
                        Plots = currentTable.Plots.ToList(),
                    };
                    var tmp = new Tuple<int, TableView>(item.First, tmpTableView);

                    result.Add(tmp);
                }

                var response = new ApiResponse<MainBoardResponse>
                {
                    StatusCode = 200,
                    Message = "Данные получены успешно!",
                    Data = new MainBoardResponse
                    {
                        MainBoards = result,
                    }
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return new ObjectResult(new BaseResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Ошибка на стороне сервера!"
                });
            }
        }
    }
}
