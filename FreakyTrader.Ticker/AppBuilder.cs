using Microsoft.Extensions.DependencyInjection;

namespace FreakyTrader.Ticker
{
    public class AppBuilder
    {
        public App Build()
        {
            var serviceCollection = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(serviceCollection);

            return Startup.ServiceProvider.GetService<App>();
        }
    }
}
