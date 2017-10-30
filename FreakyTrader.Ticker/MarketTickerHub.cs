using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace FreakyTrader.Ticker
{
    [HubName("MarketTicker")]
    public class MarketTickerHub : Hub
    {
        private readonly MarketTickerService _marketTickerHub;

        public MarketTickerHub(MarketTickerService marketTicker)
        {
            _marketTickerHub = marketTicker;
        }

        public string GetMarketState()
        {
            return _marketTickerHub.MarketState.ToString();
        }

        public bool OpenMarket(DateTime startDate, DateTime endDate, int interval)
        {
            return _marketTickerHub.OpenMarket(startDate, endDate, interval);
        }

        public bool CloseMarket()
        {
            return _marketTickerHub.CloseMarket();
        }

        public void Subscribe(List<string> stockCodes)
        {
            _marketTickerHub.Subscribe(Context.ConnectionId, stockCodes);
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            _marketTickerHub.UnSubscribe(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
    }
}
