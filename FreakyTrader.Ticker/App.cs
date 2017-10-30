using System;
using System.Collections.Generic;
using System.ServiceProcess;
using ServiceProcess.Helpers;

namespace FreakyTrader.Ticker
{
    public class App
    {
        private readonly MarketTickerService _marketTickerService;

        public App(MarketTickerService marketTickerService)
        {
            _marketTickerService = marketTickerService;
        }

        public void Run()
        {
            var servicesToRun = new List<ServiceBase>()
            {
                _marketTickerService
            };

            if (Environment.UserInteractive)
            {
                servicesToRun.ToArray().LoadServices();
            }
            else
            {
                ServiceBase.Run(servicesToRun.ToArray());
            }
        }
    }
}
