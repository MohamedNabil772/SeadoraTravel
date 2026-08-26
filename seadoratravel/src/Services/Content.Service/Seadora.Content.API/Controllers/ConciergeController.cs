using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.DTOs;
using Seadora.Content.Application.Concierge.Queries.ProcessChat;
using System.Threading.Tasks;

namespace Seadora.Content.API.Controllers
{
    [ApiController]
    [Route("api/concierge")]
    public class ConciergeController : ControllerBase
    {
        private readonly ISender _mediator;

        public ConciergeController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("chat")]
        [AllowAnonymous]
        public async Task<ActionResult<ConciergeChatResponseDto>> ProcessChat([FromBody] ConciergeChatRequestDto request)
        {
            var response = await _mediator.Send(new ProcessConciergeChatQuery(request));
            return Ok(response);
        }
    }
}
