using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using ANIDataAggregationLibrary.Database;
using ANIDataAggregationLibrary.Util;

namespace ANIDataAggregationLibrary.Weather
{
    public class WeatherForecastRecordingProcessor
    {
        const string YahooNamespace = "http://xml.weather.yahoo.com/ns/rss/1.0";

        private readonly ServiceLogger _logger;
        private readonly AniEntities _entities;
        private readonly FrostPredictionAlgorithm _frostAlgorithm;

        /// <summary>
        /// Gets or sets the zip codes to monitor.
        /// </summary>
        /// <value>The zip codes.</value>
        public IEnumerable<int> ZipCodes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastRecordingProcessor" /> class.
        /// </summary>
        /// <param name="creatorNodeId">The creator node identifier.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="zipCodes">The zip codes to monitor.</param>
        /// <param name="entities">The entities.</param>
        /// <exception cref="System.ArgumentNullException">entities</exception>
        public WeatherForecastRecordingProcessor(int creatorNodeId, ServiceLogger logger, IEnumerable<int> zipCodes, AniEntities entities)
        {
            // Ensure logger exists
            _logger = logger ?? new ServiceLogger();

            CreatorNodeId = creatorNodeId;
            ZipCodes = zipCodes.ToList();

            // Ensure entities is not null
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            _entities = entities;

            _frostAlgorithm = new FrostPredictionAlgorithm(_entities);
        }

        /// <summary>
        /// Gets or sets the ID of the node responsible for creating prediction entries.
        /// </summary>
        /// <value>The creator node identifier.</value>
        public int CreatorNodeId { get; set; }

        public void UpdateNeuralNetwork()
        {
            _logger.Log("Retraining Frost Prediction Neural Net");

            // Ensure the neural net is trained up
            _frostAlgorithm.TrainNeuralNet();

            _logger.Log("Frost Prediction Neural Net Calibrated at error rate of: " + _frostAlgorithm.ErrorRate);
        }

        /// <summary>
        /// Gets tomorrow's weather prediction for this zip code and records it in the database.
        /// </summary>
        public void RecordWeatherForecasts()
        {
            foreach (var zipCode in this.ZipCodes)
            {
                try
                {
                    var forecasts = GetWeatherForecast(zipCode);

                    foreach (var forecast in forecasts)
                    {
                        // Calculate the frost from the neural net
                        var frostTimeInMinutes = _frostAlgorithm.GetExpectedFrostInMinutes(forecast);

                        // Log what we're doing
                        _logger.Log(string.Format("Logging Forecast for {4} on {0}: {1}/{2},  code: {3} and frost time: {5}",
                            forecast.Date.ToShortDateString(), forecast.Low, forecast.High, forecast.Code,
                            forecast.ZipCode, frostTimeInMinutes));

                        // Put the values into the database - this will update an existing entry or make a new one as needed
                        _entities.InsertUpdateWeatherPrediction(forecast.Date, CreatorNodeId, forecast.ZipCode,
                            forecast.Low, forecast.High, forecast.Code, frostTimeInMinutes);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Problem parsing zip code {0}: {1}", zipCode, ex.Message));
                }
            }
        }

        /// <summary>
        /// Gets the weather forecasts from a web service and converts them into domain objects which are then yielded.
        /// </summary>
        /// <param name="zipCode">The zip code.</param>
        /// <returns>A yielded enumerable of WeatherForecast objects representing forecasts for the next 5 days including today.</returns>
        private IEnumerable<WeatherForecast> GetWeatherForecast(int zipCode)
        {

            // Grab the data from a GET request based on our zip code.
            var uriString = string.Format("http://xml.weather.yahoo.com/forecastrss/{0}_f.xml", zipCode);
            var request = WebRequest.Create(uriString);
            var response = request.GetResponse();

            string responseData = null;

            // Read the response and get data
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
            }

            // If we actually got data, interpret it
            if (responseData != null)
            {

                // Parse the XML into a document and identify the forecasts contained in it
                var document = XDocument.Parse(responseData);
                var forecasts = document.Descendants(XName.Get("forecast", YahooNamespace));

                // Parse the forecast elements into forecast objects
                foreach (var forecastElement in forecasts)
                {
                    var forecast = new WeatherForecast {ZipCode = zipCode};

                    var dateAttribute = forecastElement.Attribute("date");
                    forecast.Date = DateTime.Parse(dateAttribute.Value);

                    var lowAttribute = forecastElement.Attribute("low");
                    forecast.Low = double.Parse(lowAttribute.Value);

                    var highAttribute = forecastElement.Attribute("high");
                    forecast.High = double.Parse(highAttribute.Value);

                    var codeAttribute = forecastElement.Attribute("code");
                    forecast.Code = int.Parse(codeAttribute.Value);

                    yield return forecast;
                }

            }

        }
    }
}