using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FreakyTrader.Data.Context;
using FreakyTrader.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceProcess.Helpers;

namespace FreakyTrader.Ticker
{
    static class Program
    {
        static void Main()
        {
            var app = new AppBuilder().Build();

            app.Run();
        }

    }
}
