using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FreakyTrader.Console
{
    public class AppBuilder
    {
        public App Build()
        {
            var serviceCollection = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(serviceCollection);
            return serviceCollection.BuildServiceProvider().GetService<App>();
        }
    }
}
