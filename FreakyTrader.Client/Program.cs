using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreakyTrader.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            CommunicationHandler.Subscribe();
            CommunicationHandler.SubscribeMarketStateChanged();

            var code = CommunicationHandler.ExecuteMethod("CloseMarket", new object[] { }, IPAddress.Any.ToString(), "MarketTicker");

            var state = CommunicationHandler.ExecuteMethod("GetMarketState", new object[] { }, IPAddress.Any.ToString(), "MarketTicker");
            Console.WriteLine("Market State is " + state);

            if (state == "Closed")
            {
                var returnCode = CommunicationHandler.ExecuteMethod("OpenMarket", new object[] { new DateTime(2017, 3, 30, 10, 0, 0), new DateTime(2017, 3, 30, 11, 0, 0), 100 }, IPAddress.Any.ToString(), "MarketTicker");
                Debug.Assert(returnCode == "True");
                Console.WriteLine("Market State is Open");
            }


            //Thread.Sleep(10000);

            //var code = CommunicationHandler.ExecuteMethod("CloseMarket", new object[] { }, IPAddress.Any.ToString(), "MarketTickerHub");

            //Console.WriteLine(code);

            //var state1 = CommunicationHandler.ExecuteMethod("GetMarketState", new object[] { }, IPAddress.Any.ToString(), "MarketTickerHub");
            //Console.WriteLine("Market State is " + state1);

            //if (state1 == "Closed")
            //{
            //    var returnCode = CommunicationHandler.ExecuteMethod("OpenMarket", new object[] { new DateTime(2017, 3, 30, 10, 0, 0), new DateTime(2017, 3, 30, 19, 0, 0), 1000 }, IPAddress.Any.ToString(), "MarketTickerHub");
            //    Debug.Assert(returnCode == "True");
            //    Console.WriteLine("Market State is Open");

            //}

            Console.ReadLine();
        }
    }
}
