using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentAgreementsController : ControllerBase
{
    private readonly IContentDbContext _context;

    public PaymentAgreementsController(IContentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentAgreement>>> Get()
    {
        var agreements = await _context.PaymentAgreements.ToListAsync();
        return Ok(agreements);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentAgreement>> GetById(Guid id)
    {
        var agreement = await _context.PaymentAgreements.FindAsync(id);
        if (agreement == null) return NotFound();
        return Ok(agreement);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentAgreement agreement)
    {
        if (agreement.Id == Guid.Empty)
        {
            agreement.Id = Guid.NewGuid();
        }
        _context.PaymentAgreements.Add(agreement);
        await _context.SaveChangesAsync(default);
        return CreatedAtAction(nameof(GetById), new { id = agreement.Id }, agreement);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PaymentAgreement agreement)
    {
        if (id != agreement.Id) return BadRequest();

        var existing = await _context.PaymentAgreements.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = agreement.Name;

        await _context.SaveChangesAsync(default);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _context.PaymentAgreements.FindAsync(id);
        if (existing == null) return NotFound();

        _context.PaymentAgreements.Remove(existing);
        await _context.SaveChangesAsync(default);
        return NoContent();
    }
}
