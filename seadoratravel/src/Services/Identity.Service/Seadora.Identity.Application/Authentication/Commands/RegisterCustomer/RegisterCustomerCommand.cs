using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Seadora.Identity.Application.Common.Interfaces;
using Seadora.Identity.Domain.Entities;
using Seadora.Contracts.Identity;
using Seadora.Common.Messaging;
using Seadora.Identity.Application.Authentication.Commands.Register;

namespace Seadora.Identity.Application.Authentication.Commands.RegisterCustomer;

public record RegisterCustomerCommand(string FirstName, string LastName, string Email, string Password, string BranchId) : IRequest<AuthResponse>;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, AuthResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEventPublisher _eventPublisher;

    public RegisterCustomerCommandHandler(UserManager<User> userManager, IJwtTokenGenerator jwtTokenGenerator, IEventPublisher eventPublisher)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _eventPublisher = eventPublisher;
    }

    public async Task<AuthResponse> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var user = new User { UserName = request.Email, Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) throw new Exception("Registration failed");

        await _userManager.AddToRoleAsync(user, "Customer");
        var roles = new List<string> { "Customer" };
        // Wait, issue JWT with branchId ? Let's see IJwtTokenGenerator.
        var token = _jwtTokenGenerator.GenerateToken(user, roles, request.BranchId);
        await _eventPublisher.PublishAsync(new CustomerRegistered(user.Id, user.Email, user.FirstName, user.LastName, request.BranchId), cancellationToken);

        return new AuthResponse(token, user.Email!, roles);
    }
}
