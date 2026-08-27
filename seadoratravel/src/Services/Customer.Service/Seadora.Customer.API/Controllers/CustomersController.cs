using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Customer.Application.Customers.Commands.CreateCustomer;
using Seadora.Customer.Application.Customers.Commands.UpdateCustomer;
using Seadora.Customer.Application.Customers.Commands.UpdateMarketingConsent;
using Seadora.Customer.Application.Customers.Queries.GetCustomerById;
using Seadora.Customer.Application.Customers.Queries.GetCustomers;

namespace Seadora.Customer.API.Controllers;

// ponytail: branch isolation lives in the handlers' query filter (every load is
// `... && BranchId == currentBranch.BranchId`), not here - the controller stays thin.
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ISender _mediator;

    public CustomersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        try
        {
            return Ok(await _mediator.Send(command));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetCustomersQuery(search, pageNumber, pageSize)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(id));
        return customer is null ? NotFound(new { error = $"Customer {id} was not found." }) : Ok(customer);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
    {
        if (id != command.Id) return BadRequest(new { error = "Mismatched customer ID." });
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}/consent")]
    public async Task<IActionResult> UpdateConsent(Guid id, [FromBody] UpdateMarketingConsentCommand command)
    {
        if (id != command.Id) return BadRequest(new { error = "Mismatched customer ID." });
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
