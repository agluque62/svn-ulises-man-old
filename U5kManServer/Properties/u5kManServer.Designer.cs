﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace U5kManServer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.8.0.0")]
    internal sealed partial class u5kManServer : global::System.Configuration.ApplicationSettingsBase {
        
        private static u5kManServer defaultInstance = ((u5kManServer)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new u5kManServer())));
        
        public static u5kManServer Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>Top:.1.1.1000.0</string>
  <string>EstadoTop:.1.1.1000.0</string>
  <string>EstadoAltavozRadio:.1.1.1000.1.2.0</string>
  <string>EstadoAltavozLC:.1.1.1000.1.2.1</string>
  <string>EstadoJacksEjecutivo:.1.1.1000.1.3.0</string>
  <string>EstadoJacksAyudante:.1.1.1000.1.3.1</string>
  <string>EstadoPtt:.1.1.1000.2</string>
  <string>EstadoPanel:.1.1.1000.1.4</string>
  <string>EstadoLan1:.1.1.1000.3.1</string>
  <string>EstadoLan2:.1.1.1000.3.2</string>
  <string>SeleccionPaginaRadio:.1.1.1000.6</string>
  <string>LlamadaSaliente:.1.1.1000.7</string>
  <string>LlamadaEntrante:.1.1.1000.9</string>
  <string>LlamadaEstablecida:.1.1.1000.11</string>
  <string>LlamadaFinaliza:.1.1.1000.10</string>
  <string>FacilidadTelefonia:.1.1.1000.8</string>
  <string>Briefing:.1.1.1000.12</string>
  <string>Replay:.1.1.1000.13</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection TopOids {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["TopOids"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>ED137NombreSistema:.1.3.6.1.2.1.1.2.0</string>
  <string>ED137Descripcion:.1.3.6.1.2.1.1.1.0</string>
  <string>ED137NumeroItfs:.1.3.6.1.2.1.2.1.0</string>
  <string>ED137TablaItfs:.1.3.6.1.2.1.2.2.1</string>
  <string>U5KDualidad:.1.3.6.1.4.1.7916.8.1.1.1.0</string>
  <string>U5KServidorDual:.1.3.6.1.4.1.7916.8.1.1.2.0</string>
  <string>U5KScvA:.1.3.6.1.4.1.7916.8.1.1.3.0</string>
  <string>U5KScvB:.1.3.6.1.4.1.7916.8.1.1.4.0</string>
  <string>U5KServ1:.1.3.6.1.4.1.7916.8.1.1.5.0</string>
  <string>U5KServ2:.1.3.6.1.4.1.7916.8.1.1.6.0</string>
  <string>U5KNtp:.1.3.6.1.4.1.7916.8.1.1.7.0</string>
  <string>U5KSacta1:.1.3.6.1.4.1.7916.8.1.1.8.0</string>
  <string>U5KSacta2:.1.3.6.1.4.1.7916.8.1.1.9.0</string>
  <string>U5KNPuestos:.1.3.6.1.4.1.7916.8.1.2.1.0</string>
  <string>U5KPuestos:.1.3.6.1.4.1.7916.8.1.2.2.1</string>
  <string>U5KNRadios:.1.3.6.1.4.1.7916.8.1.3.1.0</string>
  <string>U5KRadios:.1.3.6.1.4.1.7916.8.1.3.2.1</string>
  <string>U5KNLineas:.1.3.6.1.4.1.7916.8.1.4.1.0</string>
  <string>U5KLineas:.1.3.6.1.4.1.7916.8.1.4.2.1</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ScvOids {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ScvOids"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool GenerarHistoricos {
            get {
                return ((bool)(this["GenerarHistoricos"]));
            }
            set {
                this["GenerarHistoricos"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("departamento")]
        public string stringSistema {
            get {
                return ((string)(this["stringSistema"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SistemaDual {
            get {
                return ((bool)(this["SistemaDual"]));
            }
            set {
                this["SistemaDual"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ServidorDual {
            get {
                return ((bool)(this["ServidorDual"]));
            }
            set {
                this["ServidorDual"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HaySacta {
            get {
                return ((bool)(this["HaySacta"]));
            }
            set {
                this["HaySacta"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool HayReloj {
            get {
                return ((bool)(this["HayReloj"]));
            }
            set {
                this["HayReloj"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("64")]
        public int LineasIncidencias {
            get {
                return ((int)(this["LineasIncidencias"]));
            }
            set {
                this["LineasIncidencias"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("es")]
        public string Idioma {
            get {
                return ((string)(this["Idioma"]));
            }
            set {
                this["Idioma"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("365")]
        public int DiasEnHistorico {
            get {
                return ((int)(this["DiasEnHistorico"]));
            }
            set {
                this["DiasEnHistorico"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.0.129")]
        public string MiDireccionIP {
            get {
                return ((string)(this["MiDireccionIP"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        public string FiltroPOS {
            get {
                return ((string)(this["FiltroPOS"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        public string FiltroGW {
            get {
                return ((string)(this["FiltroGW"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>EstadoGw:.1.1.100.2.0</string>
  <string>TipoSlot0:.1.1.100.31.1.1.0</string>
  <string>TipoSlot1:.1.1.100.31.1.1.1</string>
  <string>TipoSlot2:.1.1.100.31.1.1.2</string>
  <string>TipoSlot3:.1.1.100.31.1.1.3</string>
  <string>EstadoSlot0:.1.1.100.31.1.2.0</string>
  <string>EstadoSlot1:.1.1.100.31.1.2.1</string>
  <string>EstadoSlot2:.1.1.100.31.1.2.2</string>
  <string>EstadoSlot3:.1.1.100.31.1.2.3</string>
  <string>PrincipalReserva:.1.1.100.21.0</string>
  <string>EstadoLan:.1.1.100.22.0</string>
  <string>TipoRecurso:.1.1.100.100.0</string>
  <string>TipoRecursoRadio:.1.1.200</string>
  <string>EstadoRecursoRadio:.1.1.200.2.0</string>
  <string>TipoRecursoLC:.1.1.300</string>
  <string>EstadoRecursoLC:.1.1.300.2.0</string>
  <string>TipoRecursoTF:.1.1.400</string>
  <string>EstadoRecursoTF:.1.1.400.2.0</string>
  <string>TipoRecursoATS:.1.1.500</string>
  <string>EstadoRecursoATS:.1.1.500.2.0</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection GwOids {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["GwOids"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>S:.1.1.200.3.1</string>
  <string>0:.1.1.200.3.1.13.1</string>
  <string>1:.1.1.200.3.1.17.1</string>
  <string>2:.1.1.200.3.1.18.1</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection RadioOids {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RadioOids"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>S:.1.1.300.3.1</string>
  <string>0:.1.1.300.3.1.9.1</string>
  <string>1:.1.1.300.3.1.13.1</string>
  <string>2:.1.1.300.3.1.14.1</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection LcenOids {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["LcenOids"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>S:.1.1.400.3.1</string>
  <string>0:.1.1.400.3.1.10.1</string>
  <string>1:.1.1.400.3.1.14.1</string>
  <string>2:.1.1.400.3.1.15.1</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection TelefOids {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["TelefOids"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>S:.1.1.500.3.1</string>
  <string>0:.1.1.500.3.1.13.1</string>
  <string>1:.1.1.500.3.1.17.1</string>
  <string>2:.1.1.500.3.1.18.1</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AtsOids {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AtsOids"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1022")]
        public int nbxSupPort {
            get {
                return ((int)(this["nbxSupPort"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("224.100.10.1")]
        public string MainStanbyMcastAdd {
            get {
                return ((string)(this["MainStanbyMcastAdd"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int MainStandByMcastPort {
            get {
                return ((int)(this["MainStandByMcastPort"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SonidoAlarmas {
            get {
                return ((bool)(this["SonidoAlarmas"]));
            }
            set {
                this["SonidoAlarmas"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("161")]
        public int TopSnmpPort {
            get {
                return ((int)(this["TopSnmpPort"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("m")]
        public string GwFtpUser {
            get {
                return ((string)(this["GwFtpUser"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("m")]
        public string GwFtpPwd {
            get {
                return ((string)(this["GwFtpPwd"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".1.1.100.1.")]
        public string HfEventOids {
            get {
                return ((string)(this["HfEventOids"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".1.1.600.1")]
        public string CfgEventOid {
            get {
                return ((string)(this["CfgEventOid"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8080")]
        public string PabxWsPort {
            get {
                return ((string)(this["PabxWsPort"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sa")]
        public string PabxSaPwd {
            get {
                return ((string)(this["PabxSaPwd"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool PabxSimulada {
            get {
                return ((bool)(this["PabxSimulada"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.0.212")]
        public string MySqlServer {
            get {
                return ((string)(this["MySqlServer"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("d:\\datos\\Empresa\\_SharedPrj\\UlisesV5000i-MN\\ulises-man\\uv5ki-06-gwu.db3")]
        public string SQLitePath {
            get {
                return ((string)(this["SQLitePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int TipoBdt {
            get {
                return ((int)(this["TipoBdt"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int TipoWeb {
            get {
                return ((int)(this["TipoWeb"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://{0}:1023/")]
        public string strFormatNbxUrl {
            get {
                return ((string)(this["strFormatNbxUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://{0}/pbx/")]
        public string strFormatPabxUrl {
            get {
                return ((string)(this["strFormatPabxUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HtmlEncode {
            get {
                return ((bool)(this["HtmlEncode"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HayAltavozHF {
            get {
                return ((bool)(this["HayAltavozHF"]));
            }
            set {
                this["HayAltavozHF"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>1021</string>
  <string>1022</string>
  <string>2050</string>
  <string>2051</string>
  <string>2052</string>
  <string>2053</string>
  <string>2300</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection FiltroIncidencias {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FiltroIncidencias"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int TopOidsShemaVersion {
            get {
                return ((int)(this["TopOidsShemaVersion"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool GwsUnificadas {
            get {
                return ((bool)(this["GwsUnificadas"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("161")]
        public int Snmp_AgentPort {
            get {
                return ((int)(this["Snmp_AgentPort"]));
            }
            set {
                this["Snmp_AgentPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("162")]
        public int Snmp_AgentListenTrapPort {
            get {
                return ((int)(this["Snmp_AgentListenTrapPort"]));
            }
            set {
                this["Snmp_AgentListenTrapPort"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8090")]
        public int WebserverPort {
            get {
                return ((int)(this["WebserverPort"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1021")]
        public int SyncserverPort {
            get {
                return ((int)(this["SyncserverPort"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public decimal SpvInterval {
            get {
                return ((decimal)(this["SpvInterval"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("root")]
        public string MySqlUser {
            get {
                return ((string)(this["MySqlUser"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("cd40")]
        public string MySqlPwd {
            get {
                return ((string)(this["MySqlPwd"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Historico_PttSqhOnBdt {
            get {
                return ((bool)(this["Historico_PttSqhOnBdt"]));
            }
            set {
                this["Historico_PttSqhOnBdt"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("new_cd40")]
        public string BdtSchema {
            get {
                return ((string)(this["BdtSchema"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("344005")]
        public string PabxSimuladaRegisterSubcriber {
            get {
                return ((string)(this["PabxSimuladaRegisterSubcriber"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("41000")]
        public string PabxSimuladaUnregisterSubcriber {
            get {
                return ((string)(this["PabxSimuladaUnregisterSubcriber"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ExternalEquipmentUnassigned {
            get {
                return ((bool)(this["ExternalEquipmentUnassigned"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("v2")]
        public string Snmp_AgentVersion {
            get {
                return ((string)(this["Snmp_AgentVersion"]));
            }
            set {
                this["Snmp_AgentVersion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("public")]
        public string Snmp_V2AgentGetComm {
            get {
                return ((string)(this["Snmp_V2AgentGetComm"]));
            }
            set {
                this["Snmp_V2AgentGetComm"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("public")]
        public string Snmp_V2AgentSetComm {
            get {
                return ((string)(this["Snmp_V2AgentSetComm"]));
            }
            set {
                this["Snmp_V2AgentSetComm"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection Snmp_V3Users {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["Snmp_V3Users"]));
            }
            set {
                this["Snmp_V3Users"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-8\" ?><NicEventMonitorConfig ><TeamingType>Intel" +
            "</TeamingType><WindowsLog>System</WindowsLog><EventSource>iANSMiniport</EventSou" +
            "rce><UpEventId>15</UpEventId><DownEventId>11</DownEventId></NicEventMonitorConfi" +
            "g>   ")]
        public string TeamingConfig {
            get {
                return ((string)(this["TeamingConfig"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int NtpClient {
            get {
                return ((int)(this["NtpClient"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int ClusterPollingMethod {
            get {
                return ((int)(this["ClusterPollingMethod"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("500")]
        public int ClusterMethodUdpRequestTimeout {
            get {
                return ((int)(this["ClusterMethodUdpRequestTimeout"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int SnmpClientRequestReint {
            get {
                return ((int)(this["SnmpClientRequestReint"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int SnmpClientRequestTimeout {
            get {
                return ((int)(this["SnmpClientRequestTimeout"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int LogRepeatFilterTime {
            get {
                return ((int)(this["LogRepeatFilterTime"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int LogRepeatSupervisionTime {
            get {
                return ((int)(this["LogRepeatSupervisionTime"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>200</string>\r\n  <string>405</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AllowedResponsesToSipOptions {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AllowedResponsesToSipOptions"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int StatisticsActivityMonitoringTime {
            get {
                return ((int)(this["StatisticsActivityMonitoringTime"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool OpcOpCableGrabacion {
            get {
                return ((bool)(this["OpcOpCableGrabacion"]));
            }
            set {
                this["OpcOpCableGrabacion"] = value;
            }
        }
    }
}
