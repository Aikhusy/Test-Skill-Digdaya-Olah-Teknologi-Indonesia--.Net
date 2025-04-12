using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _context.Users
                .Where(u => u.DeletedAt == null)
                .Select(u => new
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                status = 200,
                message = "OK",
                result = users
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = 500,
                message = "Internal Server Error: " + ex.Message,
                result = new object[] { }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _context.Users
                .Where(u => u.DeletedAt == null)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Ok(new
                {
                    status = 200,
                    message = "OK",
                    result = (object)null
                });
            }

            var response = new
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(new
            {
                status = 200,
                message = "OK",
                result = response
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = 500,
                message = "Internal Server Error: " + ex.Message,
                result = new object[] { }
            });
        }
    }
}
