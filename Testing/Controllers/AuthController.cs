using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pms.Dto.AuthDto;
using Pms.Service.Interface;
using PmsRepository.Interface;

namespace Pms.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {


        private readonly IAuthService _authService;
        private readonly Ijwtservice _jwtService;

        public AuthController(IAuthService authService,Ijwtservice jwtservice)
        {
            _authService = authService;
            _jwtService = jwtservice;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.ValidateUserAsync(dto);
            if (user == null)
                return Unauthorized("Invalid email or password");

               var token = _jwtService.GenerateToken(user);


            return Ok(new
            { 
                token,
                user.UserId,
                user.Email,
                user.FirstName,
                user.LastName
            });
        }
    }
}
