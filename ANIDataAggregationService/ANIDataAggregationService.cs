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

        public ANIDataAggregationService()
        {
            InitializeComponent();
        }

        private void Log(string message)
        {
            this.logger.WriteEntry(message, EventLogEntryType.Information);
        }

        private void Warn(string message)
        {
            this.logger.WriteEntry(message, EventLogEntryType.Warning);
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
