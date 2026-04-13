using ProvaPub.Dtos;
using ProvaPub.Models;

namespace ProvaPub.Mappers
{
    public class OrderMapper
    {
        public static OrderDto ToDto(Order order)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

            return new OrderDto
            {
                Id = order.Id,
                Value = order.Value,
                CustomerId = order.CustomerId,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, tz),
                Customer = order.Customer
            };
        }
    }
}
