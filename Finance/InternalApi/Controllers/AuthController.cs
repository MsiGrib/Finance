using Cryptograf;
using DataModel.ModelsRequest;
using DataModel.ModelsResponse;
using InternalApi.Service;
using InternalApi.Utilitys;
using InternalApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;
        private readonly BasicConfiguration _basicConfiguration;
        private readonly ICryptoService _cryptoService;

        public AuthController(ILogger<AuthController> logger, IUserService userService, BasicConfiguration basicConfiguration, ICryptoService cryptoService)
        {
            _logger = logger;
            _userService = userService;
            _basicConfiguration = basicConfiguration;
            _cryptoService = cryptoService;
        }

        [HttpPost("Authorization")]
        public async Task<IActionResult> AuthorizationUser([FromBody] AuthorizationRequest request)
        {
            try
            {
                if (!AuthCValidator.IsValidAuthorizationRequest(request))
                {
                    return BadRequest(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Данные пустые или некорректны!"
                    });
                }

                var user = await _userService.AuthorizationUserAsync(_cryptoService.DecryptString(request.Login), _cryptoService.DecryptString(request.Password));

                if (user == null)
                {
                    return Unauthorized(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Данного пользователя нет в системе!"
                    });
                }

                string token = TokenUtility.CreateJWTToken(_basicConfiguration.SecretJWT, _basicConfiguration.IssuerJWT, _basicConfiguration.AudienceJWT, user.Id.ToString(), DateTime.Now.AddHours(6));

                return Ok(new ApiResponse<AuthorizationResponse>
                {
                    StatusCode = 200,
                    Message = "Авторизация прошла успешно!",
                    Data = new AuthorizationResponse
                    {
                        ExpirationTimeToken = DateTime.Now.AddHours(6),
                        Token = _cryptoService.EncryptString(token),
                    }
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

        [HttpPost("Registration")]
        public async Task<IActionResult> RegistrationUser([FromBody] RegistrationRequest request)
        {
            try
            {
                if (!AuthCValidator.IsValidRegistrationRequest(request))
                {
                    return BadRequest(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Данные пустые или некорректны!"
                    });
                }

                string login = _cryptoService.DecryptString(request.Login);
                string password = _cryptoService.DecryptString(request.Password);
                string email = _cryptoService.DecryptString(request.Email);

                if (await _userService.IsExistsRegistrUserAsync(login, email))
                {
                    return BadRequest(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Такой пользователь уже есть!"
                    });
                }

                var registrResult = await _userService.RegistrationUserAsync(login, password, email);

                if (!registrResult.Second)
                {
                    new ObjectResult(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Не получилось зарегистрировать пользователя!"
                    });
                }

                return Ok(new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Регистрация прошла успешно!"
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
