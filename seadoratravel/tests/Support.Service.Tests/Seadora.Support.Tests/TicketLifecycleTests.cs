using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Seadora.Support.Application.Commands;
using Seadora.Support.Domain.Enums;
using Seadora.Support.Infrastructure.Data;
using Seadora.Common.Messaging.Outbox;
using Moq;

namespace Seadora.Support.Tests;

public class TicketLifecycleTests
{
    private readonly SupportDbContext _dbContext;
    private readonly Mock<IOutboxWriter> _outboxMock;

    public TicketLifecycleTests()
    {
        var options = new DbContextOptionsBuilder<SupportDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new SupportDbContext(options);
        _outboxMock = new Mock<IOutboxWriter>();
    }

    [Fact]
    public async Task CreateTicket_ShouldSaveTicketAndPublishEvent()
    {
        // Arrange
        var handler = new CreateTicketCommandHandler(_dbContext, _outboxMock.Object);
        var cmd = new CreateTicketCommand("Help", "John", "john@test.com", null, TicketChannel.Email, TicketPriority.High, "I need help");

        // Act
        var ticketId = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        var ticket = await _dbContext.Tickets.Include(t => t.Messages).FirstOrDefaultAsync(t => t.Id == ticketId);
        Assert.NotNull(ticket);
        Assert.Equal("Help", ticket.Subject);
        Assert.Single(ticket.Messages);
        Assert.Equal("I need help", ticket.Messages[0].Body);
        
        _outboxMock.Verify(x => x.Enqueue(It.IsAny<Seadora.Contracts.Support.TicketCreated>()), Times.Once);
    }
}
