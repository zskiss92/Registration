using Authentication.Models;
using Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<bool>> Register(Register request)
        {
            var user = await _authService.Register(new User {Email = request.Email}, request.Password);

            if(!user)
            {
                return BadRequest("User already exists");
            }

            return Ok("User successfully registered");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(Login request)
        {
            var response = await _authService.Login(request.Email, request.Password);

            if(response == "User not found" || response == "Wrong password")
            {
                return BadRequest("User not found or wrong password");
            }
            else if (response == "Email not verified")
            {
                return BadRequest("User email not verified");
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> VerifyEmail(string id)
        {
            var response = await _authService.VerifyEmail(id);

            if(response != "Ok")
            {
                return BadRequest(response);
            }

            return Ok("Success");
        }
    }
}
