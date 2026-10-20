using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OliveOilApi.Data;
using OliveOilApi.Models;

namespace OliveOilApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OliveOilProductsController(AppDbContext db) : ControllerBase
{
    // GET api/OliveOilProducts
    [HttpGet]
    public async Task<ActionResult<List<OliveOilProduct>>> GetAll()
        => await db.OliveOilProducts.ToListAsync();

    [HttpGet("search")]
    public async Task<ActionResult<List<OliveOilProduct>>> Search([FromQuery] string term)
        => await db.OliveOilProducts
                   .Where(p => p.Name.Contains(term))
                   .ToListAsync();

    [HttpGet("search-vulnerable")]
    public async Task<ActionResult<List<OliveOilProduct>>> SearchVulnerable([FromQuery] string term)
    {
        var sql = "SELECT Id, Name, Region, PricePerLiter, QuantityInStock " +
                  "FROM OliveOilProducts " +
                  "WHERE Name LIKE '%" + term + "%'";

        return await db.OliveOilProducts.FromSqlRaw(sql).ToListAsync();
    }

    [HttpGet("search-secure")]
    public async Task<ActionResult<List<OliveOilProduct>>> SearchSecure([FromQuery] string term)
    {
        var pattern = "%" + term + "%";

        return await db.OliveOilProducts
            .FromSqlInterpolated($@"SELECT Id, Name, Region, PricePerLiter, QuantityInStock
                                    FROM OliveOilProducts
                                    WHERE Name LIKE {pattern}")
            .ToListAsync();
    }
}
