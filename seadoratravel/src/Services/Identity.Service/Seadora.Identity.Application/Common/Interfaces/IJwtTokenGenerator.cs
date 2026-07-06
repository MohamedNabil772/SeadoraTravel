using System.Collections.Generic;
using Seadora.Identity.Domain.Entities;

namespace Seadora.Identity.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IList<string> roles);
}
