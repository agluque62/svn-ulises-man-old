using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;

namespace TestingOnConsole.RxNetInAction
{
    class StockTick
    {
        public string QuoteSymbol { get; set; }
        public decimal Price { get; set; }
        //other properties
    }
    class StockTicker
    {
        public event EventHandler<StockTick> StockTick;
    }

    class StockInfo
    {
        public StockInfo(string symbol, decimal price)
        {
            Symbol = symbol;
            PrevPrice = price;
        }
        public string Symbol { get; set; }
        public decimal PrevPrice { get; set; }
    }

    class StockMonitor
    {
        const decimal maxChangeRatio = 0.1m;
        public StockMonitor(StockTicker ticker)
        {
            ticker.StockTick += OnStockTick;
            _ticker = ticker;
        }
        void OnStockTick(object sender, StockTick stockTick)
        {
            StockInfo stockInfo;
            var quoteSymbol = stockTick.QuoteSymbol;
            lock (_stockTickLocker)
            {

                var stockInfoExists = _stockInfos.TryGetValue(quoteSymbol, out stockInfo);
                if (stockInfoExists)
                {
                    var priceDiff = stockTick.Price - stockInfo.PrevPrice;
                    var changeRatio = Math.Abs(priceDiff / stockInfo.PrevPrice);
                    if (changeRatio > maxChangeRatio)
                    {
                        //Do something with the stock – notify users or display on screen
                        Console.WriteLine($"Stock:{quoteSymbol} has changed with {changeRatio} ratio, Old Price:{stockInfo.PrevPrice} New Price:{stockTick.Price} ");
                    }
                    _stockInfos[quoteSymbol].PrevPrice = stockTick.Price;
                }
                else
                {
                    _stockInfos[quoteSymbol] = new StockInfo(quoteSymbol, stockTick.Price);
                }
            }
        }
        public void Dispose()
        {
            _ticker.StockTick -= OnStockTick;
            _stockInfos.Clear();
        }
        private readonly StockTicker _ticker;
        Dictionary<string, StockInfo> _stockInfos = new Dictionary<string, StockInfo>();
        object _stockTickLocker = new object();
        
        //rest of the code
    }

    class RxStockMonitor
    {
        public RxStockMonitor(StockTicker ticker)
        {
        }
    }
    class RxNetInAction
    {
    }
}
