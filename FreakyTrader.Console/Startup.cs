using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreakyTrader.Business.Helpers;
using FreakyTrader.Data.Context;
using FreakyTrader.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FreakyTrader.Console
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false);

            Configuration = builder.Build();
        }

        IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration.GetSection("Logging"))
                .AddDebug());
            services.AddLogging();

            services.AddDbContext<FreakyTraderContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("FreakyTraderConnection"), op => op.MaxBatchSize(30));
                options.EnableSensitiveDataLogging();
                options.UseLoggerFactory(new LoggerFactory());
            });

            services.AddTransient<StockLoader>();

            services.AddTransient<StockRepository>();
            services.AddTransient<TransactionRepository>();

            services.AddTransient<App>();
        }

    }
}
