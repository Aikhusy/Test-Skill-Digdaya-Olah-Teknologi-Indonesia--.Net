using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
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
        {
            return Ok(new
            {
                status = 401,
                message = "Email tidak ditemukan.",
                result = new object[] { }
            });
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        var logMessage = isPasswordValid ? "User successfully logged in." : "Login attempt failed: Incorrect password.";

        _context.UserLogs.Add(new UserLog
        {
            UserId = user.Id,
            LogMessage = logMessage,
            CreatedAt = DateTime.UtcNow
        });
        _context.SaveChanges();

        if (!isPasswordValid)
        {
            return Ok(new
            {
                status = 401,
                message = "Password salah.",
                result = new object[] { }
            });
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

        return Ok(new
        {
            status = 200,
            message = "OK",
            result = new object[] { new { token = jwt, role = user.Role } }
        });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        if (email == null)
        {
            return Ok(new
            {
                status = 401,
                message = "Token tidak valid.",
                result = new object[] { }
            });
        }

        var user = _context.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
        {
            return Ok(new
            {
                status = 404,
                message = "Pengguna tidak ditemukan.",
                result = new object[] { }
            });
        }

        return Ok(new
        {
            status = 200,
            message = "OK",
            result = new object[] { new 
            { 
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            }}
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = _context.Users.SingleOrDefault(u => u.Email == email);

        if (user == null)
        {
            return Ok(new
            {
                status = 404,
                message = "Pengguna tidak ditemukan.",
                result = new object[] { }
            });
        }

        _context.UserLogs.Add(new UserLog
        {
            UserId = user.Id,
            LogMessage = "User logged out successfully.",
            CreatedAt = DateTime.UtcNow
        });
        _context.SaveChanges();

        return Ok(new
        {
            status = 200,
            message = "OK",
            result = new object[] { "Berhasil logout." }
        });
    }
}
