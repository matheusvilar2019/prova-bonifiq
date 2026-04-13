using ProvaPub.Models;
using ProvaPub.Context;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services
{
    public class OrderService
	{
        TestDbContext _ctx;
        IEnumerable<IPayment> _payment;

        public OrderService(TestDbContext ctx, IEnumerable<IPayment> payment)
        {
            _ctx = ctx;
            _payment = payment;
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
            var payment = _payment
            .FirstOrDefault(p => p.Method == paymentMethod);

            if (payment == null)
                throw new InvalidOperationException("Método de pagamento indisponível");

            payment.ProcessPayment(paymentValue, customerId);

			return await InsertOrder(new Order() //Retorna o pedido para o controller
            {
                Value = paymentValue,
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
            });
		}

		public async Task<Order> InsertOrder(Order order)
        {
			//Insere pedido no banco de dados
            var entity = (await _ctx.Orders.AddAsync(order)).Entity;
            await _ctx.SaveChangesAsync();
            return entity;
        }
	}
}
