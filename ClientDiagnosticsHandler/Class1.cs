using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.Collections.Specialized;
using System.IO;

namespace ClientDiagnosticsHandler
{
    public class SyncInfo
    {
        public void SetMDCState(String s){ MDCState = int.Parse(s); }

        public void SetCCState(String s){ CCState = int.Parse(s); }

        public int MDCState;
        public int CCState;

        public bool IsMDCFailed() { return MDCState < 0; }
        public bool IsCCFailed() { return MDCState > 100; }
    }

    public class ClientDiagnosticsModule : IHttpModule
    {
        public ClientDiagnosticsModule() { }

        public string ModuleName { get { return "ClientDiagnosticsModule"; } }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
        }

        private void CreateFilesIfNeeded()
        {
            if (!File.Exists("C:\\Sync_Successes_List.txt"))
            {
                // Create it
                File.Create("C:\\Sync_Successes_List.txt").Close();
            }

            if (!File.Exists("C:\\Sync_Failures_List.txt"))
            {
                // Create it
                File.Create("C:\\Sync_Failures_List.txt").Close();
            }
        }

        private void WriteFailure(String logLine)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Sync_Failures_List.txt", true);
            file.WriteLine(logLine);
            file.Close();
        }

        private void WriteSuccess(String logLine)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Sync_Successes_List.txt", true);
            file.WriteLine(logLine);
            file.Close();
        }

        private string MapMDCStateToString(int state)
        {
            switch (state)
            {
                case -9:
                    return "Archive Changed While Sync In Progress";
                case -8:
                    return "Server Syncing";
                case -7:
                    return "Sync Prevented";
                case -6:
                    return "Insufficient Disk";
                case -5:
                    return "Failed Slot";
                case -4:
                    return "Could Not Connect To EV Web Server Virtual Dir";
                case -3:
                    return "Offline";
                case -2:
                    return "Dirty MDC";
                case -1:
                    return "Unknown Error";
                case 0:
                    return "Succeeded";
                case 10:
                    return "None";
                case 19:
                    return "Pending";
                case 20:
                    return "In Progress";
                case 30:
                    return "In Progress (Acquire Slot)";
                case 31:
                    return "In Progress (Acquire Slot Waiting)";
                case 2147483645:
                    return "Reset";
                case 2147483646:
                    return "Suspended";
                case 2147483647:
                    return "Shutdown";
                default:
                    return "Unknown State";

            }
        }
        private string MapCCStateToString(int state)
        {
            switch (state)
            {
                case 0:
                    return "Initializing";
                case 1:
                    return "Never Downloaded";
                case 2:
                    return "No Download";
                case 3:
                    return "Complete";
                case 4:
                    return "DB Building";
                case 5:
                    return "DB Downloading";
                case 100:
                    return "Retry Web server";
                case 200:
                    return "BITS Not Available";
                case 300:
                    return "Failed First DB Build";
                case 301:
                    return "Failed Next DB Build";
                case 302:
                    return "Failed BITS Download";
                case 303:
                    return "Failed Processing Next Archive";
                case 304:
                    return "Failed adding archives";
                case 305:
                    return "Failed Webserver";
                case 306:
                    return "DB List Locked";
                case 400:
                    return "Generic Error";
               
                default:
                    return "Unknown State";

            }
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            
            CreateFilesIfNeeded();

            if (context.Request.FilePath == "/EnterpriseVault/ClientDiagnostics.aspx")
            {
                NameValueCollection queryStringParams = context.Request.QueryString;
                SyncInfo syncInfo = new SyncInfo();                

                foreach (String s in queryStringParams.AllKeys)
                {
                    switch (s)
                    {
                        case "MSt":
                            syncInfo.SetMDCState(queryStringParams[s]);
                            break;
                        case "CSt":
                            syncInfo.SetCCState(queryStringParams[s]);
                            break;
                        default:
                            break;
                    }                                    
                }
              
                if (syncInfo.IsMDCFailed())
                {
                    String log = String.Format("{0}: Client ({1}) MDC Sync failure ({2})\n", DateTime.Now.ToString(), context.Request.LogonUserIdentity.Name, MapMDCStateToString(syncInfo.MDCState));
                    WriteFailure(log);
                }
                else if (syncInfo.IsCCFailed())
                {
                    String log = String.Format("{0}: Client ({1}}) CC Sync failure ({2})\n", DateTime.Now.ToString(), context.Request.LogonUserIdentity.Name, MapCCStateToString(syncInfo.CCState));
                    WriteFailure(log);
                }
                else
                {
                    String log = String.Format("{0}: Client ({1}) Succesfully synchronized.\n", DateTime.Now.ToString(), context.Request.LogonUserIdentity.Name);
                    WriteSuccess(log);
                }
            }
        }

        public void Dispose() { }
    }
}
