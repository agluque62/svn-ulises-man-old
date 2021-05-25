﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using U5kBaseDatos;
using U5kManServer;

namespace UnitTesting
{
    [TestClass]
    public class ProxyServiceTests
    {
        void SetPbxAdd(U5kBdtService db, string ip)
        {
            var sql = $"UPDATE equiposeu SET ipred1=\"{ip}\" WHERE interno=1;";
            db.Execute(sql);
        }
        void LoadConfig()
        {
            //string PbxIp = U5kManService.PbxEndpoint == null ? "none" : U5kManService.PbxEndpoint.Address.ToString();
            //List<BdtPabxDest> destinos = U5kManService.Database.ListaDestinosPABX(PbxIp);
            //U5kManService.GlobalData.CFGPBXS = destinos.Select(d => new Uv5kManDestinosPabx.DestinoPabx() { Id = d.Id }).ToList();
        }
        [TestMethod]
        public void BasicTest()
        {
            U5kManService.Database = new U5kBdtService(Thread.CurrentThread.CurrentUICulture, eBdt.bdtMySql, "127.0.0.1", "root", "cd40");
            LoadConfig();

            var service = new ProxyService();
            service.Subscribe((msg) =>
            {
                switch (msg.EventId)
                {
                    case ProxyService.EventIds.Activate:
                        Debug.WriteLine($"Proxy {msg.Sender} Activado...");
                        break;
                    case ProxyService.EventIds.Deactivate:
                        Debug.WriteLine($"Proxy {msg.Sender} Desactivado...");
                        break;
                    case ProxyService.EventIds.UserRegistered:
                        Debug.WriteLine($"En Proxy {msg.Sender} User {msg.UserId} Registrado...");
                        break;
                    case ProxyService.EventIds.UserUnregistered:
                        Debug.WriteLine($"En Proxy {msg.Sender} User {msg.UserId} Desregistrado...");
                        break;
                }
            });
            service.Start();

            Task.Delay(TimeSpan.FromSeconds(3)).Wait();
            EventBus.GlobalEvents.Publish(EventBus.GlobalEventsIds.Main);
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            EventBus.GlobalEvents.Publish(EventBus.GlobalEventsIds.CfgLoad);
            Task.Delay(TimeSpan.FromSeconds(60)).Wait();

            SetPbxAdd(U5kManService.Database, "127.0.0.1");
            EventBus.GlobalEvents.Publish(EventBus.GlobalEventsIds.CfgLoad);
            Task.Delay(TimeSpan.FromSeconds(30)).Wait();

            service.Stop();
            SetPbxAdd(U5kManService.Database, "10.68.60.36");
            U5kManService.Database.dbClose();
        }

        [TestMethod]
        public void BasicObservableTest()
        {
            IObservable<int> source = Observable.Range(1, 10);
            IDisposable subscription = source.Subscribe(
                x => Debug.WriteLine("OnNext: {0}", x),
                ex => Debug.WriteLine("OnError: {0}", ex.Message),
                () => Debug.WriteLine("OnCompleted"));
            //Console.WriteLine("Press ENTER to unsubscribe...");
            //Console.ReadLine();
            subscription.Dispose();
        }
    }
}
