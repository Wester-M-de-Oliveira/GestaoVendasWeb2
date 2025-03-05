using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;
using System.Text;

namespace GestaoVendasWeb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autenticar usuário
        /// </summary>
        /// <param name="loginDto">Credenciais de login</param>
        /// <returns>Token JWT para autenticação</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.AuthenticateAsync(loginDto);
            if (result == null)
            {
                return Unauthorized(new { message = "Usuário ou senha inválidos" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Registrar novo usuário
        /// </summary>
        /// <param name="registerDto">Dados do novo usuário</param>
        /// <returns>Token JWT para autenticação</returns>
        [HttpPost("register")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
            {
                return BadRequest(new { message = "Nome de usuário já existe" });
            }

            return CreatedAtAction(nameof(Login), result);
        }
    }
}