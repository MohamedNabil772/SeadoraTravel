using System;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Seadora.Common.Tenancy;
using Xunit;

namespace Seadora.Common.Tests.Tenancy;

public class CurrentBranchAccessorTests
{
    private static IHttpContextAccessor AccessorWith(ClaimsPrincipal? user)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        if (user is null)
        {
            accessor.SetupGet(a => a.HttpContext).Returns((HttpContext?)null);
        }
        else
        {
            var context = new Mock<HttpContext>();
            context.SetupGet(c => c.User).Returns(user);
            accessor.SetupGet(a => a.HttpContext).Returns(context.Object);
        }

        return accessor.Object;
    }

    private static ClaimsPrincipal PrincipalWith(params Claim[] claims)
        => new(new ClaimsIdentity(claims, "TestAuth"));

    [Fact]
    public void BranchId_ReturnsClaimValue_WhenBranchClaimIsValidGuid()
    {
        var expected = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var sut = new CurrentBranchAccessor(AccessorWith(
            PrincipalWith(new Claim(SeadoraBranches.BranchClaimType, expected.ToString()))));

        sut.BranchId.Should().Be(expected);
    }

    [Fact]
    public void BranchId_ReturnsHeadOffice_WhenBranchClaimMissing()
    {
        var sut = new CurrentBranchAccessor(AccessorWith(
            PrincipalWith(new Claim(ClaimTypes.Email, "someone@seadora.test"))));

        sut.BranchId.Should().Be(SeadoraBranches.HeadOffice);
    }

    [Fact]
    public void BranchId_ReturnsHeadOffice_WhenHttpContextIsNull()
    {
        var sut = new CurrentBranchAccessor(AccessorWith(null));

        sut.BranchId.Should().Be(SeadoraBranches.HeadOffice);
    }

    [Fact]
    public void BranchId_ReturnsHeadOffice_WhenBranchClaimIsNotAGuid()
    {
        var sut = new CurrentBranchAccessor(AccessorWith(
            PrincipalWith(new Claim(SeadoraBranches.BranchClaimType, "not-a-guid"))));

        sut.BranchId.Should().Be(SeadoraBranches.HeadOffice);
    }
}
