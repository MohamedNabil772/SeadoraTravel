using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Seadora.Contracts.Identity;
using Seadora.Customer.Application.Consumers;
using Seadora.Customer.Application.Common.Interfaces;
using Seadora.Customer.Infrastructure.Persistence;

namespace Seadora.UnitTests;

public class CustomerRegisteredConsumerTests
{
    private CustomerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CustomerDbContext(options);
    }

    [Fact]
    public async Task Consume_NewCustomer_CreatesCustomer()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var consumer = new CustomerRegisteredConsumer(context);
        
        var msg = new CustomerRegistered(Guid.NewGuid().ToString(), "test@test.com", "John", "Doe", Guid.NewGuid().ToString());
        var consumeContext = new Mock<ConsumeContext<CustomerRegistered>>();
        consumeContext.Setup(c => c.Message).Returns(msg);

        // Act
        await consumer.Consume(consumeContext.Object);

        // Assert
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id.ToString() == msg.UserId);
        Assert.NotNull(customer);
        Assert.Equal("John Doe", customer.FullName);
        Assert.Equal("test@test.com", customer.Email); // Should be normalized but fine for basic assert
    }
}
