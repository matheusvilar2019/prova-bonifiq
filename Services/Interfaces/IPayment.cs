namespace ProvaPub.Services.Interfaces
{
    public interface IPayment
    {
        string Method { get; }
        void ProcessPayment(decimal paymentValue, int customerId);
    }
}
