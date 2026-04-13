using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class PagedResultService<T>
    {
        TestDbContext _ctx;

        public PagedResultService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public PagedResultList<Product> ListProducts(int page)
        {
            int pageSize = 10;
            int totalCount = _ctx.Products.Count();
            bool hasNext = page * pageSize < totalCount;

            List<Product> products = _ctx.Products
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultList<Product>() { HasNext = hasNext, TotalCount = totalCount, Items = products };
        }

    }
}
