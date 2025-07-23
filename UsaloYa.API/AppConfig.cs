
namespace UsaloYa.API.Config
{
    public class AppConfig
    {
        public int MaxPendingPaymentDaysAllowAccess { get;}
        public int FreeRoleMaxCustomers { get; set; }
        public int FreeRoleMaxProducts { get; set; }

        public string FreeRoleMaxLimitReachedMsg { get; set; }

        public readonly static string NO_AUTORIZADO = "Petición incorrecta.";

        public AppConfig(IConfiguration configuration)
        {
            var settings = configuration.GetSection("AppSettings");

            if (!int.TryParse(settings.GetValue<string>("MaxPendingPaymentDaysAllowAccess"), out var maxInactiveCompanyDays))
                maxInactiveCompanyDays = 5;

            MaxPendingPaymentDaysAllowAccess = maxInactiveCompanyDays;

            if (!int.TryParse(settings.GetValue<string>("FreeRoleMaxCustomers"), out var maxFreeRoleCustomers))
                maxFreeRoleCustomers = 5;
            FreeRoleMaxCustomers = maxFreeRoleCustomers;

            if (!int.TryParse(settings.GetValue<string>("FreeRoleMaxProducts"), out var maxFreeRoleProducts))
                maxFreeRoleProducts = 20;
            FreeRoleMaxProducts = maxFreeRoleProducts;

            var freeRoleLimitReachedMsg = settings.GetValue<string>("FreeRoleMaxLimitReachedMsg");
            FreeRoleMaxLimitReachedMsg = string.IsNullOrEmpty(freeRoleLimitReachedMsg) ? "Actualiza a Premium" : freeRoleLimitReachedMsg;

        }

       
    }
}
