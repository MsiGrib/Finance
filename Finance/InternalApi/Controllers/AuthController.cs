using DataModel.ModelsRequest;
using DataModel.ModelsResponse;
using InternalApi.Service;
using InternalApi.Validators;
using Microsoft.AspNetCore.Http;
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

        public AuthController(ILogger<AuthController> logger, IUserService userService, BasicConfiguration basicConfiguration)
        {
            _logger = logger;
            _userService = userService;
            _basicConfiguration = basicConfiguration;
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

                var user = await _userService.AuthorizationUserAsync(request.Login, request.Password);

                if (user == null)
                {
                    return Unauthorized(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Данного пользователя нет в системе!"
                    });
                }

                string token = TokenUtility.CreateJWTToken(_basicConfiguration.SecretJWT, _basicConfiguration.IssuerJWT, _basicConfiguration.AudienceJWT, user.Id.ToString(), DateTime.Now.AddHours(3));

                return Ok(new ApiResponse<AuthorizationResponse>
                {
                    StatusCode = 200,
                    Message = "Авторизация прошла успешно!",
                    Data = new AuthorizationResponse
                    {
                        ExpirationTimeToken = DateTime.Now.AddHours(3),
                        Token = token,
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

                if (await _userService.IsExistsRegistrUserAsync(request.Login, request.Email))
                {
                    return BadRequest(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Такой пользователь уже есть!"
                    });
                }

                DateTime.TryParse(request.Birthday, out var birthday);
                var registrResult = await _userService.RegistrationUserAsync(request.Login, request.Password, request.NumberPhone, request.LastName, request.FirstName, request.Email, birthday);
                bool statusProfile = await _userProfileService.PrimaryProfileConsciousness(registrResult.First);

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
