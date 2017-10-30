using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreakyTrader.Business;
using FreakyTrader.Data.DTO;
using Microsoft.AspNet.SignalR.Client;

namespace FreakyTrader.Client
{
    public static class CommunicationHandler
    {
        public static string ExecuteMethod(string method, object[] args, string serverUri, string hubName)
        {
            var hubConnection = new HubConnection("http://localhost:8084");
            IHubProxy currencyExchangeHubProxy = hubConnection.CreateHubProxy("marketTicker");

            // Start the connection
            hubConnection.Start().Wait();

            var result = currencyExchangeHubProxy.Invoke<string>(method, args).Result;

            return result;
        }

        public static void Subscribe()
        {
            var hubConnection = new HubConnection("http://localhost:8084");
            IHubProxy currencyExchangeHubProxy = hubConnection.CreateHubProxy("marketTicker");
            // This line is necessary to subscribe for broadcasting messages
            currencyExchangeHubProxy.On<string, CandleDTO>("updateStockRate", HandleNotify);
            hubConnection.Start().Wait();
        }

        private static void HandleNotify(string stockCode, CandleDTO candle)
        {
            if (candle != null)
            {
                Console.WriteLine("Stock " + stockCode + ", High = " + candle.High + ", Time = " + candle.RepDate.ToString("HH:mm:ss:fff"));
            }
        }

        public static void SubscribeMarketStateChanged()
        {
            var hubConnection = new HubConnection("http://localhost:8084");
            IHubProxy currencyExchangeHubProxy = hubConnection.CreateHubProxy("marketTicker");
            // This line is necessary to subscribe for broadcasting messages
            currencyExchangeHubProxy.On<string>("marketStateChanged", HandleNotifyMarketStateChanged);
            hubConnection.Start().Wait();
        }

        private static void HandleNotifyMarketStateChanged(string state)
        {
            Console.WriteLine($"market - {state}");
        }
    }
}
