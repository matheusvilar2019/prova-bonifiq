using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Implementations
{
    public class PaypalPayment : IPayment
    {
        public string Method => "paypal";

        public void ProcessPayment(decimal paymentValue, int customerId)
        {
            //Faz pagamento...
        }
    }
}
