using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Finance.Application.Dashboard;

namespace Seadora.Finance.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Policy = "Finance.ViewDashboard")]
public class DashboardController : ControllerBase
{
    private readonly ISender _mediator;
    public DashboardController(ISender mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency, [FromQuery] string granularity = "day")
        => Ok(await _mediator.Send(new DashboardQuery(from, to, branchId, currency, granularity)));
}
