using System.Globalization;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Finance.Application.Reports;

namespace Seadora.Finance.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Policy = "Finance.ViewReports")]
public class ReportsController : ControllerBase
{
    private readonly ISender _mediator;
    public ReportsController(ISender mediator) => _mediator = mediator;

    private static ReportFilter BuildFilter(DateTime? from, DateTime? to, Guid? branchId, string? currency,
        int page = 1, int pageSize = 50) => new(from, to, branchId, currency, page, pageSize);

    [HttpGet("general-ledger")]
    public async Task<IActionResult> GeneralLedger([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency, [FromQuery] Guid? accountId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => Ok(await _mediator.Send(new GeneralLedgerQuery(BuildFilter(from, to, branchId, currency, page, pageSize), accountId)));

    [HttpGet("trial-balance")]
    public async Task<IActionResult> TrialBalance([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency)
        => Ok(await _mediator.Send(new TrialBalanceQuery(BuildFilter(from, to, branchId, currency))));

    [HttpGet("profit-and-loss")]
    public async Task<IActionResult> ProfitAndLoss([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency)
        => Ok(await _mediator.Send(new ProfitAndLossQuery(BuildFilter(from, to, branchId, currency))));

    [HttpGet("revenue")]
    public async Task<IActionResult> Revenue([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency)
        => Ok(await _mediator.Send(new RevenueReportQuery(BuildFilter(from, to, branchId, currency))));

    [HttpGet("ar-aging")]
    public async Task<IActionResult> ArAging([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency, [FromQuery] DateTime? asOf)
        => Ok(await _mediator.Send(new ArAgingQuery(BuildFilter(from, to, branchId, currency), asOf)));

    [HttpGet("supplier-payables")]
    public async Task<IActionResult> SupplierPayables([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency)
        => Ok(await _mediator.Send(new SupplierPayablesQuery(BuildFilter(from, to, branchId, currency))));

    [HttpGet("receipts")]
    public async Task<IActionResult> Receipts([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => Ok(await _mediator.Send(new ReceiptsQuery(BuildFilter(from, to, branchId, currency, page, pageSize))));

    [HttpGet("refunds")]
    public async Task<IActionResult> Refunds([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency)
        => Ok(await _mediator.Send(new RefundsReportQuery(BuildFilter(from, to, branchId, currency))));

    [HttpGet("tax")]
    public async Task<IActionResult> Tax([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency)
        => Ok(await _mediator.Send(new TaxReportQuery(BuildFilter(from, to, branchId, currency))));

    // ---- CSV export (Finance.Export) ----
    [HttpGet("export/{report}")]
    [Authorize(Policy = "Finance.Export")]
    public async Task<IActionResult> Export(string report, [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] Guid? branchId, [FromQuery] string? currency)
    {
        var f = BuildFilter(from, to, branchId, currency, 1, 500);
        string csv;
        try
        {
            csv = report.ToLowerInvariant() switch
            {
            "trial-balance" => ToCsv(await _mediator.Send(new TrialBalanceQuery(f)),
                new[] { "AccountCode", "AccountName", "AccountType", "TotalDebit", "TotalCredit", "Balance" },
                r => new object?[] { r.AccountCode, r.AccountName, r.AccountType, r.TotalDebit, r.TotalCredit, r.Balance }),
            "ar-aging" => ToCsv((await _mediator.Send(new ArAgingQuery(f))).Items,
                new[] { "BookingId", "BranchId", "Currency", "Due", "AgeDays", "Bucket", "BookingDateUtc" },
                r => new object?[] { r.BookingId, r.BranchId, r.Currency, r.Due, r.AgeDays, r.Bucket, r.BookingDateUtc }),
            "receipts" => ToCsv((await _mediator.Send(new ReceiptsQuery(f))).Items,
                new[] { "Id", "BookingId", "BranchId", "Amount", "Currency", "Method", "Reference", "ReceivedUtc", "Reconciled" },
                r => new object?[] { r.Id, r.BookingId, r.BranchId, r.Amount, r.Currency, r.Method, r.Reference, r.ReceivedUtc, r.Reconciled }),
            "supplier-payables" => ToCsv(await _mediator.Send(new SupplierPayablesQuery(f)),
                new[] { "SupplierId", "BranchId", "PeriodStart", "PeriodEnd", "Accrued", "Paid", "Due", "Status", "Currency" },
                r => new object?[] { r.SupplierId, r.BranchId, r.PeriodStart, r.PeriodEnd, r.Accrued, r.Paid, r.Due, r.Status, r.Currency }),
            "general-ledger" => ToCsv((await _mediator.Send(new GeneralLedgerQuery(f))).Items,
                new[] { "EntryId", "OccurredUtc", "Description", "BranchId", "BookingId", "AccountCode", "AccountName", "Debit", "Credit", "Currency" },
                r => new object?[] { r.EntryId, r.OccurredUtc, r.Description, r.BranchId, r.BookingId, r.AccountCode, r.AccountName, r.Debit, r.Credit, r.Currency }),
                _ => throw new KeyNotFoundException($"Unknown report '{report}'")
            };
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"{report}.csv");
    }

    private static string ToCsv<T>(IReadOnlyList<T> rows, string[] headers, Func<T, object?[]> project)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", headers));
        foreach (var row in rows)
            sb.AppendLine(string.Join(",", project(row).Select(Field)));
        return sb.ToString();

        static string Field(object? v)
        {
            var s = v switch
            {
                null => "",
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => v.ToString() ?? ""
            };
            return s.Contains(',') || s.Contains('"') || s.Contains('\n')
                ? "\"" + s.Replace("\"", "\"\"") + "\""
                : s;
        }
    }
}
