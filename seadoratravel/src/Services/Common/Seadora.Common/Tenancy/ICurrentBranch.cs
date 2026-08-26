using System;

namespace Seadora.Common.Tenancy;

public interface ICurrentBranch
{
    Guid BranchId { get; }
}
