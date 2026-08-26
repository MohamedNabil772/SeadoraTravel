using System;

namespace Seadora.Common.Tenancy;

public static class SeadoraBranches
{
    // ponytail: single default branch until the Organization service exists (Phase 6).
    public static readonly Guid HeadOffice = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public const string HeadOfficeClaimValue = "00000000-0000-0000-0000-000000000001";
    public const string BranchClaimType = "branch";
}
