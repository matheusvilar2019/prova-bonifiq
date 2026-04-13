using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Context;
using ProvaPub.Services.Interfaces;
using ProvaPub.Common.Interfaces;

namespace ProvaPub.Services
{
    public class CustomerService : ICustomerService
    {
        TestDbContext _ctx;
        IDateTimeProvider _dateTime;

        public CustomerService(TestDbContext ctx, IDateTimeProvider dateTime)
        {
            _ctx = ctx;
            _dateTime = dateTime;
        }

        public PagedResultList<Customer> ListCustomers(int page)
        {
            int pageSize = 10;
            int totalCount = _ctx.Customers.Count();
            bool hasNext = page * pageSize < totalCount;

            List<Customer> customers = _ctx.Customers
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultList<Customer>() { HasNext = hasNext, TotalCount = totalCount, Items = customers };
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var baseDate = _dateTime.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            if (_dateTime.UtcNow.Hour < 8 || _dateTime.UtcNow.Hour > 18 || _dateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday || _dateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }

    }
}
