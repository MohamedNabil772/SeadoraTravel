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
public class SuppliersController : ControllerBase
{
    private readonly IContentDbContext _context;

    public SuppliersController(IContentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> Get()
    {
        var suppliers = await _context.Suppliers
            .Include(s => s.PaymentAgreement)
            .ToListAsync();
        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Supplier>> GetById(Guid id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.PaymentAgreement)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return NotFound();
        return Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Supplier supplier)
    {
        if (supplier.Id == Guid.Empty)
        {
            supplier.Id = Guid.NewGuid();
        }
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(default);
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Supplier supplier)
    {
        if (id != supplier.Id) return BadRequest();

        var existing = await _context.Suppliers.FindAsync(id);
        if (existing == null) return NotFound();

        existing.NameAr = supplier.NameAr;
        existing.NameEn = supplier.NameEn;
        existing.BankAccountInfo = supplier.BankAccountInfo;
        existing.PaymentAgreementId = supplier.PaymentAgreementId;

        await _context.SaveChangesAsync(default);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _context.Suppliers.FindAsync(id);
        if (existing == null) return NotFound();

        _context.Suppliers.Remove(existing);
        await _context.SaveChangesAsync(default);
        return NoContent();
    }
}
