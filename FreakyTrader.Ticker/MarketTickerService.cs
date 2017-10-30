using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using FreakyTrader.Data.DTO;
using FreakyTrader.Data.Repositories;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;

namespace FreakyTrader.Ticker
{
    public partial class MarketTickerService : ServiceBase
    {
        private Timer _timer;
        private int _iteration = 0;

        private readonly object _marketStateLock = new object();
        private volatile MarketState _marketState;

        private static readonly ConcurrentDictionary<string, List<CandleDTO>> StockCandles = new ConcurrentDictionary<string, List<CandleDTO>>();
        private static readonly ConcurrentDictionary<string, List<string>> ClientStockes = new ConcurrentDictionary<string, List<string>>();

        private readonly StockRepository _stockRepo;
        private readonly TransactionRepository _transactionRepository;

        private IHubConnectionContext<dynamic> Clients { get; set; }

        public MarketTickerService(StockRepository stockRepo, TransactionRepository transactionRepository)
        {
            _stockRepo = stockRepo;
            _transactionRepository = transactionRepository;

            InitializeComponent();

            Clients = GlobalHost.ConnectionManager.GetHubContext<MarketTickerHub>().Clients;
        }

        protected override void OnStart(string[] args)
        {
            WebApp.Start("http://localhost:8084");
        }

        protected override void OnStop()
        {

        }


        public MarketState MarketState
        {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public bool OpenMarket(DateTime startDate, DateTime endDate, int interval)
        {
            lock (_marketStateLock)
            {
                if (MarketState != MarketState.Open)
                {
                    LoadMarket(startDate, endDate, 10);

                    _timer = new Timer(UpdateMarketRates, null, interval, interval);

                    MarketState = MarketState.Open;

                    BroadcastMarketStateChange(MarketState.Open);
                }
            }
            return true;
        }

        private void LoadMarket(DateTime startDate, DateTime endDate, int period)
        {
            StockCandles.Clear();
            var stocks = _stockRepo.GetAllStocks();

            stocks.ForEach(s =>
            {
                var dataCandles = _transactionRepository.GetCandles(s.Code, startDate, endDate, period);
                var sparseCandles = new List<CandleDTO>();

                var tDate = startDate;
                while (tDate <= endDate)
                {
                    if (dataCandles.Count > 0)
                    {
                        var candle = dataCandles[0];
                        if (candle.RepDate == tDate)
                        {
                            sparseCandles.Add(candle);
                            dataCandles.RemoveAt(0);
                        }
                        else
                        {
                            if (tDate.DayOfWeek != DayOfWeek.Saturday && tDate.DayOfWeek != DayOfWeek.Sunday && (tDate.Hour >= 10 && tDate.Hour < 19))
                            {
                                sparseCandles.Add(null);
                            }
                        }
                    }
                    else
                    {
                        sparseCandles.Add(null);
                    }

                    tDate = tDate.AddSeconds(period);
                }

                StockCandles.TryAdd(s.Code, sparseCandles);
            });
        }


        public bool CloseMarket()
        {
            lock (_marketStateLock)
            {
                if (MarketState == MarketState.Open)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }

                    MarketState = MarketState.Closed;
                    _iteration = 0;

                    BroadcastMarketStateChange(MarketState.Closed);
                }
            }

            return true;
        }


        public void Subscribe(string connectionId, List<string> stockCodes)
        {
            ClientStockes.TryAdd(connectionId, stockCodes);
        }

        public void UnSubscribe(string connectionId)
        {
            List<string> stockCodes;
            ClientStockes.TryRemove(connectionId, out stockCodes);
        }


        private void UpdateMarketRates(object state)
        {
            foreach (var sc in StockCandles)
            {
                if (sc.Value.Count > _iteration)
                {
                    var value = sc.Value[_iteration];
                    if (value != null)
                    {
                        BroadcastStockRate(sc.Key, sc.Value[_iteration]);
                    }
                }
                else
                {
                    CloseMarket();
                    break;
                }
            }

            _iteration++;
        }


        private void BroadcastMarketStateChange(MarketState marketState)
        {
            switch (marketState)
            {
                case MarketState.Open:
                    Clients.All.marketStateChanged(marketState.ToString());
                    break;
                case MarketState.Closed:
                    Clients.All.marketStateChanged(marketState.ToString());
                    break;
                default:
                    break;
            }
        }

        private void BroadcastStockRate(string stockCode, CandleDTO candle)
        {
            foreach (var cs in ClientStockes)
            {
                if (cs.Value.Contains(stockCode))
                {
                    Clients.Client(cs.Key).updateStockRate(stockCode, candle);
                }
            }
        }

    }

    public enum MarketState
    {
        Closed,
        Open
    }
}
