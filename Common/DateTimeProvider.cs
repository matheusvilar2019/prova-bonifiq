using ProvaPub.Common.Interfaces;

namespace ProvaPub.Common
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
