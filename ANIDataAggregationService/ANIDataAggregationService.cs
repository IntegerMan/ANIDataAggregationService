﻿using System;
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
            mLogger = new ServiceLogger(this.EventLog);
            mWeatherForecastProcessor = new WeatherForecastRecordingProcessor(CreatorNodeId, mLogger);

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
            mWeatherForecastProcessor.RecordWeatherForecasts();
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
