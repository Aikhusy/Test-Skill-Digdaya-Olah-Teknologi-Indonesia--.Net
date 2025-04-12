using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class TripController : ControllerBase
{
    private readonly AppDbContext _context;

    public TripController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var trips = await _context.Trips
            .Include(t => t.Employee)
            .Include(t => t.AssignedBy)
            .Include(t => t.City)
            .ToListAsync();

        return Ok(trips);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var trip = await _context.Trips
            .Include(t => t.Employee)
            .Include(t => t.AssignedBy)
            .Include(t => t.City)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Trip request)
    {
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        request.CreatedAt = DateTime.UtcNow;
        _context.Trips.Add(request);

        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = "Created a new trip.",
            CreatedAt = DateTime.UtcNow
        };
        _context.UserLogs.Add(userLog);

        await _context.SaveChangesAsync();
        return Ok(request);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Trip request)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip == null)
            return NotFound();

        trip.EmployeeId = request.EmployeeId;
        trip.AssignedById = request.AssignedById;
        trip.CityId = request.CityId;
        trip.StartDate = request.StartDate;
        trip.EndDate = request.EndDate;
        trip.Purpose = request.Purpose;
        trip.UpdatedAt = DateTime.UtcNow;

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = $"Updated trip with ID: {id}.",
            CreatedAt = DateTime.UtcNow
        };
        _context.UserLogs.Add(userLog);

        await _context.SaveChangesAsync();
        return Ok(trip);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip == null)
            return NotFound();

        _context.Trips.Remove(trip);

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = $"Deleted trip with ID: {id}.",
            CreatedAt = DateTime.UtcNow
        };
        _context.UserLogs.Add(userLog);

        await _context.SaveChangesAsync();
        return Ok(new { message = "Trip berhasil dihapus." });
    }
}
