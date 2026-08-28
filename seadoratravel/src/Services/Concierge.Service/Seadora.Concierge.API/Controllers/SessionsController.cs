using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Concierge.Application.Commands;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Seadora.Concierge.API.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly IConciergeDbContext _dbContext;

    public SessionsController(IConciergeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSession(Guid id)
    {
        var session = await _dbContext.ConversationSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
            return NotFound();

        return Ok(session);
    }
}
