using Cryptograf;
using DataModel.ModelsResponse;
using InternalApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ReportService _reportService;
        private readonly ICryptoService _cryptoService;

        public ReportController(ILogger<AuthController> logger, ReportService reportService, ICryptoService cryptoService)
        {
            _logger = logger;
            _reportService = reportService;
            _cryptoService = cryptoService;
        }

        [Authorize]
        [HttpGet("Word")]
        public async Task<IActionResult> GetReportWord()
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

                //var bytes = _cryptoService.Encrypt(await _reportService.GenerateReportWordAsync());
                var bytes = await _reportService.GenerateReportWordAsync();
                var fileName = _cryptoService.EncryptString($"Report_{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}.docx");
                var payload = new FileResponse
                {
                    Bytes = bytes,
                    FileName = fileName,
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                };

                return Ok(new ApiResponse<FileResponse>
                {
                    StatusCode = 200,
                    Message = "ОК",
                    Data = payload
                });
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

        [Authorize]
        [HttpGet("Excel")]
        public async Task<IActionResult> GetReportExcel()
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

                //var bytes = _cryptoService.Encrypt(await _reportService.GenerateReportExcelAsync());
                var bytes = await _reportService.GenerateReportExcelAsync();
                var fileName = _cryptoService.EncryptString($"Report_{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}.xlsx");
                var payload = new FileResponse
                {
                    Bytes = bytes,
                    FileName = fileName,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };

                return Ok(new ApiResponse<FileResponse>
                {
                    StatusCode = 200,
                    Message = "ОК",
                    Data = payload
                });
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

        [Authorize]
        [HttpGet("Json")]
        public async Task<IActionResult> GetReportJson()
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

                var bytes = _cryptoService.Encrypt(await _reportService.GenerateReportJsonAsync());
                var fileName = _cryptoService.EncryptString($"Report_{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}.json");
                var payload = new FileResponse
                {
                    Bytes = bytes,
                    FileName = fileName,
                    ContentType = "application/json"
                };

                return Ok(new ApiResponse<FileResponse>
                {
                    StatusCode = 200,
                    Message = "ОК",
                    Data = payload
                });
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
