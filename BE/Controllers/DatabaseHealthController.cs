using System.Threading.Tasks;
using BE.Constants;
using BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseHealthController : ControllerBase
{
    private readonly DatabaseHealthService _databaseHealthService;

    public DatabaseHealthController(DatabaseHealthService databaseHealthService)
    {
        _databaseHealthService = databaseHealthService;
    }

    [HttpGet("version")]
    public async Task<IActionResult> GetVersionAsync()
    {
        var version = await _databaseHealthService.GetDatabaseVersionAsync();
        if (string.IsNullOrWhiteSpace(version))
        {
            return StatusCode(503, new { Message = MessageConstants.DatabaseVersionUnavailable });
        }

        return Ok(new { Version = version });
    }
}
