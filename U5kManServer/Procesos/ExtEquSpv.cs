﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using Utilities;

namespace U5kManServer.ExtEquSpvSpace
{
    class ExtEquSpv : NucleoGeneric.NGThread/*, IDisposable*/
    {
        /// <summary>
        /// 
        /// </summary>
        public ExtEquSpv()
        {
            Name = "ExtEquSpv";
            /** 20180709. Peticion #3632 */
            AllowedSipResponses = new List<string>();
            if (Properties.u5kManServer.Default.AllowedResponsesToSipOptions != null)
            {
                foreach (var item in Properties.u5kManServer.Default.AllowedResponsesToSipOptions)
                    AllowedSipResponses.Add(item);
            }

            local_ua = new SipUA() { user = "MTTO", ip = Properties.u5kManServer.Default.MiDireccionIP, port = 7060 };
            sips = new SipSupervisor(local_ua);
            sips.NotifyException += (ua, x) =>
            {
                LogException<SipSupervisor>("Supervisando Equipo externo " + ua.uri, x);
            };
        }
        /// <summary>
        /// 
        /// </summary>
        protected void LocalDispose()
        {
            sips.Dispose();
            LogDebug<ExtEquSpv>("ExtEquSpv Dispose...");
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void Run()
        {
            U5kGenericos.TraceCurrentThread(this.GetType().Name);

            Decimal interval = Properties.u5kManServer.Default.SpvInterval;
            using (timer = new TaskTimer(new TimeSpan(0, 0, 0, 0, Decimal.ToInt32(interval)), this.Cancel))
            {
                while (IsRunning())
                {
                    try
                    {
                        if (U5kManService._Master == true)
                        {
                            Utilities.TimeMeasurement tm = new Utilities.TimeMeasurement("EXT Explorer");

                            // Copia de equipo configurados.
                            List<EquipoEurocae> localequ = new List<EquipoEurocae>();
                            GlobalServices.GetWriteAccess((gdata) =>
                            {
                                gdata.STDEQS.ForEach(equ =>
                                {
                                    localequ.Add(new EquipoEurocae(equ));
                                });
                                // Supervision de Destinos ATS Externos. TODO....
                                // stdeqeu.AddRange(U5kManService._std.atsDestStd.Equipos);
                            });

                            // Arranco las tareas de exploracion...
                            List<Task> task = new List<Task>();

                            localequ.ForEach(equipo =>
                            {
                                task.Add(Task.Factory.StartNew(() =>
                                {
                                    U5kGenericos.TraceCurrentThread(this.GetType().Name + " " + equipo.Id);
                                    try
                                    {
                                        SupervisaEquipo(equipo);
                                    }
                                    catch (Exception x)
                                    {
                                        LogException<ExtEquSpv>("Supervisando Equipo externo " + equipo.Id, x);
                                    }
                                }, TaskCreationOptions.LongRunning));
                            });
                            /// Espero que todos los procesos acaben...
                            Task.WaitAll(task.ToArray(), 9000);
                            // Actualizo los datos..
                            GlobalServices.GetWriteAccess((gdata) =>
                            {
                                localequ.ForEach(eq =>
                                {
                                    if (gdata.EQUDIC.ContainsKey(eq.Key))
                                    {
                                        gdata.EQUDIC[eq.Key].CopyFrom(eq);
                                    }
                                });

                                SetEstadoGlobalEquipos(gdata, localequ);
                            });


                            tm.StopAndPrint((msg) =>
                            {
                                LogTrace<ExtEquSpv>(msg);
                            });
                        }
                    }
                    catch (Exception x)
                    {
                        if (x is ThreadAbortException)
                        {
                            Thread.ResetAbort();
                            break;
                        }
                        LogException<ExtEquSpv>("SupervisaEquiposExternos", x);
                    }
                    GoToSleepInTimer();
                }
            }
            LocalDispose();
            Dispose();
            LogInfo<ExtEquSpv>("Finalizado...");
        }
        /// <summary>
        /// 
        /// </summary>
        void SetEstadoGlobalEquipos(U5kManStdData gdata, List<EquipoEurocae> stdeqeu)
        {
            int equipos = stdeqeu.Count;
            int equipos_presentes = stdeqeu.Where(e => e.EstadoRed1 == std.Ok).ToList().Count;
            int equipos_error = stdeqeu.Where(e => e.EstadoRed1 == std.Ok && e.EstadoSip == std.Error).ToList().Count;
            int equipos_aviso = stdeqeu.Where(e => e.EstadoRed1 == std.Ok && e.EstadoSip == std.Aviso).ToList().Count;

            U5KStdGeneral gen = gdata.STDG;
            gen.stdGlobalExt =
                equipos_presentes == 0 ? std.NoInfo :
                equipos_error != 0 ? std.Error :
                equipos_aviso != 0 ? std.Aviso :
                equipos_presentes == equipos ? std.Ok : std.Aviso;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipo"></param>
        void SupervisaEquipo(EquipoEurocae equipo)
        {
            /** */
            std last = equipo.EstadoGeneral;

            SipUA ua = new SipUA()
            {
                user = equipo.sip_user,
                ip = equipo.Ip1,
                port = equipo.sip_port,
                radio = (equipo.Tipo == 2)
            };

            bool ping1 = U5kGenericos.Ping(equipo.Ip1, equipo.EstadoRed1 == std.Ok);
            bool ping2 = U5kGenericos.Ping(equipo.Ip2, equipo.EstadoRed2 == std.Ok);

            /** Estado Conectividad */
            equipo.EstadoRed1 = ChangeStd(equipo, ping1 == true ? std.Ok : std.NoInfo); /** Provocará el histórico */
            equipo.EstadoRed2 = ping2 == true ? std.Ok : std.NoInfo;

            /** Estado Agente SIP */
            if (equipo.Tipo == 5)
            {
                /** Los Grabadores no tienen Agente SIP, Para que se muestre Ok, 
                    Ponemos que está bien */
                equipo.EstadoSip = std.Ok;
            }
            else
            {
                bool sipp = (sips != null && ping1 == true) ? sips.SipPing(ua) : false;
                if (equipo.EstadoRed1 == std.Ok)
                {
                    if (sips != null && sipp == true)
                    {
                        /** 20180709. Peticion #3632 */
                        if (ua.last_response != null)
                        {
                            var allowedReponse = AllowedSipResponses.Contains(ua.last_response.Result);
                            equipo.EstadoSip = allowedReponse ? std.Ok : std.Aviso;
                            equipo.LastOptionsResponse = ua.last_response.Result;
                        }
                        //if (ua.last_response != null && ua.last_response.Result == "200")
                        //{
                        //    equipo.EstadoSip = std.Ok;
                        //}
                        //else if (ua.last_response != null && ua.last_response.Result == "405")  // El equipo no tiene implementado OPTIONS, pero RESPONDE.
                        //{
                        //    /** 20180308. Se Solicita que este caso sea OK */
                        //    // equipo.EstadoSip = std.Aviso;
                        //    equipo.EstadoSip = std.Ok;
                        //}
                        else
                        {
                            equipo.EstadoSip = std.Error;
                            equipo.LastOptionsResponse = "";
                        }
                    }
                    else
                    {
                        equipo.EstadoSip = std.Error;
                        equipo.LastOptionsResponse = "";
                    }
                }
                else
                {
                    equipo.EstadoSip = std.NoInfo;
                    equipo.LastOptionsResponse = "";
                }
            }

            ///** Ha cambiado el estado. */
            //if (last != equipo.EstadoGeneral)
            //{
            //    U5kEstadisticaProc.Estadisticas.EventoExterno(equipo.sip_user ?? equipo.Id, equipo.EstadoGeneral != std.NoInfo);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="nuevo"></param>
        /// <returns></returns>
        private std ChangeStd(EquipoEurocae equipo, std nuevo)
        {
            if (equipo.EstadoRed1 != nuevo)
            {
                /** Generar evento */
                U5kBaseDatos.eTiposInci tinci = equipo.Tipo == 2 ? U5kBaseDatos.eTiposInci.TEH_EXTERNO_RADIO :
                    equipo.Tipo == 3 ? U5kBaseDatos.eTiposInci.TEH_EXTERNO_TELEFONIA :
                    equipo.Tipo == 5 ? U5kBaseDatos.eTiposInci.TEH_RECORDER : U5kBaseDatos.eTiposInci.TEH_SISTEMA;
                string id = equipo.Tipo == 5 ? equipo.Id : equipo.sip_user;
                RecordEvent<ExtEquSpv>(DateTime.Now,
                    nuevo == std.Ok ? U5kBaseDatos.eIncidencias.IEE_ENTRADA :
                    U5kBaseDatos.eIncidencias.IEE_CAIDA,
                    tinci,
                    id, Params());
            }
            return nuevo;
        }

        private SipUA local_ua = null;
        private SipSupervisor sips = null;
        private List<string> AllowedSipResponses = null;
    }

}
