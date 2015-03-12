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
        private const int CreatorNodeId = 1;
        private WeatherForecastRecordingProcessor mWeatherForecastProcessor;
        private ServiceLogger mLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ANIDataAggregationService"/> class.
        /// </summary>
        public ANIDataAggregationService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the service starts.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void OnStart(string[] args)
        {
            // Log that we're attempting start
            mLogger = new ServiceLogger(this.EventLog);
            mLogger.Log("Starting Service");

            // Kick into action immediately
            mWeatherForecastProcessor = new WeatherForecastRecordingProcessor(CreatorNodeId, mLogger);
            this.ProcessData();

            // Start the timer
            this.timer.Tick += Timer_Tick;
            this.timer.Interval = TimeSpan.FromHours(3).Milliseconds;
            this.timer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            ProcessData();
        }

        /// <summary>
        /// Processes the weather data.
        /// </summary>
        private void ProcessData()
        {
            try
            {
                mWeatherForecastProcessor.RecordWeatherForecasts();
            }
            catch (Exception ex)
            {
                mLogger.Error("Problem processing aggregated data: " + ex.Message);
            }
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
