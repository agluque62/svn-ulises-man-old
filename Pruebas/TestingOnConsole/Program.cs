using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocket4Net;
using TestingOnConsole.RxNetInAction;

namespace TestingOnConsole
{
    class Program
    {
        static void TestBkk()
        {
            Console.WriteLine($"Hello world!");
            var pbxUrl = $"ws://10.12.60.36:8080/pbx/ws?login_user=sa&login_password=sa&user=*&registered=True&line=*";
            var PbxWebSocket = new WebSocket(pbxUrl);
            var PbxObservableOpen = Observable.FromEventPattern(h => PbxWebSocket.Opened += h, h => PbxWebSocket.Opened -= h)
                .Select(_ => (0, $""));
            var PbxObservableClose = Observable.FromEventPattern(h => PbxWebSocket.Closed += h, h => PbxWebSocket.Closed -= h)
                .Select(_ => (1, $""));
            var PbxObservableError = Observable.FromEventPattern<SuperSocket.ClientEngine.ErrorEventArgs>(h => PbxWebSocket.Error += h, h => PbxWebSocket.Error -= h)
                .Select(e => (2, e.EventArgs.Exception.Message));
            var PbxObservableMessage = Observable.FromEventPattern<MessageReceivedEventArgs>(h => PbxWebSocket.MessageReceived += h, h => PbxWebSocket.MessageReceived -= h)
                .Select(e => (3, e.EventArgs.Message));
            var PbxObservable = PbxObservableOpen.Merge(PbxObservableClose).Merge(PbxObservableError).Merge(PbxObservableMessage);
            var subscription = PbxObservable.Subscribe(e =>
            {
                switch (e.Item1)
                {
                    case 0:
                        Console.WriteLine($"PbxWebSocket ({PbxWebSocket.State}): Opened...");
                        break;
                    case 1:
                        Console.WriteLine($"PbxWebSocket ({PbxWebSocket.State}): Closed...");
                        break;
                    case 2:
                        Console.WriteLine($"PbxWebSocket ({PbxWebSocket.State}): Error Reported: {e.Item2}");
                        break;
                    case 3:
                        Console.WriteLine($"PbxWebSocket ({PbxWebSocket.State}): Message Received: {e.Item2}");
                        break;
                }
            });

            PbxWebSocket.Open();
            Task.Delay(TimeSpan.FromSeconds(57)).Wait();

            PbxWebSocket.Close();
            Task.Delay(TimeSpan.FromSeconds(6)).Wait();

            subscription.Dispose();
        }
        static void Main(string[] args)
        {
            RxNetInActionClass.CurrentTest();
            Console.ReadLine();
        }
    }
}
