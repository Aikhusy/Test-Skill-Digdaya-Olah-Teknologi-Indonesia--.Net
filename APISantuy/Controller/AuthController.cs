// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
        if (user == null)
            return Unauthorized("Email tidak ditemukan.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            var failedLog = new UserLog
            {
                UserId = user.Id,
                LogMessage = "Login attempt failed: Incorrect password.",
                CreatedAt = DateTime.UtcNow
            };
            _context.UserLogs.Add(failedLog);
            _context.SaveChanges();

            return Unauthorized("Password salah.");
        }

            


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = "User successfully logged in.",
            CreatedAt = DateTime.UtcNow
        };
        
        _context.UserLogs.Add(userLog);
        _context.SaveChanges();

        return Ok(new LoginResponse
        {
            Token = jwt,
            Role = user.Role
        });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;

        if (email == null)
            return Unauthorized("Token tidak valid.");

        var user = _context.Users.SingleOrDefault(u => u.Email == email);

        if (user == null)
            return NotFound("Pengguna tidak ditemukan.");

        var response = new ProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // Untuk logout menggunakan JWT, tidak ada perubahan di server, cukup beri respon sukses.
        // Biasanya di sisi klien yang akan menghapus token yang tersimpan di localStorage/sessionStorage.

        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = _context.Users.SingleOrDefault(u => u.Email == email);

        if (user == null)
            return NotFound("Pengguna tidak ditemukan.");

        // Jika Anda ingin mencatat log/logout yang berhasil, bisa ditambahkan di sini.
        var logoutLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = "User logged out successfully.",
            CreatedAt = DateTime.UtcNow
        };

        _context.UserLogs.Add(logoutLog);
        _context.SaveChanges();

        // Menyediakan pesan logout
        return Ok(new { message = "Berhasil logout." });
    }


}
