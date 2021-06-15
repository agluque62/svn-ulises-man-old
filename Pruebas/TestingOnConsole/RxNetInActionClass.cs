using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace TestingOnConsole.RxNetInAction
{
    public static class MyExtensions
    {
        public static IDisposable SubscribeConsole<T>(this IObservable<T> observable, string name = "")
        {
            return observable.Subscribe(new ConsoleObserver<T>(name));
        }
        public static IObservable<T> Log<T>(this IObservable<T> observable, string msg = "")
        {
            return observable.Do(
            x => Console.WriteLine("{0} - OnNext({1})", msg, x),
            ex =>
            {
                Console.WriteLine("{0} - OnError:", msg);
                Console.WriteLine("\t {0}", ex);
            },
            () => Console.WriteLine("{0} - OnCompleted()", msg));
        }
    }
    public class ConsoleObserver<T> : IObserver<T>
    {
        private readonly string _name;
        public ConsoleObserver(string name = "")
        {
            _name = name;
        }
        public void OnNext(T value)
        {
            Console.WriteLine("{0} - OnNext({1})", _name, value);
        }
        public void OnError(Exception error)
        {
            Console.WriteLine("{0} - OnError:", _name);
            Console.WriteLine("\t {0}", error);
        }
        public void OnCompleted()
        {
            Console.WriteLine("{0} - OnCompleted()", _name);
        }
    }
    class StockTick
    {
        public string QuoteSymbol { get; set; }
        public decimal Price { get; set; }
        //other properties
        public override string ToString()
        {
            return $"{QuoteSymbol}: ${Price}";
        }
    }
    class StockTicker : IDisposable
    {
        public event EventHandler<StockTick> StockTick;

        public StockTicker()
        {
        }

        public void Run()
        {
            Obs = Sequence.ToObservable();
            Subscription = Obs.Subscribe((a) =>
            {
                Console.WriteLine($"Tick => {a}");
                StockTick?.Invoke(this, a);
                var delay = RandomGenerator.Next(1, 5);
                Task.Delay(TimeSpan.FromSeconds(delay)).Wait();
            },
            () => Console.WriteLine("Fin de secuencia")
            );
        }

        public void Dispose()
        {
            Subscription?.Dispose();
        }

        IObservable<StockTick> Obs { get; set; }
        IDisposable Subscription { get; set; }

        List<StockTick> Sequence = new List<StockTick>()
        {
            new StockTick(){ QuoteSymbol="MSFT", Price=27.01M},
            new StockTick(){ QuoteSymbol="INTC", Price=21.75M},
            new StockTick(){ QuoteSymbol="MSFT", Price=27.96M},
            new StockTick(){ QuoteSymbol="MSFT", Price=31.21M},
            new StockTick(){ QuoteSymbol="INTC", Price=22.54M},
            new StockTick(){ QuoteSymbol="INTC", Price=20.98M},
            new StockTick(){ QuoteSymbol="MSFT", Price=30.73M}
        };

        Random RandomGenerator = new Random((int)DateTime.Now.Ticks);
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

        public override string ToString()
        {
            return $"{Symbol}: ${PrevPrice}";
        }
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
    class RxStockMonitor : IDisposable
    {
        class DrasticChange
        {
            public string Symbol { get; set; }
            public decimal ChangeRatio { get; set; }
            public decimal OldPrice { get; set; }
            public decimal NewPrice { get; set; }
        }
        public RxStockMonitor(StockTicker ticker)
        {
            var ticks = Observable.FromEventPattern<EventHandler<StockTick>, StockTick>(
                h => ticker.StockTick += h,
                h => ticker.StockTick -= h)
                .Select(tickEvent => tickEvent.EventArgs)
                .Synchronize();
            var drasticChanges =
             from tick in ticks
             group tick by tick.QuoteSymbol
             into company
             from tickPair in company.Buffer(2, 1)
             let changeRatio = Math.Abs((tickPair[1].Price - tickPair[0].Price) / tickPair[0].Price)
             where changeRatio > maxChangeRatio
             select new DrasticChange()
             {
                 Symbol = company.Key,
                 ChangeRatio = changeRatio,
                 OldPrice = tickPair[0].Price,
                 NewPrice = tickPair[1].Price
             };

            _subscription = drasticChanges.Subscribe((change) =>
            {
                Console.WriteLine($"Stock:{change.Symbol} has changed with {change.ChangeRatio } ratio, Old Price: {change.OldPrice} New Price: {change.NewPrice}");
            });
        }
        const decimal maxChangeRatio = 0.1M;
        private IDisposable _subscription;
        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
    class RxNetInActionClass
    {
        // Async method
        static async Task<bool> IsEventAsync(int number)
        {
            return await Task.Run(() =>
            {
                Console.WriteLine($"Waiting on {number}");
                Task.Delay(250).Wait();
                Console.WriteLine($"Testing {number}");
                return number % 2 == 0;
            });
        }
        static async Task<bool> IsPrimeAsync(int number)
        {
            return await Task.Run(() =>
            {
                //Console.WriteLine($"Testing {number}");
                //Task.Delay(100).Wait();
                if (number == 1) return false;
                if (number == 2) return true;

                var limit = Math.Ceiling(Math.Sqrt(number)); //hoisting the loop limit

                for (int i = 2; i <= limit; ++i)
                    if (number % i == 0)
                        return false;
                return true;
            });
        }
        static void Test()
        {
            using (var ticker = new StockTicker())
            using (var monitor = new RxStockMonitor(ticker))
            {
                ticker.Run();
            }
        }
        static void TestingObservablesFromAsync()
        {
            int InitialNumber = 10000;
            int NumbersCount = 100;
            //int workerThreads;
            //int portThreads;
            //System.Threading.ThreadPool.GetMinThreads(out workerThreads, out portThreads);
            //System.Threading.ThreadPool.SetMinThreads(32, 32);

            //var subscription = Observable.Range(1, 20)
            //    .SelectMany((number) => IsEven(number), (number, isEven) => new { number, isEven })
            //    .Where(x => x.isEven)
            //    .Select(x => x.number)
            //    .Subscribe(number => Console.WriteLine($"{number} es par..."), () => Console.WriteLine("Fin de la Secuencia"));
            Console.WriteLine($"Getting Primes from {InitialNumber} to {InitialNumber + NumbersCount} Unordered.");
            IObservable<int> primes =
             from number in Observable.Range(InitialNumber, NumbersCount)
             from isPrime in IsPrimeAsync(number)
             where isPrime
             select number;

            primes.Subscribe(number => Console.WriteLine($"{number} es primo..."), () => Console.WriteLine("Fin de la Secuencia"));
            Console.ReadLine();

            Console.WriteLine($"Getting Primes from {InitialNumber} to {InitialNumber + NumbersCount} Ordered.");
            IObservable<int> primes1 =
             Observable.Range(InitialNumber, NumbersCount)
             .Select(async (number) => new
             {
                 number,
                 IsPrime = await IsPrimeAsync(number)
             })
             .Concat()
             .Where(x => x.IsPrime)
             .Select(x => x.number);
            primes1.Subscribe(number => Console.WriteLine($"{number} es primo..."), () => Console.WriteLine("Fin de la Secuencia"));

            //System.Threading.ThreadPool.SetMinThreads(workerThreads, portThreads);

        }
        static void TestingObservableCancelation()
        {
            //var cts = new CancellationTokenSource();
            //cts.Token.Register(() => Console.WriteLine("Subscription canceled"));
            var sun = Observable.Interval(TimeSpan.FromSeconds(1))
                .Timestamp()
                .Log("Logging")
                .SubscribeConsole("Final");
            //.Subscribe(x => Console.WriteLine(x), cts.Token);
            //cts.CancelAfter(TimeSpan.FromSeconds(5));
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();
            sun.Dispose();
        }
        static void TestingBasicSubjects()
        {
            Subject<string> sbj = new Subject<string>();

            Observable.Interval(TimeSpan.FromSeconds(1))
             .Select(x => "First: " + x)
             .Take(5)
             .Subscribe(sbj);
            Observable.Interval(TimeSpan.FromSeconds(2))
             .Select(x => "Second: " + x)
             .Take(5)
             .Subscribe(sbj);

            sbj.SubscribeConsole();
        }

        static void TestingCold2HotObservables()
        {
            //var coldObservable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            //var connectableObservable = coldObservable.Publish();

            //connectableObservable.SubscribeConsole("First");
            //connectableObservable.SubscribeConsole("Second");
            //connectableObservable.Connect();
            //Thread.Sleep(2000);
            //connectableObservable.SubscribeConsole("Third");

            //int I = 0;
            //var numbers = Observable.Range(1, 5).Select(_ => I++);
            //var zipped = numbers
            // .Zip(numbers, (a, b) => a + b)
            // .SubscribeConsole("zipped");
            //var publishedZip = numbers.Publish(published =>
            // published.Zip(published, (a, b) => a + b));
            //publishedZip.SubscribeConsole("publishedZipped");

            var publishedObservable = Observable.Interval(TimeSpan.FromSeconds(1))
             .Do(x => Console.WriteLine("Generating {0}", x))
             .Publish()
             .RefCount();
            var subscription1 = publishedObservable.SubscribeConsole("First");
            var subscription2 = publishedObservable.SubscribeConsole("Second");
            Thread.Sleep(3000);
            subscription1.Dispose();
            Thread.Sleep(3000);
            subscription2.Dispose();

            Thread.Sleep(3000);
            subscription1 = publishedObservable.SubscribeConsole("Tirth");
            Thread.Sleep(3000);
            subscription1.Dispose();
        }


        public static void CurrentTest()
        {
            //TestingBasicSubjects();
            TestingCold2HotObservables();
        }
    }
}
