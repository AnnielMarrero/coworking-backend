using coworking.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace coworking.Test.Helpers;

public class CoworkingDbMock : IDbContextFactory<CoworkingManager>
{
    public CoworkingManager CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CoworkingManager>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new CoworkingManager(options);
    }
}
