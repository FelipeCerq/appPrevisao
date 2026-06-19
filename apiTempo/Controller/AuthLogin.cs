using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using apiTempo.Models;

namespace apiTempo.Controllers
{
    [ApiController]
    [Route("api/authLogin")]
    public class AuthLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;        
        private readonly AppDbContext _context;
        public AuthLoginController(IConfiguration configuration, AppDbContext context)
        {        
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastro([FromBody] LoginRequest request)
        {
            var email = request.Email?.Trim().ToLower();
            var senha = request.Senha?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            var usuarioExistente = await _context.Usuarios.AnyAsync(x => x.Email == email);
            if (usuarioExistente)
            {
                return Conflict("Usuario já cadastrado.");
            }

            var hashSenha = new PasswordHasher<Usuario>();
            var usuario = new Usuario
            {
                Email = email,
                Senha = string.Empty
            };

            usuario.Senha = hashSenha.HashPassword(usuario, senha);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { ok = true });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var email = request.Email?.Trim().ToLower();
            var senha = request.Senha?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
            if (usuario == null)
            {
                return Unauthorized("Usuário não encontrado");
            }

            var hashSenha = new PasswordHasher<Usuario>();            
            //// usuario.Senha = hashSenha.HashPassword(usuario, "123");
            //// Receber senha via console na api para testar SQL e hash senha - Fazer isso apenas para teste em DESENV.;
            ////Console.WriteLine($"Senha do usuário: {usuario.Senha}");
            var verificarsenha = hashSenha.VerifyHashedPassword(usuario, usuario.Senha, senha);
            if(verificarsenha.Equals(PasswordVerificationResult.Failed))
            {
                return Unauthorized("Senha incorreta");
            }
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), 
                        SecurityAlgorithms.HmacSha256Signature)

                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
        }

    }
}
