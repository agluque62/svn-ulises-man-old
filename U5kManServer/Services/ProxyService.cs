using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using WebSocket4Net;
using Newtonsoft.Json;

using Utilities;
using NucleoGeneric;

namespace U5kManServer
{
    public class ProxyService : BaseCode, IDisposable
    {
        #region Proxy Events
        public enum EventIds { Activate, Deactivate, UserRegistered, UserUnregistered }
        public class EventMessage : ITinyMessage
        {
            public object Sender { get; private set; }
            public EventIds EventId { get; private set; }
            public string UserId { get; private set; }
            public EventMessage(string who, EventIds what, string user = "")
            {
                Sender = who;
                EventId = what;
                UserId = user;
            }
        }

        public event EventHandler ProxyEvent;
        public class ProxyEventArgs : EventArgs
        {
            public EventMessage Message { get; set; }
        }

        TinyMessengerHub SystemEventHub { get; set; } = new TinyMessengerHub();
        public object Subscribe(Action<EventMessage> notify)
        {
            return SystemEventHub.Subscribe<EventMessage>(msg => notify(msg));
        }
        public void Unsubscribe(object tk)
        {
            SystemEventHub.Unsubscribe<EventMessage>(tk as TinyMessageSubscriptionToken);
        }
        public void Publish(EventMessage msg)
        {
            SystemEventHub.Publish<EventMessage>(msg);
            ProxyEvent?.Invoke(this, new ProxyEventArgs() { Message = msg });
        }

        #endregion

        #region BKK Engine
        enum BkkEngineEvents
        {
            Open, Closed, Error,
            UserRegistered, UserUnregistered,
            UserAvailable, UserBusy, UserBusyUninterrupted
        }
        class BkkEngineEventArgs : EventArgs
        {
            public BkkEngineEvents Ev { get; set; }
            public string Idsub { get; set; }
            public override string ToString()
            {
                return $"BkkEvent: {Ev}, Subscriber: {Idsub}";
            }
        }
        class BkkEngine : BaseCode, IDisposable
        {
            #region IAgentEngine
            public event EventHandler<BkkEngineEventArgs> EventOccurred;
            /// <summary>
            /// 
            /// </summary>
            public bool Available
            {
                get
                {
                    Debug.Assert(PbxWebSocket != null);
                    return (PbxWebSocket.State == WebSocketState.Connecting || PbxWebSocket.State == WebSocketState.Open);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="OnEventOccurred"></param>
            public void Init(EventHandler<BkkEngineEventArgs> OnEventOccurred)
            {
                LogDebug<BkkEngine>($"Trying Init Engine...");
                EventOccurred = OnEventOccurred;
                Debug.Assert(WsEndpoint != null);
                String pbxUrl = String.Format("ws://{0}:{1}/pbx/ws?login_user={2}&login_password={3}&user=*&registered=True&status=True&line=*",
                    WsEndpoint.Address.ToString(),
                    WsEndpoint.Port.ToString(), User, Password);
                PbxWebSocket = new WebSocket(pbxUrl);
                PbxWebSocket.Opened += new EventHandler(OnWsOpened);
                PbxWebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(OnWsError);
                PbxWebSocket.Closed += new EventHandler(OnWsClosed);
                PbxWebSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(OnWsData);
                LogDebug<BkkEngine>($"Engine Init...");
            }
            /// <summary>
            /// 
            /// </summary>
            public void Start()
            {
                Debug.Assert(PbxWebSocket != null /*&& _pbxws.State == WebSocketState.Closed*/);
                WebSocketSpv = Task.Run(() =>
                {
                    do
                    {
                        switch (PbxWebSocket?.State)
                        {
                            case WebSocketState.Closed:
                            case WebSocketState.None:
                                LogDebug<BkkEngine>($"Trying Open PbxWebSocket...");
                                PbxWebSocket.Open();
                                LogDebug<BkkEngine>($"PbxWebSocket Opening...");
                                break;
                        }
                    } while (PbxWebSocketSync.WaitOne(TimeSpan.FromSeconds(5)) == false);

                    LogDebug<BkkEngine>($"Trying close PbxWebSocket...");
                    if (PbxWebSocket?.State == WebSocketState.Open)
                        PbxWebSocket?.Close();
                });
            }
            /// <summary>
            /// 
            /// </summary>
            public void Stop()
            {
                Debug.Assert(PbxWebSocket != null /*&& _pbxws.State == WebSocketState.Closed*/);
                PbxWebSocketSync?.Set();
                WebSocketSpv?.Wait();
                WebSocketSpv = null;
                PbxWebSocketSync?.Reset();
            }
            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                Debug.Assert(PbxWebSocket != null);

                EventOccurred = null;
                PbxWebSocket.Opened -= new EventHandler(OnWsOpened);
                PbxWebSocket.Error -= new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(OnWsError);
                PbxWebSocket.Closed -= new EventHandler(OnWsClosed);
                PbxWebSocket.MessageReceived -= new EventHandler<MessageReceivedEventArgs>(OnWsData);
                PbxWebSocket.Close();
                PbxWebSocket = null;
            }
            #endregion

            #region Eventos
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected virtual void OnBrekekeEventOccurred(BkkEngineEventArgs e)
            {
                EventOccurred?.Invoke(this, e);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void OnWsOpened(object sender, EventArgs e)
            {
                LogDebug<BkkEngine>($"PbxWebSocket ({PbxWebSocket.State}): Opened...");
                OnBrekekeEventOccurred(new BkkEngineEventArgs() { Ev = BkkEngineEvents.Open, Idsub = null });
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void OnWsClosed(object sender, EventArgs e)
            {
                LogDebug<BkkEngine>($"PbxWebSocket ({PbxWebSocket.State}): Closed...");
                OnBrekekeEventOccurred(new BkkEngineEventArgs() { Ev = BkkEngineEvents.Closed, Idsub = null });
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void OnWsError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
            {
                LogDebug<BkkEngine>($"PbxWebSocket ({PbxWebSocket.State}): Reported error: {e.Exception.Message}");
                OnBrekekeEventOccurred(new BkkEngineEventArgs() { Ev = BkkEngineEvents.Error, Idsub = e.Exception.Message });
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void OnWsData(object sender, MessageReceivedEventArgs e)
            {
                try
                {
                    LogTrace<BkkEngine>($"PbxWebSocket ({PbxWebSocket.State}): Message received: {e.Message}");

                    string msg = e.Message.Replace("params", "parametros");

                    if (msg.StartsWith("{"))
                    {
                        BkkMessage bkkmsg = JsonConvert.DeserializeObject<BkkMessage>(msg);
                        ProcessData(bkkmsg);
                    }
                    else if (msg.StartsWith("["))
                    {
                        JsonConvert.DeserializeObject<BkkMessage[]>(msg).ToList().ForEach(bkkmsg =>
                        {
                            ProcessData(bkkmsg);
                        });
                    }
                }
                catch (Exception x)
                {
                    LogException<BkkEngine>("OnWsData Exception", x);
                    LogTrace<BkkEngine>($"OnWebSocketData exception => {x}");
                }
            }

            #endregion

            #region Datos

            public IPEndPoint WsEndpoint { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            private WebSocket PbxWebSocket { get; set; } = null;
            private ManualResetEvent PbxWebSocketSync { get; set; } = new ManualResetEvent(false);
            private Task WebSocketSpv { get; set; } = null;

            #endregion

            #region Rutinas Internas

            class BkkParamInfo
            {
                // Evento Register
                public string registered { get; set; }
                public string user { get; set; }
                // Evento Status,
                public long time { get; set; }
                public string other_number { get; set; }
                public string status { get; set; }
            };
            /// <summary>
            /// 
            /// </summary>
            class BkkMessage
            {
                public string jsonrpc { get; set; }
                public string method { get; set; }
                public BkkParamInfo parametros { get; set; }
            };

            private void ProcessData(BkkMessage data)
            {
                switch (data.method)
                {
                    case "notify_serverstatus":
                        switch (data.parametros.status)
                        {
                            case "active":
                                OnBrekekeEventOccurred(new BkkEngineEventArgs() { Ev = BkkEngineEvents.Open, Idsub = null });
                                break;
                            default:
                                OnBrekekeEventOccurred(new BkkEngineEventArgs() { Ev = BkkEngineEvents.Error, Idsub = null });
                                break;
                        }
                        LogTrace<BkkEngine>($"Server Status Message: {data.parametros.status}");
                        break;

                    case "notify_status":
                        OnBrekekeEventOccurred(new BkkEngineEventArgs()
                        {
                            Ev = GetStatusEventcode(int.Parse(data.parametros.status)),
                            Idsub = data.parametros.user
                        });
                        LogTrace<BkkEngine>($"Status Message. User {data.parametros.user}, Status: {data.parametros.status}");
                        break;

                    case "notify_registered":
                        bool registered = data.parametros.registered == "true";
                        OnBrekekeEventOccurred(new BkkEngineEventArgs()
                        {
                            Ev = registered ? BkkEngineEvents.UserRegistered : BkkEngineEvents.UserUnregistered,
                            Idsub = data.parametros.user
                        });
                        LogTrace<BkkEngine>($"Registration message. User {data.parametros.user}, {data.parametros.registered}");
                        break;

                    default:
                        LogError<BkkEngine>($"Unknown event => {data.method}.");
                        break;
                }
            }

            /*  Estado Externo          Estado Interno
                CALLING = 0             2, Ocupado No Interrumplible.
                INCOMING = 1            2, Ocupado No Interrumplible.
                CALL_SUCCESS = 2        1, Ocupado Interrumplible.
                ENDTALKING = 12         2, Ocupado No Interrumplible.
                ANSWER_SUCCESS = 14     1, Ocupado Interrumplible.
                PARK_CANCEL = 21        2, Ocupado No Interrumplible.
                PARK_START = 30         2, Ocupado No Interrumplible.
                STARTRINGING = 65       2, Ocupado No Interrumplible.
                HOLD = 35               1, Ocupado Interrumplible.
                UNHOLD = 36             1, Ocupado Interrumplible.
                DISCONNECT = -1         0, Libre
             * * */
            private BkkEngineEvents GetStatusEventcode(int extStatus)
            {
                switch (extStatus)
                {
                    case -1:
                        return BkkEngineEvents.UserAvailable;
                    case 2:
                    case 14:
                    case 35:
                    case 36:
                        return BkkEngineEvents.UserBusy;
                    case 0:
                    case 1:
                    case 12:
                    case 21:
                    case 30:
                    case 65:
                        return BkkEngineEvents.UserBusyUninterrupted;
                    default:
                        return BkkEngineEvents.UserAvailable;
                }
            }

            #endregion
        }
        #endregion BKK Engine

        #region public
        public ProxyService()
        {
        }
        public void Start()
        {
            lock (Locker)
            {
                Tick(TickInterval);
                GlobalEventsToken = EventBus.GlobalEvents.Subscribe(OnGlobalEvent);
                LogDebug<ProxyService>($"ProxyService Started.");
            }
        }
        public void Stop()
        {
            lock (Locker)
            {
                // Para el Engine...
                StopEngine();
                EventBus.GlobalEvents.Unsubscribe(GlobalEventsToken);
                LogDebug<ProxyService>($"ProxyService Stopped.");
            }
        }
        public void Dispose()
        {
        }
        #endregion public

        #region event managers
        private void OnGlobalEvent(EventBus.GlobalEventsIds eventid)
        {
            LogDebug<ProxyService>($"Global Event => {eventid}");
            lock (Locker)
            {
                switch (eventid)
                {
                    case EventBus.GlobalEventsIds.Main:
                        if (!Master)
                        {
                            LogDebug<ProxyService>($"ProxyService Changing to MASTER.");
                        }
                        Master = true;
                        LogDebug<ProxyService>($"ProxyService MASTER.");
                        break;
                    case EventBus.GlobalEventsIds.Standby:
                        if (Master)
                        {
                            LogDebug<ProxyService>($"ProxyService Changing to STANDBY.");

                            // Para el Engine...
                            StopEngine();
                        }
                        Master = false;
                        LogDebug<ProxyService>($"ProxyService STANDBY.");
                        break;
                    case EventBus.GlobalEventsIds.CfgLoad:
                        if (Master)
                        {
                            LoadConfig((change) =>
                            {
                                if (change)
                                {
                                    // Reinicio el Engine...
                                    LogDebug<ProxyService>($"ProxyService Reinicio Engine.");
                                    StopEngine();
                                    if (CurrentPbxEnable)
                                    {
                                        LogDebug<ProxyService>($"ProxyService Creando Engine en {PbxVirtualIp}.");
                                        StartEngine();
                                    }
                                }
                            });
                        }
                        LogDebug<ProxyService>($"ProxyService Configuration Loaded.");
                        break;
                }
            }
        }
        private void OnPbxEngineEvent(object sender, BkkEngineEventArgs eventOcurred)
        {
            LogDebug<ProxyService>($"Bkk Event => {eventOcurred}");
            lock (Locker)
            {
                switch (eventOcurred.Ev)
                {
                    case BkkEngineEvents.Open:
                        Publish(new EventMessage("hola", EventIds.Activate));
                        break;
                    case BkkEngineEvents.Closed:
                        Publish(new EventMessage("hola", EventIds.Deactivate));
                        break;
                    case BkkEngineEvents.UserRegistered:
                        Publish(new EventMessage("hola", EventIds.UserRegistered, eventOcurred.Idsub));
                        break;
                    case BkkEngineEvents.UserUnregistered:
                        Publish(new EventMessage("hola", EventIds.UserUnregistered, eventOcurred.Idsub));
                        break;
                    case BkkEngineEvents.Error:
                        break;
                }
            }
        }
        #endregion event managers

        #region private methods
        private void Tick(TimeSpan interval)
        {
            TimerTick = new System.Threading.Timer(x =>
            {
                lock (Locker)
                {
                    LogDebug<ProxyService>($"Tick. Master: {Master}. ");
                    if (Master)
                    {
                    }
                    IamAlive.Tick("Proxy Service", () => IamAlive.Message("Proxy Service. Is Alive."));
                    Tick(interval);
                }
            }, null, interval, Timeout.InfiniteTimeSpan);

        }
        private void LoadConfig(Action<bool> response)
        {
            bool change = PbxEnabled != CurrentPbxEnable || PbxVirtualIp != CurrentPbxVirtualIp;
            CurrentPbxEnable = PbxEnabled;
            CurrentPbxVirtualIp = PbxVirtualIp;
            response(change);
        }
        private void StartEngine()
        {
            LogDebug<ProxyService>($"ProxyService Trying Open Engine.");
            PbxEngine = new BkkEngine() { WsEndpoint = new IPEndPoint(IPAddress.Parse(PbxVirtualIp), 1444), User = "sa", Password = "sa" }; // todo
            PbxEngine?.Init(OnPbxEngineEvent);
            PbxEngine?.Start();
            LogDebug<ProxyService>($"ProxyService Engine starting.");
        }
        private void StopEngine()
        {
            LogDebug<ProxyService>($"ProxyService Trying close Engine.");
            PbxEngine?.Stop();
            PbxEngine?.Dispose();
            PbxEngine = null;
            LogDebug<ProxyService>($"ProxyService Engine closed.");
        }
        private void CheckProxyData()
        {
            if (PbxEnabled)
            {

            }
            else
            {

            }
        }

        #endregion methods

        #region private data
        private bool Master { get; set; }
        private Object Locker { get; set; } = new object();
        private BkkEngine PbxEngine { get; set; } = null;
        private System.Threading.Timer TimerTick { get; set; }
        private TimeSpan TickInterval = TimeSpan.FromSeconds(5);
        private TimeSpan TestProxyDataInterval = TimeSpan.FromSeconds(30);
        private DateTime LastTestProxyData = DateTime.MinValue;
        private Object GlobalEventsToken { get; set; }
        private bool CurrentPbxEnable { get; set; } = false;
        private string CurrentPbxVirtualIp { get; set; } = "";
        #endregion private data

        #region External Data
        bool PbxEnabled => U5kManService.PbxEndpoint != null;
        string PbxVirtualIp => U5kManService.PbxEndpoint == null ? "none" : U5kManService.PbxEndpoint.Address.ToString();
        bool DualMode => true;

        #endregion External Data

    }
}
