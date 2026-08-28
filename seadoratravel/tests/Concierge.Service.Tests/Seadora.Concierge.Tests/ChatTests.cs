using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Seadora.Concierge.Application.Commands;
using Seadora.Concierge.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Seadora.Concierge.Tests;

public class ChatTests
{
    private DbContextOptions<ConciergeDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ConciergeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task ProcessChatCommand_CreatesSessionAndReturnsResponse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var dbContext = new ConciergeDbContext(options))
        {
            var handler = new ProcessChatCommandHandler(dbContext);
            var sessionId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var command = new ProcessChatCommand
            {
                SessionId = sessionId,
                BranchId = branchId,
                Message = "Hello"
            };

            // Act
            var response = await handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Content.Should().Be("How can I help you today?");
            
            var session = await dbContext.ConversationSessions.Include(s => s.Messages).FirstOrDefaultAsync(s => s.Id == sessionId);
            session.Should().NotBeNull();
            session!.Messages.Count.Should().Be(2); // User message and Bot message
            session.Messages.First().Role.Should().Be("User");
            session.Messages.Last().Role.Should().Be("Assistant");
        }
    }

    [Fact]
    public async Task ProcessChatCommand_TourSearchIntent_ReturnsSuggestedTours()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var dbContext = new ConciergeDbContext(options))
        {
            var branchId = Guid.NewGuid();
            dbContext.TourCatalogIndices.Add(new Domain.Entities.TourCatalogIndex
            {
                TourId = Guid.NewGuid(),
                BranchId = branchId,
                Title = "Rome Tour",
                IsActive = true,
                PriceEur = 100
            });
            await dbContext.SaveChangesAsync();

            var handler = new ProcessChatCommandHandler(dbContext);
            var sessionId = Guid.NewGuid();
            var command = new ProcessChatCommand
            {
                SessionId = sessionId,
                BranchId = branchId,
                Message = "search for tours"
            };

            // Act
            var response = await handler.Handle(command, CancellationToken.None);

            // Assert
            response.Intent.Should().Be("TourSearch");
            response.SuggestedTours.Should().HaveCount(1);
            response.SuggestedTours.First().Title.Should().Be("Rome Tour");
        }
    }
}
