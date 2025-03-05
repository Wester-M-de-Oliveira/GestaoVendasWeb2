using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace GestaoVendasWeb2.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TokenResponseDto?> AuthenticateAsync(LoginDto loginDto)
        {            
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return null;
            }
            
            // Verify password using BCrypt
            bool isPasswordValid = BC.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return null;
            }

            // Update last access time
            user.UltimoAcesso = DateTime.Now;
            await _context.SaveChangesAsync();

            return GenerateTokenResponse(user);
        }

        public async Task<TokenResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == registerDto.Username))
            {
                return null; // Username already exists
            }

            var passwordHash = BC.HashPassword(registerDto.Password);

            var newUser = new Usuario
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                Nome = registerDto.Nome,
                Email = registerDto.Email,
                Role = registerDto.Role,
                DataCriacao = DateTime.Now,
                UltimoAcesso = DateTime.Now
            };

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            return GenerateTokenResponse(newUser);
        }

        private TokenResponseDto GenerateTokenResponse(Usuario user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "O58D8T3EW5OMuQORYQZm6Uh2qwFttIu6"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(24);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Nome = user.Nome,
                Email = user.Email,
                Role = user.Role,
                Ativo = user.Ativo,
                DataCriacao = user.DataCriacao,
                UltimoAcesso = user.UltimoAcesso
            };

            return new TokenResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires,
                User = userDto
            };
        }
    }
}
