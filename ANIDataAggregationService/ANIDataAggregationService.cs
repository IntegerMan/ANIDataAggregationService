using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace ANIDataAggregationService
{
    /// <summary>
    /// A WindowsService to manage data aggregation.
    /// </summary>
    public partial class ANIDataAggregationService : ServiceBase
    {
        private const string EventLogSource = "ANI Data Aggregation Service";

        /// <summary>
        /// Initializes a new instance of the <see cref="ANIDataAggregationService"/> class.
        /// </summary>
        public ANIDataAggregationService()
        {
            InitializeComponent();
       }

        /// <summary>
        /// Logs the specified message to the event log.
        /// </summary>
        /// <param name="message">The message.</param>
        private void Log(string message)
        {
            this.logger.WriteEntry(message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Logs a warning message to the event log.
        /// </summary>
        /// <param name="message">The message.</param>
        private void Warn(string message)
        {
            this.logger.WriteEntry(message, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Called when the service starts.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void OnStart(string[] args)
        {
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Log("Evaluating now.");
        }

        /// <summary>
        /// Called when the service is stopped.
        /// </summary>
        protected override void OnStop()
        {
            this.timer.Stop();
        }

        /// <summary>
        /// Called when the service is paused.
        /// </summary>
        protected override void OnPause()
        {
            this.timer.Stop();

            base.OnPause();
        }

        /// <summary>
        /// Called when the service was paused and is continued.
        /// </summary>
        protected override void OnContinue()
        {
            this.timer.Start();

            base.OnContinue();
        }
    }
}
