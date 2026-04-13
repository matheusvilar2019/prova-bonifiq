using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Implementations
{
    public class CreditCardPayment : IPayment
    {
        public string Method => "creditcard";

        public void ProcessPayment(decimal paymentValue, int customerId)
        {
            //Faz pagamento...
        }
    }
}
