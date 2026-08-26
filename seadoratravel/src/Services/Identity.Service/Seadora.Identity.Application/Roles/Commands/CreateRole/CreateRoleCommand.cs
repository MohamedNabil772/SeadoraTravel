using MediatR;
using Microsoft.AspNetCore.Identity;
using Seadora.Identity.Domain.Entities;

namespace Seadora.Identity.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string Name) : IRequest<string>;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, string>
{
    private readonly RoleManager<Role> _roleManager;

    public CreateRoleCommandHandler(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<string> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await _roleManager.RoleExistsAsync(request.Name))
            throw new Exception("Role already exists");

        var role = new Role { Name = request.Name };
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);

        return role.Id;
    }
}
