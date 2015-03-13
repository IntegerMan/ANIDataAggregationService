using System;
using System.ServiceProcess;
using System.Timers;

namespace ANIDataAggregationService
{
    /// <summary>
    /// A WindowsService to manage data aggregation.
    /// </summary>
    public partial class ANIDataAggregationService : ServiceBase
    {
        private const int CreatorNodeId = 1;
        private WeatherForecastRecordingProcessor _weatherForecastProcessor;
        private ServiceLogger _logger;
        private Timer _weatherTimer;

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
            _logger = new ServiceLogger(EventLog);
            _logger.Log("Starting ANI Service");

            // Kick into action immediately
            _weatherForecastProcessor = new WeatherForecastRecordingProcessor(CreatorNodeId, _logger);
            ProcessWeatherData();

            // Start the timer
            _weatherTimer = new Timer(TimeSpan.FromHours(3).TotalMilliseconds);
            _weatherTimer.Elapsed += Timer_Tick;
            _weatherTimer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            _logger.Log("Timer Tick");
            ProcessWeatherData();
        }

        /// <summary>
        /// Processes the weather data.
        /// </summary>
        private void ProcessWeatherData()
        {
            try
            {
                _weatherForecastProcessor.RecordWeatherForecasts();
            }
            catch (Exception ex)
            {
                _logger.Error("Problem processing weather data: " + ex.Message);
            }
        }

        /// <summary>
        /// Called when the service is stopped.
        /// </summary>
        protected override void OnStop()
        {
            _weatherTimer.Stop();
        }

        /// <summary>
        /// Called when the service is paused.
        /// </summary>
        protected override void OnPause()
        {
            _weatherTimer.Stop();

            base.OnPause();
        }

        /// <summary>
        /// Called when the service was paused and is continued.
        /// </summary>
        protected override void OnContinue()
        {
            _weatherTimer.Start();

            base.OnContinue();
        }
    }
}
