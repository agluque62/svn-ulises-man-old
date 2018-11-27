﻿// #define _EXPLORE_THREADS_
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Net;

using System.Threading.Tasks;
using System.Net.Http;

using Lextm.SharpSnmpLib;
//using Lextm.SharpSnmpLib.Messaging;
//using Lextm.SharpSnmpLib.Objects;
//using Lextm.SharpSnmpLib.Pipeline;

using NucleoGeneric;
using U5kBaseDatos;
using U5kManServer.WebAppServer;

using Utilities;
namespace U5kManServer
{
    /// <summary>
    /// 
    /// </summary>
    enum eGwPar
    {
        None = 0,
        GwStatus,
        Slot0Type, Slot1Type, Slot2Type, Slot3Type,
        Slot0Status, Slot1Status, Slot2Status, Slot3Status, LanStatus, MainOrStandby,
        ResourceType,
        RadioResourceType, RadioResourceStatus,
        IntercommResourceType, IntercommResourceStatus,
        LegacyPhoneResourceType, LegacyPhoneResourceStatus,
        ATSPhoneResourceType, ATSPhoneResourceStatus,
        Lan2Status
    };

    class GwHelper
    {
        public static string SlotStd2String(Int32 nslot, Int32 tipo, Int32 slotstd)
        {
            // BIT | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
            // CAN | - | - | - | 3 | 2 | 1 | 0 | - |
            return tipo == 2 ? String.Format("{4}: [{0} {1} {2} {3}]",
                                (slotstd & Convert.ToInt32("00010", 2)) != 0 ? "0" : "-",
                                (slotstd & Convert.ToInt32("00100", 2)) != 0 ? "1" : "-",
                                (slotstd & Convert.ToInt32("01000", 2)) != 0 ? "2" : "-",
                                (slotstd & Convert.ToInt32("10000", 2)) != 0 ? "3" : "-",
                                nslot) : String.Format("{0}", nslot);
        }
        public static void SetToOutOfOrder(stdPhGw phgw)
        {
            phgw.SnmpDataReset();
            phgw.version = string.Empty;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    class GwExplorer : NucleoGeneric.NGThread
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static string OidGet(string id, string _default)
        {
            foreach (string s in Properties.u5kManServer.Default.GwOids)
            {
                string[] p = s.Split(':');
                if (p.Count() == 2 && p[0] == id)
                    return p[1];
            }

            return _default;
        }
        /// <summary>
        /// 
        /// </summary>
        static public Dictionary<eGwPar, string> _GwOids = new Dictionary<eGwPar, string>()
        {
            // General de la Pasarela.
            {eGwPar.GwStatus,OidGet("EstadoGw",".1.1.100.2.0")},
            {eGwPar.Slot0Type,OidGet("TipoSlot0",".1.1.100.31.1.1.0")},    
            {eGwPar.Slot1Type,OidGet("TipoSlot1",".1.1.100.31.1.1.1")},
            {eGwPar.Slot2Type,OidGet("TipoSlot2",".1.1.100.31.1.1.2")},
            {eGwPar.Slot3Type,OidGet("TipoSlot3",".1.1.100.31.1.1.3")},
            {eGwPar.Slot0Status,OidGet("EstadoSlot0",".1.1.100.31.1.2.0")},
            {eGwPar.Slot1Status,OidGet("EstadoSlot1",".1.1.100.31.1.2.1")},
            {eGwPar.Slot2Status,OidGet("EstadoSlot2",".1.1.100.31.1.2.2")},
            {eGwPar.Slot3Status,OidGet("EstadoSlot3",".1.1.100.31.1.2.3")},
            {eGwPar.MainOrStandby,OidGet("PrincipalReserva","1.1.100.21.0")},
            {eGwPar.LanStatus,OidGet("EstadoLan","1.1.100.22.0")},
        
            // Para cada Recurso.
            {eGwPar.ResourceType,OidGet("TipoRecurso",".1.1.100.100.0")},
            
            {eGwPar.RadioResourceType,OidGet("TipoRecursoRadio",".1.1.200")},
            {eGwPar.RadioResourceStatus,OidGet("EstadoRecursoRadio",".1.1.200.2.0")},         
            
            {eGwPar.IntercommResourceType,OidGet("TipoRecursoLC",".1.1.300")},
            {eGwPar.IntercommResourceStatus,OidGet("EstadoRecursoLC",".1.1.300.2.0")},
            
            {eGwPar.LegacyPhoneResourceType,OidGet("TipoRecursoTF",".1.1.400")},
            {eGwPar.LegacyPhoneResourceStatus,OidGet("EstadoRecursoTF",".1.1.400.2.0")},
            
            {eGwPar.ATSPhoneResourceType,OidGet("TipoRecursoATS",".1.1.500")},
            {eGwPar.ATSPhoneResourceStatus,OidGet("EstadoRecursoATS",".1.1.500.2.0")},
            
        };

        /// <summary>
        /// Polling a la Pasarela.
        /// </summary>
        static List<Variable> _GwVarList = new List<Variable>()
        {
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.GwStatus])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.MainOrStandby])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.LanStatus])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot0Type])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot1Type])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot2Type])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot3Type])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot0Status])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot1Status])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot2Status])),
            new Variable(new ObjectIdentifier(_GwOids[eGwPar.Slot3Status]))
        };

        /// <summary>
        /// Tipos Notificados...
        /// </summary>
        const int RadioResource_AgentType = 2;
        const int IntercommResource_AgentType = 3;
        const int LegacyPhoneResource_AgentType = 4;
        const int ATSPhoneResource_AgentType = 5;

        /// <summary>
        /// Oid de los Tipos Reales segun los tipos notificados.
        /// </summary>
        Dictionary<int, string> _TypesOids = new Dictionary<int, string>() 
        { 
            { RadioResource_AgentType, _GwOids[eGwPar.RadioResourceType] }, 
            { IntercommResource_AgentType, _GwOids[eGwPar.IntercommResourceType] }, 
            { LegacyPhoneResource_AgentType, _GwOids[eGwPar.LegacyPhoneResourceType]}, 
            { ATSPhoneResource_AgentType, _GwOids[eGwPar.ATSPhoneResourceType] }, 
        };

        /// <summary>
        /// Oid de los Estados segun los tipos notificados.
        /// </summary>
        Dictionary<int, string> _StatusOids = new Dictionary<int, string>() 
        { 
            { RadioResource_AgentType, _GwOids[eGwPar.RadioResourceStatus] }, 
            { IntercommResource_AgentType, _GwOids[eGwPar.IntercommResourceStatus] }, 
            { LegacyPhoneResource_AgentType, _GwOids[eGwPar.LegacyPhoneResourceStatus] }, 
            { ATSPhoneResource_AgentType, _GwOids[eGwPar.ATSPhoneResourceStatus] }, 
        };

        /// <summary>
        /// Incidencias de Activacion Asociadas a los tipos de Recurso notificados.
        /// </summary>
        static protected Dictionary<trc, eIncidencias> ResourceActivationEventsCodes = new Dictionary<trc, eIncidencias>()
        {
            {trc.rcRadio, eIncidencias.IGW_CONEXION_RECURSO_RADIO},
            {trc.rcTLF, eIncidencias.IGW_CONEXION_RECURSO_TLF},
            {trc.rcLCE, eIncidencias.IGW_CONEXION_RECURSO_TLF},
            {trc.rcATS, eIncidencias.IGW_CONEXION_RECURSO_R2},
            {trc.rcNotipo, eIncidencias.IGNORE}
        };

        /// <summary>
        /// Incidencias de Desactivacion Asociadas a los tipos de Recurso notificados...
        /// </summary>
        static protected Dictionary<trc, eIncidencias> ResourceDeactivationEventsCodes = new Dictionary<trc, eIncidencias>()
        {
            {trc.rcRadio, eIncidencias.IGW_DESCONEXION_RECURSO_RADIO},
            {trc.rcTLF, eIncidencias.IGW_DESCONEXION_RECURSO_TLF},
            {trc.rcLCE, eIncidencias.IGW_DESCONEXION_RECURSO_TLF},
            {trc.rcATS, eIncidencias.IGW_DESCONEXION_RECURSO_R2},
            {trc.rcNotipo, eIncidencias.IGNORE}
        };

        /// <summary>
        /// 
        /// </summary>
        public static event ChangeStatusDelegate ChangeStatus;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdata"></param>
        public GwExplorer(ChangeStatusDelegate ChangeStatusRoutine)
        {
            Name = "GwSnmpExplorer";
            ChangeStatus = ChangeStatusRoutine;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void Run()
        {
            U5kGenericos.TraceCurrentThread(this.GetType().Name);

            //U5kGenericos.SetCurrentCulture();
            LogInfo<GwExplorer>("Arrancado...");

            Decimal interval = Properties.u5kManServer.Default.SpvInterval;

            using (timer = new TaskTimer(new TimeSpan(0, 0, 0, 0, Decimal.ToInt32(interval)), this.Cancel))
            {
                while (IsRunning())
                {
                    if (U5kManService._std.wrAccAcquire())
                    {
                        try
                        {
                            if (U5kManService._Master == true)
                            {
                                List<stdGw> stdgws = U5kManService._std.STDGWS;
                                List<Task> task = new List<Task>();

                                Utilities.TimeMeasurement tm = new Utilities.TimeMeasurement("GW Explorer");
                                foreach (stdGw gw in stdgws)
                                {
                                    task.Add(
                                        Task.Factory.StartNew(() =>
                                            {
                                                try
                                                {
                                                    U5kGenericos.TraceCurrentThread(this.GetType().Name + " " + gw.name);
                                                    ExploraGw(gw);
                                                }
                                                catch (Exception x)
                                                {
                                                    LogException<GwExplorer>("Supervisando Pasarela " + gw.name, x);
                                                }
                                            }, TaskCreationOptions.LongRunning)
                                    );
                                }
                                Task.WaitAll(task.ToArray(), 9000);
                                tm.StopAndPrint((msg) =>
                                {
                                    LogTrace<GwExplorer>(msg);
                                });
                                U5kManService._std.STDGWS = stdgws;
                            }
                        }
                        catch (Exception x)
                        {
                            if (x is ThreadAbortException)
                            {
                                Thread.ResetAbort();
                                break;
                            }
                            LogException<GwExplorer>("Supervisando Pasarelas ", x);
                        }
                        finally
                        {
                            U5kManService._std.wrAccRelease();
                        }
                    }

                    GoToSleepInTimer();
                }
            }
            LogInfo<GwExplorer>("Finalizado...");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nslot"></param>
        /// <param name="slot"></param>
        /// <param name="tipo"></param>
        static public void SlotTypeSet(/*stdGw gw, */stdPhGw pgw, int nslot, stdSlot slot, int tipo, int estado)
        {
            std current = tipo == 2 ? std.Ok : std.NoInfo;
            estado = tipo == 2 ? (estado | 0x01) : (estado & 0xFFFE);

            //slot.std_online = ChangeStatus(slot.std_online,
            //                               current, 0,
            //                               current != std.Ok ? eIncidencias.IGW_DESCONEXION_IA4 : eIncidencias.IGW_CONEXION_IA4,
            //                               eTiposInci.TEH_TIFX, pgw.name, /*nslot, */GwHelper.SlotStd2String(nslot, tipo, estado));

            if (slot.lastResMsc != estado || current != slot.std_online)
            {
                if (current == std.Ok)
                {
                    RecordEvent<GwExplorer>(DateTime.Now, eIncidencias.IGW_CONEXION_IA4, eTiposInci.TEH_TIFX, pgw.name, /*nslot, */
                        Params(GwHelper.SlotStd2String(nslot, 2, estado)));
                }
                else
                {
                    RecordEvent<GwExplorer>(DateTime.Now, eIncidencias.IGW_DESCONEXION_IA4, eTiposInci.TEH_TIFX, pgw.name, /*nslot, */
                        Params(GwHelper.SlotStd2String(nslot, 2, estado)));
                    slot.Reset();
                }
                slot.lastResMsc = estado;
                slot.std_online = current;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nslot"></param>
        /// <param name="slot"></param>
        /// <param name="estado"></param>
        static public void SlotStateSet(/*stdGw gw, */stdPhGw pgw, int nslot, stdSlot slot, int estado)
        {
            /** El primer bit es el estado de la tarjeta */
            estado = (estado >> 1);

            for (int irec = 0; irec < 4; irec++)
            {
                stdRec rec = slot.rec[irec];
                bool presente = ((estado >> irec) & 1) == 1 ? true : false;

                // 20180111. Quitamos este historico....
                // ChangeStatus(rec.presente ? std.Ok : std.NoInfo,
                //    presente ? std.Ok : std.NoInfo,
                //    0,
                //    eIncidencias.IGW_EVENTO,
                //    eTiposInci.TEH_TIFX,
                //    pgw.name,
                //    presente ? idiomas.strings.GWS_ResInterfazSi : idiomas.strings.GWS_ResInterfazNo,   // "Interfaz de Recurso Disponible" : "Interfaz de Recurso no Disponible",
                //    nslot, irec);

                // 20180111. Quitamos el historico y solo actualizamos las variables locales...
                //eIncidencias inci = rec.tipo == itf.rcRadio ? eIncidencias.IGW_DESCONEXION_RECURSO_RADIO :
                //    rec.tipo == itf.rcAtsN5 || rec.tipo == itf.rcAtsR2 ? eIncidencias.IGW_DESCONEXION_RECURSO_R2 :
                //    eIncidencias.IGW_DESCONEXION_RECURSO_TLF;

                //rec.std_online = ChangeStatus(rec.std_online,
                //                              (presente == false ? std.NoInfo : rec.std_online),
                //                              0,
                //                              inci,
                //                              eTiposInci.TEH_TIFX, pgw.name, rec.name);
                rec.presente = presente;
                rec.std_online = (presente == false ? std.NoInfo : rec.std_online);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="slot"></param>
        protected void SlotRecsAct(/*stdGw gw, */stdPhGw pgw, string ip, stdSlot slot)
        {
            if (slot.std_online != std.Ok)
                return;

            SnmpClient snmpc = new SnmpClient();
            for (int rec = 0; rec < 4; rec++)
            {
                //if (slot.rec[rec].bdt == false || slot.rec[rec].presente == false)
                //    continue;
                if (/*slot.rec[rec].bdt == false || */slot.rec[rec].presente == false)
                    continue;

                int tipo = -1;

                // Tipo Notificado de Recurso.
                if (!snmpc.GetInt(ip, slot.rec[rec].snmp_port, "public", _GwOids[eGwPar.ResourceType], 1000, out tipo))
                    continue;

                if (!TestTipoNotificado(tipo))
                    continue;

                SlotRecursoTipoAgenteSet(/*gw, */pgw, slot.rec[rec], tipo);

                // Tipo de Interfaz
                /**
                int titf = (int)itf.rcNotipo;
                SnmpClient.GetInt(ip, slot.rec[rec].snmp_port, "public", OidsTipos[tipo], 1000, out titf);
                SlotRecursoTipoInterfazSet(gw, pgw, slot.rec[rec], titf);
                */

                // Estado  de Recurso.
                int estado = (int)std.NoInfo;
                if (!snmpc.GetInt(ip, slot.rec[rec].snmp_port, "public", _StatusOids[tipo], 1000, out estado))
                    continue;

                SlotRecursoEstadoSet(/*gw, */pgw, slot.rec[rec], estado, (trc)tipo);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gwname"></param>
        /// <param name="rec"></param>
        /// <param name="tipo"></param>
        public static void SlotRecursoTipoAgenteSet(/*stdGw gw, */stdPhGw pgw, stdRec rec, int tipo)
        {
            rec.tipo_online = (trc)tipo;              // Todo. Generar Incidencia si procede. Esta Incidencia no Existe.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gwname"></param>
        public static void SlotRecursoTipoInterfazSet(/*stdGw gw, */stdPhGw pgw, stdRec rec, int tipo)
        {
            rec.tipo_itf = (itf)tipo;               // Todo. Generar Incidencia si procede. Esta Incidencia no Existe.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gwname"></param>
        /// <param name="rec"></param>
        /// <param name="estado">0: NP, 1: OK, 2: Fallo, 3: Degradado.</param>
        public static void SlotRecursoEstadoSet(/*stdGw gw, */stdPhGw pgw, stdRec rec, int estado, trc tipo)
        {
            if (rec.presente)
            {
                if (tipo == trc.rcRadio)
                {
                    /** Para evitar los Fuera de Servicio por falta de sesion radio... */
                    std current = estado == 1 ? std.Ok : std.Error;
                    if (rec.std_online != current)
                    {
                        rec.std_online = current;
                        RecordEvent<GwExplorer>(DateTime.Now, estado != 0 ? ResourceActivationEventsCodes[tipo] : ResourceDeactivationEventsCodes[tipo],
                                                  eTiposInci.TEH_TIFX, pgw.name, Params(rec.name));
                    }
                }
                else
                {
                    rec.std_online = ChangeStatus(rec.std_online,
                                                  estado == 1 ? std.Ok : std.Error, 0,
                                                  estado == 1 ? ResourceActivationEventsCodes[tipo] : ResourceDeactivationEventsCodes[tipo],
                                                  eTiposInci.TEH_TIFX, pgw.name, rec.name);
                }
            }
            else
            {
                rec.std_online = std.NoInfo;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        bool TestTipoNotificado(int tipo)
        {
            return tipo ==
                RadioResource_AgentType ||
                tipo == IntercommResource_AgentType ||
                tipo == LegacyPhoneResource_AgentType ||
                tipo == ATSPhoneResource_AgentType ? true : false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gw"></param>
        public static void GwActualizaEstado(stdGw gw)
        {
            //std std_old = gw.std;

            gw.presente = gw.Dual == false ? gw.gwA.presente : (gw.gwA.presente == true || gw.gwB.presente == true) ? true : false;
            if (gw.presente == false)
                gw.Reset();
            gw.std = gw.presente == false ? std.NoInfo : gw.Errores == true ? std.Error : std.Ok;

            //if (std_old != gw.std)
            //{
            //    switch (gw.std)           // Ha habido un cambio de estado global...
            //    {
            //        case std.NoInfo:        // Desconectado o No Operativo.
            //            U5kEstadisticaProc.Estadisticas.EventoPasarela(gw.name, false);
            //            break;

            //        case std.Error:         // Conectado pero en Fallo.
            //        case std.Ok:            // Plenamente operativo.
            //            U5kEstadisticaProc.Estadisticas.EventoPasarela(gw.name, true);
            //            break;

            //        default:
            //            break;
            //    }
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gwp"></param>
        /// <param name="pgw"></param>
        /// <param name="estado"></param>
        public static void PhGwCambioEstado(/*stdGw gw, */stdPhGw pgw, int estado)
        {
            /* Actualizo el estado de la pasarela fisica **/
            bool nuevo_estado = estado == 1 ? true : false;
            if (nuevo_estado != pgw.presente)
            {
                pgw.presente = nuevo_estado;
                std std_new = pgw.presente == false ? std.NoInfo : pgw.Errores == true ? std.Error : std.Ok;
                eIncidencias inci = std_new == std.NoInfo ? eIncidencias.IGW_CAIDA : eIncidencias.IGW_ENTRADA;

                pgw.std = ChangeStatus(pgw.std, std_new, 0, inci, eTiposInci.TEH_TIFX, pgw.name);

                if (pgw.presente == false)
                {
                    /** Reset Estado GW fisica */
                    pgw.Reset();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pgw"></param>
        /// <param name="estado"></param>
        public static void PhGwPrincipalReservaSet(/*stdGw gw, */stdPhGw pgw, int estado)
        {
            pgw.Seleccionada = ChangeStatus(pgw.Seleccionada == true ? std.Ok : std.NoInfo,
                estado == 0 ? std.NoInfo : std.Ok, 0, eIncidencias.IGW_PRINCIPAL_RESERVA, eTiposInci.TEH_TIFX, pgw.name,
                pgw.name,
                estado == 0 ? idiomas.strings.GWS_Reserva : idiomas.strings.GWS_Principal/* "Reserva" : "Principal"*/) == std.Ok ? true : false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pwg"></param>
        /// <param name="status"></param>
        public static void PhGwLanStatusSet(/*stdGw gw, */stdPhGw pgw, int status)
        {
            int bond = (status & 0x4) >> 2;
            int eth1 = (status & 0x2) >> 1;
            int eth0 = (status & 0x1);

            pgw.lan1 = eth0 == 1 ? std.Ok : std.Error;
            pgw.lan2 = bond == 0 ? std.NoInfo : eth1 == 1 ? std.Ok : std.Error;
        }

        #region Threads de Exploracion en Paralelo.

        /// <summary>
        /// Explora una pasarela logica.
        /// </summary>
        /// <param name="obj"></param>
        protected void ExploraGw(object obj)
        {
            stdGw gw = (stdGw)obj;

            ExplorePhGw(gw.gwA);
            if (gw.Dual)
                ExplorePhGw(gw.gwB);

            /** Actualiza los Parametros Globales de la Pasarela */
            GwActualizaEstado(gw);
            
            /** */
            if (gw.presente)
                GetNtpStatus(gw);
        }

        #region Exploracion de GW Unificada

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipgw"></param>
        /// <param name="port"></param>
        protected void ExploraGwStdGen_unificada(stdPhGw pgw)
        {
            IPEndPoint gwep = new IPEndPoint(IPAddress.Parse(pgw.ip), pgw.snmpport);
            OctetString community = new OctetString("public");
            List<Variable> vIn = new List<Variable>()
            {
                new Variable(new ObjectIdentifier(".1.3.6.1.4.1.7916.8.3.1.1.2.0")),   // Estado Hw.
                new Variable(new ObjectIdentifier(".1.3.6.1.4.1.7916.8.3.1.1.6.0")),   // Estado LAN1
                new Variable(new ObjectIdentifier(".1.3.6.1.4.1.7916.8.3.1.1.7.0")),   // Estado LAN2
                new Variable(new ObjectIdentifier(".1.3.6.1.4.1.7916.8.3.1.1.8.0")),   // Estado P/R,
                new Variable(new ObjectIdentifier(".1.3.6.1.4.1.7916.8.3.1.1.4.0")),   // Estado FA,
                new Variable(new ObjectIdentifier(".1.3.6.1.4.1.7916.8.3.1.1.1.0")),   // Identificador. Habilita el envio de TRAPS
            };
            SnmpClient snmpc = new SnmpClient();

            IList<Variable> vOut = snmpc.Get(VersionCode.V2, gwep, community, vIn, pgw.SnmpTimeout, pgw.SnmpReintentos);
            // estadoGeneral. 0: No Inicializado, 1: Ok, 2: Fallo, 3: Aviso.
            int stdGeneral = snmpc.Integer(vOut[0].Data);
            // stdLAN1. 0: No Presente, 1: Ok, 2: Error.
            int stdLan1 = snmpc.Integer(vOut[1].Data);
            // stdLAN2. 0: No Presente, 1: Ok, 2: Error.
            int stdLan2 = snmpc.Integer(vOut[2].Data);
            // stdCpuLocal. 0: No Presente. 1: Principal, 2: Reserva, 3: Arrancando
            int stdPR = snmpc.Integer(vOut[3].Data);
            // stdFA. 0: No Presente. 1: Ok, 2: Error
            int stdFA = snmpc.Integer(vOut[4].Data);
            pgw.std = stdGeneral == 0 ? std.NoInfo : stdGeneral == 1 ? std.Ok : std.Error;

            int stdLan = (stdLan1 == 1 ? 0x01 : 0x00) | (stdLan2 == 1 ? 0x02 : 0x00);
            PhGwLanStatusSet(pgw, (0x04 | stdLan));                 // En este tipo de Pasarelas BOND configurado...

            PhGwPrincipalReservaSet(pgw, stdPR == 2 ? 0 : 1);

            pgw.stdFA = stdFA == 0 ? std.NoInfo : stdFA == 1 ? std.Ok : stdFA == 2 ? std.Error : std.NoExiste;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        protected void ExploraSlot_unificada(object obj)
        {
            KeyValuePair<stdPhGw, int> objIn = (KeyValuePair<stdPhGw, int>)obj;
            stdPhGw gw = objIn.Key;
            int nslot = objIn.Value;
            stdSlot slot = gw.slots[nslot];
            IPEndPoint gwep = new IPEndPoint(IPAddress.Parse(gw.ip), gw.snmpport);
            OctetString community = new OctetString("public");

            try
            {
                string oidbase = ".1.3.6.1.4.1.7916.8.3.1.3.2.1.";
                List<Variable> vIn = new List<Variable>()
                {
                    new Variable(new ObjectIdentifier(oidbase+"2."+(nslot+1).ToString())),   // Tipo. 0: Error, 1: IA4, 2: IQ1
                    new Variable(new ObjectIdentifier(oidbase+"3."+(nslot+1).ToString())),   // Status,
                    new Variable(new ObjectIdentifier(oidbase+"4."+(nslot+1).ToString())),   // Canal-0
                    new Variable(new ObjectIdentifier(oidbase+"5."+(nslot+1).ToString())),   // Canal-1
                    new Variable(new ObjectIdentifier(oidbase+"6."+(nslot+1).ToString())),   // Canal-2
                    new Variable(new ObjectIdentifier(oidbase+"7."+(nslot+1).ToString()))    // Canal-3
                };
                SnmpClient snmpc = new SnmpClient();

                IList<Variable> vOut = snmpc.Get(VersionCode.V2, gwep, community, vIn, gw.SnmpTimeout, gw.SnmpReintentos);
                int stipo = snmpc.Integer(vOut[0].Data);                            // 0: Error, 1: IA4, 2: IQ1
                int status = snmpc.Integer(vOut[1].Data);                           // 0: No presente, 1: Presente

                stipo = status == 0 ? 0 : (stipo == 1 ? 2 : 0);

                int can0 = snmpc.Integer(vOut[2].Data);                             // 0: Desconectada. 1: Conectada
                int can1 = snmpc.Integer(vOut[3].Data);                             // 0: Desconectada. 1: Conectada
                int can2 = snmpc.Integer(vOut[4].Data);                             // 0: Desconectada. 1: Conectada
                int can3 = snmpc.Integer(vOut[5].Data);                             // 0: Desconectada. 1: Conectada

                int std = (can0 << 1) | (can1 << 2) | (can2 << 3) | (can3 << 4);

                SlotTypeSet(gw, nslot, gw.slots[nslot], stipo, std);
                SlotStateSet(gw, nslot, gw.slots[nslot], std);

                for (int rec = 0; rec < 4; rec++)
                {
                    if (slot.rec[rec].presente == true)
                    {
                        ExploraRecurso_unificada(new KeyValuePair<stdPhGw, int>(gw, nslot * 4 + rec));
                    }
                    else
                    {
                        Reset_ExploraRecurso(gw, nslot, rec);
                    }
                }
            }

            catch (Exception x)
            {
                LogException<GwExplorer>(String.Format(" Explorando Slot. CGW {0}.{1}",
                    obj == null ? "null" : ((KeyValuePair<stdPhGw, int>)obj).Key.ip,
                    obj == null ? "null" : ((KeyValuePair<stdPhGw, int>)obj).Value.ToString()), x);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        protected void ExploraRecurso_unificada(object obj)
        {
            KeyValuePair<stdPhGw, int> objIn = (KeyValuePair<stdPhGw, int>)obj;
            stdPhGw gw = objIn.Key;
            int nres = objIn.Value;
            int nslot = nres / 4;
            int ires = nres % 4;
            stdRec rec = gw.slots[nslot].rec[ires];
            IPEndPoint gwep = new IPEndPoint(IPAddress.Parse(gw.ip), gw.snmpport);
            OctetString community = new OctetString("public");

            if (gw.name == "" && nslot == 0 && ires == 1)
                LogInfo<GwExplorer>(String.Format("Presencia (1) Slot 0, Recurso 1: {0}", gw.slots[0].rec[1].presente));

            try
            {
                string oidbase = ".1.3.6.1.4.1.7916.8.3.1.4.2.1.";
                List<Variable> vIn = new List<Variable>()
                {
                    new Variable(new ObjectIdentifier(oidbase+"3."+(nres+1).ToString())),   // Tipo
                    new Variable(new ObjectIdentifier(oidbase+"6."+(nres+1).ToString())),   // Status Hardware,
                    new Variable(new ObjectIdentifier(oidbase+"15."+(nres+1).ToString())),  // Status Interfaz.
                };
                SnmpClient snmpc = new SnmpClient();

                IList<Variable> vOut = snmpc.Get(VersionCode.V2, gwep, community, vIn, gw.SnmpTimeout, gw.SnmpReintentos);

                int ntipo = snmpc.Integer(vOut[0].Data);   // 0: RD, 1: LC, 2: BC, 3: BL, 4: AB, 5: R2, 6: N5, 7: QS, 9: NP, 13: PPEM 
                if (ntipo == 9)
                {
                    // 20170630. El código 9 no es no presente sino NO CONFIGURADO
                    // Reset_ExploraRecurso(gw, nslot, ires);
                    // rec.presente = false;
                    rec.tipo_itf = itf.rcNotipo;
                    rec.tipo_online = trc.rcNotipo;
                    rec.std_online = std.NoInfo;
                }
                else if ((ntipo >= 0 && ntipo < 9) || ntipo == 13)
                {
                    int TipoNotificado = ntipo == 0 ? RadioResource_AgentType :
                        ntipo == 1 ? IntercommResource_AgentType :
                        (ntipo < 5 || ntipo == 13) ? LegacyPhoneResource_AgentType : ATSPhoneResource_AgentType;

                    SlotRecursoTipoAgenteSet(gw, rec, TipoNotificado);
                    /*
                            rcRadio = 0, 
                            rcLCE = 1, 
                            rcPpBC = 2, 
                            rcPpBL = 3, 
                            rcPpAB = 4, 
                            rcAtsR2 = 5, 
                            rcAtsN5 = 6, 
                            rcPpEM = 13, 
                            rcPpEMM = 51, 
                            rcNotipo = -1 
                     * */
                    SlotRecursoTipoInterfazSet(gw, rec, ntipo);

                    int estado = snmpc.Integer(vOut[2].Data);   // 0: NP, 1: OK, 2: Fallo, 3: Degradado
                    SlotRecursoEstadoSet(gw, rec, estado, (trc)TipoNotificado);
                }
                else if (ntipo != 9 && ntipo != -1)
                {
                    LogError<GwExplorer>(String.Format("Error Explorando Recurso {0}:{1}: Tipo Notificado <{2}> Erroneo.",
                                gw.ip, nres, ntipo));
                }
            }
            catch (Exception x)
            {
                LogException<GwExplorer>(String.Format(" Explorando recurso en {0}: Rec:{1}-{2}", gw.ip, nres, rec.name), x);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="nslot"></param>
        /// <param name="rec"></param>
        protected void Reset_ExploraRecurso(stdPhGw gw, int nslot, int rec)
        {
            SlotRecursoTipoAgenteSet(gw, gw.slots[nslot].rec[rec], (int)trc.rcNotipo);
            SlotRecursoTipoInterfazSet(gw, gw.slots[nslot].rec[rec], (int)itf.rcNotipo);
            SlotRecursoEstadoSet(gw, gw.slots[nslot].rec[rec], (int)std.NoInfo, trc.rcNotipo);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pgw"></param>
        protected async void GetVersion_unificada(stdPhGw pgw)
        {
            if (pgw.version == string.Empty || pgw.version == idiomas.strings.GWS_VersionError/*"Error en GetVersion_unificada"*/)
            {
                try
                {
                    string page = "http://" + pgw.ip + ":8080/mant/lver";

                    // ... Use HttpClient.
                    using (HttpClient client = new HttpClient())
                    using (HttpResponseMessage response = await client.GetAsync(page))
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = await content.ReadAsStringAsync();
                        pgw.version = result;
                    }
                }
                catch (Exception x)
                {
                    if (pgw.version != idiomas.strings.GWS_VersionError/*"Error en GetVersion_unificada"*/)
                        LogException<GwExplorer>(String.Format(" GW:{0},{1}", pgw.name, pgw.ip), x);
                    pgw.version = idiomas.strings.GWS_VersionError/*"Error en GetVersion_unificada"*/;
                }
            }
        }
        protected async void GetNtpStatus(stdGw gw)
        {
            try
            {
                string page = "http://" + gw.ip + ":8080/ntpstatus";
                // ... Use HttpClient.
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(page))
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    string result = await content.ReadAsStringAsync();
                    gw.ntp_client_status = (U5kManWebAppData.JDeserialize<stdGw.RemoteNtpClientStatus>(result)).lines;
                }
            }
            catch (Exception x)
            {
                LogException<GwExplorer>(String.Format(" GW:{0},{1}", gw.name, gw.ip), x);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="pgw"></param>
        /// <param name="oidEnt"></param>
        /// <param name="oidvar"></param>
        static public void RecibidoTrapGw_unificada(stdGw gw, stdPhGw pgw, string oidEnt, string oidvar, ISnmpData data)
        {
            switch (oidEnt)
            {
                case ".1.3.6.1.4.1.7916.8.3.2.1.1":         // Cambio de Configuracion.
                    break;

                case ".1.3.6.1.4.1.7916.8.3.2.1.2":         // Cambio de Estado.
                    break;

                case ".1.3.6.1.4.1.7916.8.3.2.1.3":         // Se genera cuando cambia un parametro del grupo tarjeta
                    break;

                case ".1.3.6.1.4.1.7916.8.3.2.1.4":         // Se genera cuando cambia parametro del grupo interfaz
                    break;

                case ".1.3.6.1.4.1.7916.8.3.2.1.5":         // Evento de Historicos.
                    if (oidvar == ".1.3.6.1.4.1.7916.8.3.2.1.7.0")
                    {
                        LogTrace<GwExplorer>(String.Format("GWU-HISTORICO: <<<{0}>>>", data.ToString())
                            );

                        Redan2UlisesHist conv = new Redan2UlisesHist(data.ToString());
                        U5kIncidencia inci;
                        List<Object> parametros;
                        if (conv.UlisesInci(out inci, out parametros))
                        {
                            RecordEvent<GwExplorer>(DateTime.Now, (eIncidencias)inci.id, (eTiposInci)inci.tipo, inci.idhw, parametros.ToArray());
                        }
                        else
                        {
                            LogWarn<GwExplorer>(String.Format("GWU-HISTORICO NO CONVERTIDO: <<<{0}>>>", data.ToString()));
                        }
                    }
                    break;

                default:
                    LogTrace<GwExplorer>(String.Format("Recibido TRAP-GW OID-Desconocida de {0}, OID={1}", gw.ip, oidEnt));
                    break;
            }
        }

        #endregion //

        #region GW_STD_V1
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        protected void ExplorePhGw(object obj)
        {
            // Obtengo una copia del estado de la pasarela.
            stdPhGw phgw = new stdPhGw();
            phgw.CopyFrom((stdPhGw)obj);

            try
            {
                // Ping para la conectividad....
                phgw.stdIpConn = U5kGenericos.Ping(phgw.ip, phgw.presente) == true ? std.Ok : std.NoInfo;
                if (phgw.stdIpConn == std.Ok)
                {
                    // Supervision del Modulo SIP...
                    SipModuleTest(phgw);
                    if (phgw.stdSipMod == std.Ok)
                    {
                        // Supervision del Modulo de Configuracion...
                        CfgModuleTest(phgw);
                        // Supervision del Modulo SNMP....
                        SnmpModuleExplore(phgw);
                    }
                }
            }
            catch (Exception x)
            {
                phgw.stdIpConn = std.NoInfo;
                LogException<GwExplorer>(phgw.name, x);
            }
            finally
            {
                ConsolidateData((stdPhGw)obj, phgw);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phgw"></param>
        protected void SipModuleTest(stdPhGw phgw)
        {
            try
            {

                SipUA locale_ua = new SipUA() { user = "MTTO", ip = Properties.u5kManServer.Default.MiDireccionIP, port = 0 };
                SipUA remote_ua = new SipUA() { user = phgw.name, ip = phgw.ip, port = 5060, radio = true };
                SipSupervisor sips = new SipSupervisor(locale_ua);
                if (sips.SipPing(remote_ua))
                {
                    phgw.stdSipMod = remote_ua.last_response != null &&
                        (remote_ua.last_response.Result == "200" || remote_ua.last_response.Result == "503") ? std.Ok : std.Error;
                }
                else
                {
                    phgw.stdSipMod = std.NoInfo;
                }
            }
            catch (Exception x)
            {
                // Error en los OPTIONS...
                phgw.stdSipMod = std.NoInfo;
                LogException<GwExplorer>(phgw.name, x);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phgw"></param>
        protected /*async*/ void CfgModuleTest(stdPhGw phgw)
        {
            try
            {
                string page = "http://" + phgw.ip + ":8080/test";

                // ... Use HttpClient.
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = /*await */client.GetAsync(page).Result)
                using (HttpContent content = response.IsSuccessStatusCode ? response.Content : null)
                {
                    if (content != null)
                    {
                        // ... Read the string.
                        string result = /*await */content.ReadAsStringAsync().Result;
                        phgw.stdCfgMod = result.Contains("Handler por Defecto") ? std.Ok : std.Error;
                    }
                    else
                    {
                        phgw.stdCfgMod = std.NoInfo;
                    }
                }

            }
            catch (Exception x)
            {
                // Error en Modulo de Configuracion Local...
                phgw.stdCfgMod = std.NoInfo;
                LogException<GwExplorer>(phgw.name, x);
            }
            finally
            {
                GetVersion_unificada(phgw);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phgw"></param>
        protected void SnmpModuleExplore(stdPhGw phgw)
        {
            try
            {
                ExploraGwStdGen_unificada(phgw);
                phgw.stdSnmpMod = std.Ok;
                for (int slot = 0; slot < 4; slot++)
                {
                    ExploraSlot_unificada(new KeyValuePair<stdPhGw, int>(phgw, slot));
                }
            }
            catch (Exception x)
            {
                // Error en la Exploracion SNMP....
                phgw.stdSnmpMod = std.NoInfo;
                phgw.SnmpDataReset();
                LogException<GwExplorer>(phgw.name, x);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="last"></param>
        /// <param name="current"></param>
        protected void ConsolidateData(stdPhGw last, stdPhGw current)
        {
            try
            {
                // Calcular Presente....
                current.presente = (current.stdIpConn == std.Ok && current.stdSipMod != std.NoInfo);
                // Calcular Estado General.... En current.std se encuentra el estado leido. last.std debe tener tambien los errores de recurso...
                current.std = current.presente == false ? std.NoInfo : current.Errores == true ? std.Error : std.Ok;

                // Historicos de Activacion / Desactivacion de Modulos...
                if (current.stdCfgMod != last.stdCfgMod)
                {
                    eIncidencias inci = current.stdCfgMod == std.NoInfo ? eIncidencias.IGW_CAIDA : eIncidencias.IGW_ENTRADA;
                    RecordEvent<GwExplorer>(DateTime.Now, inci, eTiposInci.TEH_TIFX, current.name, Params(idiomas.strings.GW_CFGL_MODULE));
                }
                if (current.stdSipMod != last.stdSipMod)
                {
                    eIncidencias inci = current.stdSipMod == std.NoInfo ? eIncidencias.IGW_CAIDA : eIncidencias.IGW_ENTRADA;
                    RecordEvent<GwExplorer>(DateTime.Now, inci, eTiposInci.TEH_TIFX, current.name, Params(idiomas.strings.GW_SIP_MODULE));
                }
                if (current.stdSnmpMod != last.stdSnmpMod)
                {
                    eIncidencias inci = current.stdSnmpMod == std.NoInfo ? eIncidencias.IGW_CAIDA : eIncidencias.IGW_ENTRADA;
                    RecordEvent<GwExplorer>(DateTime.Now, inci, eTiposInci.TEH_TIFX, current.name, Params(idiomas.strings.GW_SNMP_MODULE));
                }

                // Genera historicos de activacion / desactivacion de la pasarela...
                if (current.presente != last.presente)
                {
                    eIncidencias inci = current.presente == false ? eIncidencias.IGW_CAIDA : eIncidencias.IGW_ENTRADA;
                    RecordEvent<GwExplorer>(DateTime.Now, inci, eTiposInci.TEH_TIFX, current.name, Params(idiomas.strings.GW_GLOBAL_MODULE));

                    if (current.presente == false)
                    {
                        /** Reset Estado GW fisica */
                        GwHelper.SetToOutOfOrder(current);
                    }
                }
            }
            catch (Exception x)
            {
                // Error en la consolidacion.
                LogException<GwExplorer>(last.name, x);
            }
            finally
            {
                last.CopyFrom(current);
            }
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gw"></param>
        static protected void GwTrace(stdGw gw)
        {
            NLog.LogLevel level = NLog.LogLevel.Info;

            Log<GwExplorer>(level, String.Format("Name={0}, IP={1}, Presente={2}, Estado={3}", gw.name, gw.ip, gw.presente, gw.std), eIncidencias.IGNORE);
            // String slots = String.Format("{0},[{1}{2}{3}{4}] 
        }

        #endregion
    }   // clase
} // namespace.