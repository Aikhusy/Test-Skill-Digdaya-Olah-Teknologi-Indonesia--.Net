using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CityController : ControllerBase
{
    private readonly AppDbContext _context;

    public CityController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cities = await _context.Cities
            .Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

        return Ok(cities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var city = await _context.Cities
            .FirstOrDefaultAsync(c => c.Id == id);

        if (city == null)
            return NotFound();

        var response = new
        {
            Id = city.Id,
            Name = city.Name
        };

        return Ok(response);
    }
}