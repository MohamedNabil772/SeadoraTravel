using MediatR;
using Microsoft.AspNetCore.Identity;
using Seadora.Identity.Domain.Entities;
using System.Linq;

namespace Seadora.Identity.Application.Roles.Queries.GetRoles;

public record RoleDto(string Id, string Name);

public record GetRolesQuery : IRequest<List<RoleDto>>;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly RoleManager<Role> _roleManager;

    public GetRolesQueryHandler(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = _roleManager.Roles
            .Select(r => new RoleDto(r.Id, r.Name!))
            .ToList();
            
        return Task.FromResult(roles);
    }
}
