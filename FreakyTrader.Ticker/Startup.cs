using System;
using System.IO;
using FreakyTrader.Data.Context;
using FreakyTrader.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FreakyTrader.Ticker
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false);

            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration.GetSection("Logging")));
            services.AddLogging();

            services.AddDbContext<FreakyTraderContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("FreakyTraderConnection"), op => op.MaxBatchSize(30));
                options.EnableSensitiveDataLogging();
                options.UseLoggerFactory(new LoggerFactory());
            });

            services.AddTransient<StockRepository>();
            services.AddTransient<TransactionRepository>();
            services.AddSingleton<MarketTickerService>();
            services.AddTransient<App>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
