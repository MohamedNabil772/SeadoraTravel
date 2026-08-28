using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Seadora.Support.Application.Commands;
using Seadora.Support.Application.Common.Interfaces;
using Seadora.Support.Domain.Entities;
using Seadora.Support.Domain.Enums;

namespace Seadora.UnitTests;

public class CreateCustomerTicketCommandTests
{
    [Fact]
    public async Task Handle_CreatesTicketAndInitialMessage()
    {
        // Arrange
        var contextMock = new Mock<ISupportDbContext>();
        var ticketsSet = new Mock<DbSet<Ticket>>();
        contextMock.Setup(c => c.Tickets).Returns(ticketsSet.Object);

        var handler = new CreateCustomerTicketCommandHandler(contextMock.Object);
        var command = new CreateCustomerTicketCommand(Guid.NewGuid(), Guid.NewGuid(), "Help", "Need help", null, "General");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        ticketsSet.Verify(x => x.Add(It.IsAny<Ticket>()), Times.Once);
        contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
