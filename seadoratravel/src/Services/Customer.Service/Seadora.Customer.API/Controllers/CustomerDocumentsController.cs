using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Customer.Application.Customers.Commands.AddCustomerDocument;
using Seadora.Customer.Application.Customers.Queries.GetCustomerById;

namespace Seadora.Customer.API.Controllers;

// ponytail: branch isolation lives in the handlers' query filter, not this controller - a customer
// outside the caller's branch simply doesn't load, so these endpoints 404 without any extra check.
[ApiController]
[Route("api/customers/{customerId:guid}/documents")]
public class CustomerDocumentsController : ControllerBase
{
    private readonly ISender _mediator;

    public CustomerDocumentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    public record AddDocumentRequest(string DocumentType, string FileRef, string FileName, DateTime? RetentionUntilUtc);

    [HttpPost]
    public async Task<IActionResult> Add(Guid customerId, [FromBody] AddDocumentRequest request)
    {
        try
        {
            var id = await _mediator.Send(new AddCustomerDocumentCommand(
                customerId, request.DocumentType, request.FileRef, request.FileName, request.RetentionUntilUtc));
            return Ok(id);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid customerId)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(customerId));
        return customer is null
            ? NotFound(new { error = $"Customer {customerId} was not found." })
            : Ok(customer.Documents);
    }
}
