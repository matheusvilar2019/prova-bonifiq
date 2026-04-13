using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Implementations
{
    public class PixPayment : IPayment
    {
        public string Method => "pix";

        public void ProcessPayment(decimal paymentValue, int customerId)
        {
            //Faz pagamento...
        }
    }
}
