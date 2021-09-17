﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

using U5kBaseDatos;

namespace UnitTesting
{
    [TestClass]
    public class GwCallLogsParseTest
    {
        /**
         * Formato de la Incidencia
         * CODE:FECHA-HORA:SITE:IDHW:???:USER:PARAM1:PARAM2:....
         * Formato PARAMS Segun Tipo.
         *      Eventos y Operaciones:
         *          PARAM-INCI = PARAM1 + PARAM2 + ...
         *      Llamadas:
         *          REC-ID:KEY1=VALUE1|KEY2=VALUE2|... :PARAM1:PARAM2:...
         *      Normal:
         *          PARAM1, PARAM2, ...
         * */
        readonly List<string> IncisFromGateways = new List<string>()
        {
            //"2007:2015-11-06 11.07.47:EMPLAZ:CGW3:31:-",
            //"2005:2015-11-06 11.07.39:EMPLAZ:CGW3:19:-:192.168.0.223:Principal:Reserva",
            //"2007:2015-11-06 11.07.47:EMPLAZ:CGW3:31:-:TX1",
            //"2008:2015-11-06 11.07.43:EMPLAZ:CGW3:23:-:TX1",
            //"2009:2015-11-06 11.07.44:EMPLAZ:CGW3:26:-:(AB) AB-2",
            //"2010:2015-11-06 11.07.44:EMPLAZ:CGW3:24:-:(AB) AB-2",
            //"2101:2015-11-06 11.07.50:EMPLAZ:CGW3:33:-:RX1:ON:sip.UV5KI@10.12.60.133:Activa",
            //"2204:2015-11-06 10.58.11:EMPLAZ:CGW3:16:-:R2-1",
            //"2311:2015-11-06 10.58.12:EMPLAZ:CGW3:17:-:AB-1:IF=4|DS=RTB:06/11/2015 10.58.12",
            //"2312:2015-11-06 11.02.52:EMPLAZ:CGW3:18:-:AB-1:IF=4|DS=RTB|DL=0000280:06/11/2015 11.02.52",

            //"2204:2015-11-06 10.58.11:EMPLAZ:CGW3:16:-:r2-1",
            //"2205:2015-11-06 10.58.11:EMPLAZ:CGW3:16:-:r2-1",
            //"2207:2015-11-06 10.58.11:EMPLAZ:CGW3:16:-:n2-1",
            //"2208:2015-11-06 10.58.11:EMPLAZ:CGW3:16:-:n2-1",
            //"2304:2021-06-16 07.57.48:EMPLAZ:CGW3:39:-:n5-n2:RC=n5-n2:16/06/2021 07.57.48",
            "2022:2021-09-17 05.39.36:EMPLAZ:CGW1:53:-:",
            "2021:2021-09-17 10.40.00:EMPLAZ:CGW1:54:-:"
        };
        void PrepareTest(Action<dynamic> take)
        {
            take(new { GwsDatesAreUtc = true, GwsHistMaxSecondsInAdvance = 5, GwsHistMaxHoursDelayed = 12, pgw=new { ip = "127.0.0.1" } });
        }
        [TestMethod]
        public void TestMethod1()
        {
            PrepareTest((settings) =>
            {
                IncisFromGateways.ForEach(inciText =>
                {
                    new Redan2UlisesHist(inciText).UlisesInci((ok, date, inci, parametros) =>
                    {
                        if (ok)
                        {
                            //var settings = Properties.u5kManServer.Default;
                            var workingDate = settings.GwsDatesAreUtc ? date.ToLocalTime() : date;
                            var deviation = DateTime.Now - workingDate;

                            if (deviation < TimeSpan.FromSeconds(-settings.GwsHistMaxSecondsInAdvance) ||
                                deviation > TimeSpan.FromHours(settings.GwsHistMaxHoursDelayed))
                            {
                                Debug.WriteLine($"GW-HISTORICO NO SINCRONIZADO: De {settings.pgw.ip}, " +
                                    $"UTC date => {date}, Local date => {DateTime.Now}, " +
                                    $"Inci => {inci}");
                            }
                            else
                            {

                                Debug.WriteLine($"RecordEvent At {workingDate} => {inci}");
                            }
                        }
                        else
                            Debug.WriteLine(String.Format("GWU-HISTORICO NO CONVERTIDO: <<<{0}>>>", inciText));
                    });
                });

            });
        }
    }
}
