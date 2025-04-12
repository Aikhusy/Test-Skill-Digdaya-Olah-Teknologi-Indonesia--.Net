using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var log = await _context.UserLogs
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (log == null)
            return NotFound();

        var response = new
        {
            Id = log.Id,
            UserName = log.User.FullName,
            UserEmail = log.User.Email,
            LogMessage = log.LogMessage,
            CreatedAt = log.CreatedAt
        };

        return Ok(response);
    }
}