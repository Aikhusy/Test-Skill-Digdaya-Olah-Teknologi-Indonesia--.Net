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

    // GET: api/trip
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var trips = await _context.Trips.Where(t => t.DeletedAt == null)
            .Include(t => t.Employee)
            .Include(t => t.AssignedBy)
            .Include(t => t.City)
            .Select(t => new TripGetResponse
            {
                Id = t.Id,
                EmployeeName = t.Employee.FullName, 
                AssignedByName = t.AssignedBy.FullName,
                CityName = t.City.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Purpose = t.Purpose
            })
            .ToListAsync();

        return Ok(new
        {
            status = 200,
            message = "Data trips berhasil diambil.",
            result = trips
        });
    }

    // GET: api/trip/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var trip = await _context.Trips.Where(t => t.DeletedAt == null)
            .Include(t => t.Employee)
            .Include(t => t.AssignedBy)
            .Include(t => t.City)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip == null)
            return NotFound(new { status = 404, message = "Trip tidak ditemukan." });

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

        return Ok(new
        {
            status = 200,
            message = "Trip berhasil ditemukan.",
            result = new[] { response }
        });
    }

    // POST: api/trip
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TripCreateRequest request)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound(new { status = 404, message = "User tidak ditemukan." });
            }

            var trip = new Trip
            {
                EmployeeId = request.EmployeeId,
                AssignedById = user.Id,
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

            return Ok(new
            {
                status = 200,
                message = "Berhasil",
                result = new[] { response }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = 500,
                message = "Internal Server Error: " + ex.Message
            });
        }
    }

    // PUT: api/trip/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TripUpdateRequest request)
    {
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
        {
            return NotFound(new { status = 404, message = "User tidak ditemukan." });
        }

        var trip = await _context.Trips.FindAsync(id);
        if (trip == null)
            return NotFound(new { status = 404, message = "Trip tidak ditemukan." });

        trip.EmployeeId = request.EmployeeId;
        trip.AssignedById = user.Id;
        trip.CityId = request.CityId;
        trip.StartDate = request.StartDate;
        trip.EndDate = request.EndDate;
        trip.Purpose = request.Purpose;
        trip.UpdatedAt = DateTime.UtcNow;

        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = $"Updated trip with ID: {id}.",
            CreatedAt = DateTime.UtcNow
        };
        _context.UserLogs.Add(userLog);

        await _context.SaveChangesAsync();

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

        return Ok(new
        {
            status = 200,
            message = "Trip berhasil diperbarui.",
            result = new[] { response }
        });
    }

    // DELETE: api/trip/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip == null)
            return NotFound(new { status = 404, message = "Trip tidak ditemukan." });

        trip.DeletedAt = DateTime.UtcNow;
        trip.UpdatedAt = DateTime.UtcNow;

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        var userLog = new UserLog
        {
            UserId = user.Id,
            LogMessage = $"Soft deleted trip with ID: {id}.",
            CreatedAt = DateTime.UtcNow
        };
        _context.UserLogs.Add(userLog);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            status = 200,
            message = "Trip berhasil dihapus (soft delete)."
        });
    }
}