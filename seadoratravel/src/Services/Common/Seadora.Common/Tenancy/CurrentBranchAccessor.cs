using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Seadora.Common.Tenancy;

public sealed class CurrentBranchAccessor(IHttpContextAccessor accessor) : ICurrentBranch
{
    public Guid BranchId
    {
        get
        {
            var value = accessor.HttpContext?.User?.FindFirst(SeadoraBranches.BranchClaimType)?.Value;
            return Guid.TryParse(value, out var branchId) ? branchId : SeadoraBranches.HeadOffice;
        }
    }
}
