using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


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
        try
        {
            var cities = await _context.Cities
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(new
            {
                status = 200,
                message = "OK",
                result = cities
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
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
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
                Id = city.Id,
                Name = city.Name
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
