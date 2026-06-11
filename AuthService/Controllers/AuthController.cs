using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDb _db;
        private readonly JwtService _jwt;
        private readonly IConfiguration _config;

        public AuthController(AuthDb db, JwtService jwt, IConfiguration config)
        {
            _db = db;
            _jwt = jwt;
            _config = config;
        }

        // ==========================
        // LOGIN
        // ==========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.usuario) ||
                string.IsNullOrWhiteSpace(request.contrasena) ||
                string.IsNullOrWhiteSpace(request.tipousuario))
            {
                return BadRequest(new { mensaje = "Datos requeridos no pueden ser vacíos" });
            }

            using var conn = _db.CreateConnection();

            var user = await conn.QueryFirstOrDefaultAsync<dynamic>(
                @"SELECT * FROM UsersAuth 
                  WHERE Email = @email AND UserType = @type",
                new { email = request.usuario, type = request.tipousuario }
            );

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(request.contrasena, (string)user.PasswordHash))
            {
                return Unauthorized(new { mensaje = "Usuario y/o contraseña incorrectos" });
            }

            //JWT
            var (token, exp) = _jwt.GenerateToken((int)user.Id);

            // Refresh Token
            var refreshToken = Guid.NewGuid().ToString();

            var refreshExp = DateTime.UtcNow.AddMinutes(
                _config.GetValue<int>("Defaults:RefreshTokenMinutes")
            );

            await conn.ExecuteAsync(@"
                INSERT INTO RefreshTokens (UserId, Token, Expiration)
                VALUES (@UserId, @Token, @Exp)",
                new
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    Exp = refreshExp
                });

            var response = new TokenResponse
            {
                expires_in = exp,
                access_token = token,
                refresh_token = refreshToken,
                usuarioID = user.Id
            };

            return StatusCode(201, response);
        }

        // ==========================
        // REFRESH
        // ==========================
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.refresh_token))
                return BadRequest(new { mensaje = "Debe enviar refresh_token" });

            using var conn = _db.CreateConnection();

            var tokenData = await conn.QueryFirstOrDefaultAsync<dynamic>(
                @"SELECT * FROM RefreshTokens 
                  WHERE Token = @token AND IsRevoked = 0",
                new { token = request.refresh_token }
            );

            if (tokenData == null || tokenData.Expiration < DateTime.UtcNow)
                return Unauthorized(new { mensaje = "No autorizado" });

            // Invalidar token usado
            await conn.ExecuteAsync(
                "UPDATE RefreshTokens SET IsRevoked = 1 WHERE Id = @id",
                new { id = tokenData.Id }
            );

            // Generar nuevo JWT
            var (newAccessToken, exp) = _jwt.GenerateToken((int)tokenData.UserId);

            // Nuevo refresh token
            var newRefreshToken = Guid.NewGuid().ToString();

            var refreshExp = DateTime.UtcNow.AddMinutes(
                _config.GetValue<int>("Defaults:RefreshTokenMinutes")
            );

            await conn.ExecuteAsync(@"
                INSERT INTO RefreshTokens (UserId, Token, Expiration)
                VALUES (@UserId, @Token, @Exp)",
                new
                {
                    UserId = tokenData.UserId,
                    Token = newRefreshToken,
                    Exp = refreshExp
                });

            return StatusCode(201, new
            {
                expires_in = exp,
                access_token = newAccessToken,
                refresh_token = newRefreshToken
            });
        }

        // ==========================
        // VALIDATE TOKEN
        // ==========================
        [HttpGet("validate")]
        public IActionResult Validate([FromQuery] string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                // Intentar leer desde Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrWhiteSpace(authHeader) &&
                    authHeader.StartsWith("Bearer "))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(token, _jwt.GetValidationParameters(), out _);
                return Ok(true);
            }
            catch
            {
                return Unauthorized();
            }
        }

        // ==========================
        // CREATE USER
        // ==========================
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.UserType))
            {
                return BadRequest(new { mensaje = "Datos inválidos" });
            }

            using var conn = _db.CreateConnection();

            //  BCrypt
            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var sql = @"
                INSERT INTO UsersAuth (Email, PasswordHash, UserType, IsActive)
                VALUES (@Email, @Hash, @UserType, 1)
            ";

            await conn.ExecuteAsync(sql, new
            {
                Email = request.Email,
                Hash = hash,
                UserType = request.UserType
            });

            return Ok(new { mensaje = "Usuario creado correctamente" });
        }
    }
}