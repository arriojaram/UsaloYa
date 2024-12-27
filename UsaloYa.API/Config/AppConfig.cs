
namespace UsaloYa.API.Config
{
    public class AppConfig
    {
        public int MaxPendingPaymentDaysAllowAccess { get;}

        public AppConfig(IConfiguration configuration)
        {
            var settings = configuration.GetSection("AppSettings");

            if (!int.TryParse(settings.GetValue<string>("MaxPendingPaymentDaysAllowAccess"), out var maxInactiveCompanyDays))
                maxInactiveCompanyDays = 5;

            MaxPendingPaymentDaysAllowAccess = maxInactiveCompanyDays;
        }

       
    }
}
