using Microsoft.EntityFrameworkCore;
using ProvaPub.Context;

namespace ProvaPub.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static TestDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TestDbContext(options);
        }
    }
}
