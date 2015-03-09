using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ANIDataAggregationService
{
    public partial class ANIDataAggregationService : ServiceBase
    {
        private const string EventLogSource = "ANI Data Aggregation Service";
        private const string EventLogName = "Application";

        public ANIDataAggregationService()
        {
            InitializeComponent();

            try
            {
                // Ensure we have the event log we're trying to write to
                if (!EventLog.SourceExists(EventLogSource))
                {
                    EventLog.CreateEventSource(EventLogSource, EventLogName);
                }
            }
            catch (SecurityException sex)
            {
                // This can happen when we don't have permission to view certain subcontained logs and is allowable.
                // If the warn logging fails, we'll not be able to log at all and the app should terminate now.
                Warn("Error ensuring event source: " + sex.Message);
            }
        }

        private void Log(string message)
        {
            EventLog.WriteEntry(EventLogSource, message, EventLogEntryType.Information);
        }

        private void Warn(string message)
        {
            EventLog.WriteEntry(EventLogSource, message, EventLogEntryType.Warning);
        }

        protected override void OnStart(string[] args)
        {
            Log("Service Starting");
        }

        protected override void OnStop()
        {
            Log("Service Stopping");
        }
    }
}
