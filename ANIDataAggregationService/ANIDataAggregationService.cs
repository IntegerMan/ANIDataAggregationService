using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Timers;
using ANIDataAggregationLibrary.Database;
using ANIDataAggregationLibrary.Traffic;
using ANIDataAggregationLibrary.Util;
using ANIDataAggregationLibrary.Weather;

namespace ANIDataAggregationService
{
    /// <summary>
    /// A WindowsService to manage data aggregation.
    /// </summary>
    public partial class ANIDataAggregationService : ServiceBase
    {
        private const int CreatorNodeId = 1;
        private WeatherForecastRecordingProcessor _weatherForecastProcessor;
        private TrafficRecordingProcessor _trafficProcessor;
        private ServiceLogger _logger;
        private Timer _weatherTimer;
        private Timer _trafficTimer;
        private readonly AniEntities _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ANIDataAggregationService"/> class.
        /// </summary>
        public ANIDataAggregationService()
        {
            InitializeComponent();

            _entities = new AniEntities();
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

            InitializeWeather();
            InitializeTraffic();
        }

        private void InitializeTraffic()
        {
            _trafficProcessor = new TrafficRecordingProcessor(CreatorNodeId, _logger, _entities);
            ProcessTrafficData();

            // Start the Traffic Timer
            _trafficTimer = new Timer(TimeSpan.FromMinutes(15).TotalMilliseconds);
            _trafficTimer.Elapsed += TrafficTimer_Tick;
            _trafficTimer.Start();
        }

        private void TrafficTimer_Tick(object sender, ElapsedEventArgs e)
        {
            _logger.Log("Processing Traffic Data");
            ProcessTrafficData();
            _logger.Log("Done Processing Traffic Data");
        }

        /// <summary>
        /// Initializes the weather processor.
        /// </summary>
        private void InitializeWeather()
        {
            // Kick into action immediately
            _weatherForecastProcessor = new WeatherForecastRecordingProcessor(CreatorNodeId, _logger, _entities);

            ProcessWeatherData();

            // Start the weather timer
            _weatherTimer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            _weatherTimer.Elapsed += WeatherTimer_Tick;
            _weatherTimer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void WeatherTimer_Tick(object sender, EventArgs e)
        {
            _logger.Log("Processing Weather Data");
            ProcessWeatherData();
            _logger.Log("Done Processing Weather Data");
        }

        /// <summary>
        /// Processes the traffic data.
        /// </summary>
        private void ProcessTrafficData()
        {
            try
            {
                // Focus on the Columbus area
                const double West = 40.198316;
                const double North = -83.194528; 
                const double East = 39.853391; 
                const double South = -82.840906;

                // Grab and record traffic data
                _trafficProcessor.RecordTrafficIncidents(West, North, East, South);
            }
            catch (Exception ex)
            {
                _logger.Error("Problem processing weather data: " + ex.Message);
            }
        }

        /// <summary>
        /// Processes the weather data.
        /// </summary>
        private void ProcessWeatherData()
        {
            try
            {
                // We need to recalibrate our frost prediction matrix
                _weatherForecastProcessor.UpdateNeuralNetwork();

                _weatherForecastProcessor.ZipCodes = AreaMonitor.GetWatchedZipCodes(_entities);

                // Now use things to record our predictions
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
            _trafficTimer.Stop();
        }

        /// <summary>
        /// Called when the service is paused.
        /// </summary>
        protected override void OnPause()
        {
            _weatherTimer.Stop();
            _trafficTimer.Stop();

            base.OnPause();
        }

        /// <summary>
        /// Called when the service was paused and is continued.
        /// </summary>
        protected override void OnContinue()
        {
            _weatherTimer.Start();
            _trafficTimer.Start();

            base.OnContinue();
        }
    }
}
