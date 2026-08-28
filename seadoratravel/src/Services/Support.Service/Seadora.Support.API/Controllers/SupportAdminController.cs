using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Seadora.Support.API.Controllers;

[ApiController]
[Route("api/support-admin")]
[Authorize(Policy = "SupportPolicy")]
public class SupportAdminController : ControllerBase
{
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(new { Message = "Stats coming soon" });
    }
}
