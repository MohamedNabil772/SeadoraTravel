using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Categories.Commands;
using Seadora.Content.Application.Categories.Queries;
using Seadora.Content.Application.DTOs;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize(Policy = "AdminOnly")]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> Get() => Ok(await mediator.Send(new GetCategoriesQuery()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id) => Ok(await mediator.Send(new GetCategoryByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command) => Ok(new { id = await mediator.Send(command) });

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id) return BadRequest("Mismatched category ID.");
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ReorderCategoriesCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}
