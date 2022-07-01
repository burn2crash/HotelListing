using HotelListing.DataAccess.Contracts;
using HotelListing.Models.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        // POST: api/account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] ApiUserDto userDto)
        {
            _logger.LogInformation($"Registration attempt for {userDto.Email}");
            
            try
            {
                var errors = await _authManager.RegisterAsync(userDto);

                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Register)} - User Registration " +
                    $"attempt for {userDto.Email}");

                return Problem($"Something went wrong in the {nameof(Register)}. Please contact support.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        // POST: api/account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            AuthResponseDto response = null;
            _logger.LogInformation($"Login attempt for {loginDto.Email}");

            try
            {
                response = await _authManager.LoginAsync(loginDto);

                if (response == null)
                    return Unauthorized();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Register)} - " +
                    $"Login {loginDto.Email} for {nameof(Login)}");

                return Problem($"Something went wrong in the {nameof(Login)}. Please contact support.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return Ok(response);
        }

        // POST: api/account/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            var authResponse = await _authManager.VerifyRefrshToken(request);

            if (authResponse == null)
                return Unauthorized();

            return Ok(authResponse);
        }
    }
}
