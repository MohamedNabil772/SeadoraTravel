using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.API.Controllers;
using Seadora.Content.Domain.Entities;
using Seadora.Content.Infrastructure.Persistence;
using Seadora.Contracts.Enums;

namespace Seadora.UnitTests;

// ponytail: the InMemory provider cannot map the jsonb dictionary/list properties, so the test
// context drops them. Only the scalar policy columns matter here.
internal sealed class InMemoryContentDbContext : ContentDbContext
{
    public InMemoryContentDbContext(DbContextOptions<ContentDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entityTypes = typeof(Seadora.Content.Application.Common.Interfaces.IContentDbContext)
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .ToHashSet();

        foreach (var clrType in entityTypes)
        {
            foreach (var property in clrType.GetProperties())
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (type.IsValueType || type == typeof(string)) continue;
                // keep navigations to mapped entities, drop everything else (jsonb payloads)
                if (entityTypes.Contains(type) || type.GetGenericArguments().Any(entityTypes.Contains)) continue;
                modelBuilder.Entity(clrType).Ignore(property.Name);
            }
        }
    }
}

public class TourTypePolicyTests
{
    private static ContentDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<ContentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InMemoryContentDbContext(options);
    }

    private static TourTypesController NewController(ContentDbContext context) =>
        new(context)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

    private static async Task<TourType> SeedAsync(ContentDbContext context)
    {
        var entity = new TourType { Code = "GROUP" };
        context.TourTypes.Add(entity);
        await context.SaveChangesAsync(default);
        return entity;
    }

    [Fact]
    public async Task Update_persists_all_policy_fields()
    {
        using var context = NewContext();
        var entity = await SeedAsync(context);

        var result = await NewController(context).Update(entity.Id, new UpdateTourTypeRequest
        {
            AllocationModel = AllocationModel.WholeUnit,
            DefaultMinCapacity = 2,
            DefaultMaxCapacity = 12,
            RequiresGuestDetails = true,
            RequiresPassport = true,
            PayLaterAllowed = false
        });

        Assert.IsType<OkObjectResult>(result);

        var saved = await context.TourTypes.AsNoTracking().SingleAsync(t => t.Id == entity.Id);
        Assert.Equal(AllocationModel.WholeUnit, saved.AllocationModel);
        Assert.Equal(2, saved.DefaultMinCapacity);
        Assert.Equal(12, saved.DefaultMaxCapacity);
        Assert.True(saved.RequiresGuestDetails);
        Assert.True(saved.RequiresPassport);
        Assert.False(saved.PayLaterAllowed);
    }

    [Fact]
    public async Task Update_rejects_max_capacity_below_min()
    {
        using var context = NewContext();
        var entity = await SeedAsync(context);

        var result = await NewController(context).Update(entity.Id, new UpdateTourTypeRequest
        {
            DefaultMinCapacity = 10,
            DefaultMaxCapacity = 4
        });

        Assert.IsType<BadRequestObjectResult>(result);

        var saved = await context.TourTypes.AsNoTracking().SingleAsync(t => t.Id == entity.Id);
        Assert.Null(saved.DefaultMinCapacity);
        Assert.Null(saved.DefaultMaxCapacity);
    }
}
