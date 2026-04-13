using Moq;
using ProvaPub.Common.Interfaces;
using ProvaPub.Models;
using ProvaPub.Services;
using ProvaPub.Tests.Helpers;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private Mock<IDateTimeProvider> CreateDateMock(DateTime date)
        {
            var mock = new Mock<IDateTimeProvider>();
            mock.Setup(x => x.UtcNow).Returns(date);
            return mock;
        }

        [Fact]
        public async Task Should_Throw_When_CustomerId_Invalid()
        {
            var ctx = DbContextFactory.CreateContext();
            var dateMock = CreateDateMock(DateTime.UtcNow);

            var svc = new CustomerService(ctx, dateMock.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                svc.CanPurchase(0, 10));
        }

        [Fact]
        public async Task Should_Throw_When_PurchaseValue_Invalid()
        {
            var ctx = DbContextFactory.CreateContext();
            var dateMock = CreateDateMock(DateTime.UtcNow);

            var svc = new CustomerService(ctx, dateMock.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                svc.CanPurchase(1, 0));
        }

        [Fact]
        public async Task Should_Throw_When_Customer_NotFound()
        {
            var ctx = DbContextFactory.CreateContext();
            var dateMock = CreateDateMock(DateTime.UtcNow);

            var svc = new CustomerService(ctx, dateMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.CanPurchase(1, 50));
        }

        [Fact]
        public async Task Should_ReturnFalse_When_AlreadyBoughtThisMonth()
        {
            var ctx = DbContextFactory.CreateContext();

            ctx.Customers.Add(new Customer { Id = 1, Name = "Fulano" });

            ctx.Orders.Add(new Order
            {
                CustomerId = 1,
                OrderDate = DateTime.UtcNow
            });

            ctx.SaveChanges();

            var dateMock = CreateDateMock(DateTime.UtcNow);

            var svc = new CustomerService(ctx, dateMock.Object);

            var result = await svc.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Theory]
        [InlineData(99.99, true)]
        [InlineData(100.00, true)]  
        [InlineData(100.01, false)]
        [InlineData(150.00, false)]
        public async Task Should_Validate_FirstPurchase_ValueLimit(decimal purchaseValue, bool expected)
        {
            var ctx = DbContextFactory.CreateContext();

            // Cliente sem histórico de compras
            ctx.Customers.Add(new Customer { Id = 1, Name = "Fulano" });
            ctx.SaveChanges();

            var dateMock = CreateDateMock(new DateTime(2026, 4, 13, 10, 0, 0));

            var svc = new CustomerService(ctx, dateMock.Object);

            var result = await svc.CanPurchase(1, purchaseValue);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(50, true)]
        [InlineData(100, true)]
        [InlineData(150, true)]  
        [InlineData(1000, true)] 
        public async Task Should_Allow_AnyValue_When_Customer_AlreadyBoughtBefore(decimal purchaseValue, bool expected)
        {
            var ctx = DbContextFactory.CreateContext();

            var now = new DateTime(2026, 4, 13, 10, 0, 0);

            var customer = new Customer { Id = 1, Name = "Fulano" };

            ctx.Customers.Add(customer);

            // Compra antiga (mais de 1 mês atrás)
            ctx.Orders.Add(new Order
            {
                CustomerId = 1,
                OrderDate = now.AddMonths(-2)
            });

            ctx.SaveChanges();

            var dateMock = CreateDateMock(now);

            var svc = new CustomerService(ctx, dateMock.Object);

            var result = await svc.CanPurchase(1, purchaseValue);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2026, 4, 13, 7, 59, false)] 
        [InlineData(2026, 4, 13, 8, 0, true)]   
        [InlineData(2026, 4, 13, 12, 0, true)]  
        [InlineData(2026, 4, 13, 18, 0, true)]  
        [InlineData(2026, 4, 13, 19, 0, false)] 
        [InlineData(2026, 4, 13, 22, 0, false)] 
        public async Task Should_Allow_Only_During_BusinessHours(int year, int month, int day, int hour, int minute, bool expected)
        {
            var ctx = DbContextFactory.CreateContext();

            ctx.Customers.Add(new Customer { Id = 1, Name = "Fulano" });
            ctx.SaveChanges();

            var date = new DateTime(year, month, day, hour, minute, 0);
            var dateMock = CreateDateMock(date);

            var svc = new CustomerService(ctx, dateMock.Object);

            var result = await svc.CanPurchase(1, 50);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2026, 4, 8, true)]   // quarta
        [InlineData(2026, 4, 9, true)]   // quinta
        [InlineData(2026, 4, 10, true)]  // sexta
        [InlineData(2026, 4, 11, false)] // sábado
        [InlineData(2026, 4, 12, false)] // domingo
        [InlineData(2026, 4, 13, true)]  // segunda
        [InlineData(2026, 4, 14, true)]  // terça
        public async Task Should_Validate_WorkingDays(int year, int month, int day, bool expected)
        {
            var ctx = DbContextFactory.CreateContext();

            ctx.Customers.Add(new Customer { Id = 1, Name = "Fulano" });
            ctx.SaveChanges();

            var date = new DateTime(year, month, day, 10, 0, 0);
            var dateMock = CreateDateMock(date);

            var svc = new CustomerService(ctx, dateMock.Object);

            var result = await svc.CanPurchase(1, 50);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_AllRulesPass()
        {
            var ctx = DbContextFactory.CreateContext();

            ctx.Customers.Add(new Customer { Id = 1, Name = "Fulano" });
            ctx.SaveChanges();

            var dateMock = CreateDateMock(new DateTime(2026, 4, 13, 10, 0, 0));

            var svc = new CustomerService(ctx, dateMock.Object);

            var result = await svc.CanPurchase(1, 50);

            Assert.True(result);
        }
        
    }
}
