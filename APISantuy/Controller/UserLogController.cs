using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserLogController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserLogController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var logs = await _context.UserLogs
                .Include(l => l.User)
                .Select(l => new
                {
                    Id = l.Id,
                    UserName = l.User.FullName,
                    UserEmail = l.User.Email,
                    LogMessage = l.LogMessage,
                    CreatedAt = l.CreatedAt
                })
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                status = 200,
                message = "OK",
                result = logs
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

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var log = await _context.UserLogs
                .Include(l => l.User)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .FirstOrDefaultAsync();

            if (log == null)
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
                Id = log.Id,
                UserName = log.User.FullName,
                UserEmail = log.User.Email,
                LogMessage = log.LogMessage,
                CreatedAt = log.CreatedAt
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