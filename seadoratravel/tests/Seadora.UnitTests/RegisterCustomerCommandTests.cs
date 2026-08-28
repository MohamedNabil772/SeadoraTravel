using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Seadora.Identity.Application.Authentication.Commands.RegisterCustomer;
using Seadora.Identity.Application.Common.Interfaces;
using Seadora.Contracts.Messaging;
using Seadora.Identity.Domain.Entities;
using Seadora.Contracts.Identity;

namespace Seadora.UnitTests;

public class RegisterCustomerCommandTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesUserAndPublishesEvent()
    {
        // Arrange
        var userStore = new Mock<IUserStore<User>>();
        var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
        
        userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
            
        userManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Customer"))
            .ReturnsAsync(IdentityResult.Success);

        var jwtGen = new Mock<IJwtTokenGenerator>();
        jwtGen.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>(), It.IsAny<string>()))
            .Returns("fake_token");

        var eventPublisher = new Mock<IEventPublisher>();

        var handler = new RegisterCustomerCommandHandler(userManager.Object, jwtGen.Object, eventPublisher.Object);
        var command = new RegisterCustomerCommand("John", "Doe", "john@example.com", "Pass123!", "branch1");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("fake_token", result.Token);
        eventPublisher.Verify(x => x.PublishAsync(It.IsAny<CustomerRegistered>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
