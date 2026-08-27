using Microsoft.EntityFrameworkCore;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Customers.Commands.AddCustomerDocument;
using Seadora.Customer.Application.Customers.Commands.CreateCustomer;
using Seadora.Customer.Application.Customers.Commands.UpdateMarketingConsent;
using Seadora.Customer.Application.Customers.Queries.GetCustomers;
using Seadora.Customer.Infrastructure.Persistence;

namespace Seadora.UnitTests;

public class CustomerTests
{
    private static readonly Guid BranchA = Guid.Parse("00000000-0000-0000-0000-0000000000aa");
    private static readonly Guid BranchB = Guid.Parse("00000000-0000-0000-0000-0000000000bb");

    private sealed class StubBranch : ICurrentBranch
    {
        public StubBranch(Guid branchId) => BranchId = branchId;
        public Guid BranchId { get; }
    }

    private static CustomerDbContext NewContext() =>
        new(new DbContextOptionsBuilder<CustomerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task CreateCustomer_StampsBranch_NormalizesEmail_AndSetsCreatedUtc()
    {
        using var db = NewContext();
        var handler = new CreateCustomerCommandHandler(db, new StubBranch(BranchA));

        var id = await handler.Handle(new CreateCustomerCommand("  Ada Lovelace ", "  Ada@Example.COM "), default);

        var saved = await db.Customers.SingleAsync(c => c.Id == id);
        Assert.Equal(BranchA, saved.BranchId);
        Assert.Equal("ada@example.com", saved.Email);
        Assert.Equal("Ada Lovelace", saved.FullName);
        Assert.NotEqual(default, saved.CreatedUtc);
        Assert.Null(saved.ConsentUpdatedUtc);
    }

    [Fact]
    public async Task CreateCustomer_DuplicateEmail_Throws_InSameBranch_ButAllowedInAnother()
    {
        using var db = NewContext();

        await new CreateCustomerCommandHandler(db, new StubBranch(BranchA))
            .Handle(new CreateCustomerCommand("Ada", "ada@example.com"), default);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new CreateCustomerCommandHandler(db, new StubBranch(BranchA))
                .Handle(new CreateCustomerCommand("Ada Again", "ADA@example.com"), default));

        // InMemory does not enforce the unique index, so this exercises the handler's AnyAsync guard.
        var otherBranchId = await new CreateCustomerCommandHandler(db, new StubBranch(BranchB))
            .Handle(new CreateCustomerCommand("Ada", "ada@example.com"), default);

        Assert.Equal(2, await db.Customers.CountAsync());
        Assert.Equal(BranchB, (await db.Customers.SingleAsync(c => c.Id == otherBranchId)).BranchId);
    }

    [Fact]
    public async Task UpdateMarketingConsent_SetsConsentAndTimestamp()
    {
        using var db = NewContext();
        var branch = new StubBranch(BranchA);
        var id = await new CreateCustomerCommandHandler(db, branch)
            .Handle(new CreateCustomerCommand("Ada", "ada@example.com"), default);

        await new UpdateMarketingConsentCommandHandler(db, branch)
            .Handle(new UpdateMarketingConsentCommand(id, true), default);

        var saved = await db.Customers.SingleAsync(c => c.Id == id);
        Assert.True(saved.MarketingConsent);
        Assert.NotNull(saved.ConsentUpdatedUtc);
    }

    [Fact]
    public async Task GetCustomers_ReturnsOnlyCurrentBranch()
    {
        using var db = NewContext();
        await new CreateCustomerCommandHandler(db, new StubBranch(BranchA))
            .Handle(new CreateCustomerCommand("Ada", "ada@example.com"), default);
        await new CreateCustomerCommandHandler(db, new StubBranch(BranchB))
            .Handle(new CreateCustomerCommand("Grace", "grace@example.com"), default);

        var page = await new GetCustomersQueryHandler(db, new StubBranch(BranchA))
            .Handle(new GetCustomersQuery(), default);

        Assert.Equal(1, page.TotalCount);
        Assert.Equal("ada@example.com", Assert.Single(page.Items).Email);
    }

    [Fact]
    public async Task AddCustomerDocument_IsBranchGated()
    {
        using var db = NewContext();
        var foreignId = await new CreateCustomerCommandHandler(db, new StubBranch(BranchB))
            .Handle(new CreateCustomerCommand("Grace", "grace@example.com"), default);

        var branchA = new StubBranch(BranchA);
        var ownId = await new CreateCustomerCommandHandler(db, branchA)
            .Handle(new CreateCustomerCommand("Ada", "ada@example.com"), default);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new AddCustomerDocumentCommandHandler(db, branchA)
                .Handle(new AddCustomerDocumentCommand(foreignId, "Passport", "fs://a", "a.pdf"), default));

        await new AddCustomerDocumentCommandHandler(db, branchA)
            .Handle(new AddCustomerDocumentCommand(ownId, "Passport", "fs://b", "b.pdf", DateTime.UtcNow.AddYears(1)), default);

        var doc = Assert.Single(await db.CustomerDocuments.Where(d => d.CustomerId == ownId).ToListAsync());
        Assert.Equal("fs://b", doc.FileRef);
        Assert.NotNull(doc.RetentionUntilUtc);
    }
}
