using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
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
            .Select(t => new TripGetResponse
            {
                Id = t.Id,
                EmployeeName = t.Employee.FullName, // atau t.Employee.Name, sesuaikan field-nya
                AssignedByName = t.AssignedBy.FullName,
                CityName = t.City.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Purpose = t.Purpose
            })
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

        var response = new TripGetResponse
        {
            Id = trip.Id,
            EmployeeName = trip.Employee.FullName,
            AssignedByName = trip.AssignedBy.FullName,
            CityName = trip.City.Name,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Purpose = trip.Purpose
        };

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TripCreateRequest request)
    {
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
        {
            return Unauthorized();
        }

        var trip = new Trip
        {
            EmployeeId = request.EmployeeId,
            AssignedById = request.AssignedById,
            CityId = request.CityId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Purpose = request.Purpose,
            CreatedAt = DateTime.UtcNow
        };

        _context.Trips.Add(trip);

        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = "Created a new trip.",
            CreatedAt = DateTime.UtcNow
        };

        _context.UserLogs.Add(userLog);
        await _context.SaveChangesAsync();

        // Mapping dari entitas ke response DTO
        var response = new TripCreateResponse
        {
            Id = trip.Id,
            EmployeeId = trip.EmployeeId,
            AssignedById = trip.AssignedById,
            CityId = trip.CityId,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Purpose = trip.Purpose
        };

        return Ok(response);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TripUpdateRequest request)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip == null)
            return NotFound();

        // Update properti Trip dari DTO
        trip.EmployeeId = request.EmployeeId;
        trip.AssignedById = request.AssignedById;
        trip.CityId = request.CityId;
        trip.StartDate = request.StartDate;
        trip.EndDate = request.EndDate;
        trip.Purpose = request.Purpose;
        trip.UpdatedAt = DateTime.UtcNow;

        // Ambil user dari token JWT
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null)
            return Unauthorized();

        // Logging
        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = $"Updated trip with ID: {id}.",
            CreatedAt = DateTime.UtcNow
        };
        _context.UserLogs.Add(userLog);

        await _context.SaveChangesAsync();

        // Mapping ke DTO response
        var response = new TripUpdateResponse
        {
            Id = trip.Id,
            EmployeeId = trip.EmployeeId,
            AssignedById = trip.AssignedById,
            CityId = trip.CityId,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Purpose = trip.Purpose,
        };

        return Ok(response);
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
