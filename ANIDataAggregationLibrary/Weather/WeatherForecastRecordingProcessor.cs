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
        const string GeoNamespace = "http://www.w3.org/2003/01/geo/wgs84_pos#";

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
                    var weatherData = GetWeatherData(zipCode);

                    // Store the current weather
                    _entities.RecordWeatherObservation(weatherData.ZipCode,
                        weatherData.WeatherCode,
                        weatherData.Temperature,
                        weatherData.Description,
                        weatherData.RecordDateUTC,
                        this.CreatorNodeId,
                        weatherData.Sunrise,
                        weatherData.Sunset,
                        weatherData.Humidity,
                        weatherData.Visibility,
                        weatherData.Pressure,
                        weatherData.Rising,
                        weatherData.WindChill,
                        weatherData.WindDirection,
                        weatherData.WindSpeed,
                        weatherData.Lat,
                        weatherData.Lng,
                        1);

                    // Store the frost predictions
                    foreach (var forecast in weatherData.Forecasts)
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
        /// Gets the weather data and forecasts from a web service and converts them into a domain objects.
        /// </summary>
        /// <param name="zipCode">The zip code.</param>
        /// <returns>A WeatherData object containing the current weather and forecasted other weather entries.</returns>
        private static WeatherData GetWeatherData(int zipCode)
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

            // No data. Ignore it.
            if (responseData == null)
            {
                return null;
            }

            // Parse the XML into a document and identify the forecasts contained in it
            var document = XDocument.Parse(responseData);

            var weatherData = new WeatherData
            {
                ZipCode = zipCode,
                Lat = double.Parse(document.Descendants(XName.Get("lat", GeoNamespace)).First().Value),
                Lng = double.Parse(document.Descendants(XName.Get("lng", GeoNamespace)).First().Value),
            };

            // Interpret Conditions
            var condition = document.Descendants(XName.Get("condition", YahooNamespace)).First();
            weatherData.Description = condition.Attribute("text").Value;
            weatherData.WeatherCode = int.Parse(condition.Attribute("code").Value);
            weatherData.Temperature = int.Parse(condition.Attribute("temp").Value);
            weatherData.RecordDateUTC = DateTime.Parse(condition.Attribute("date").Value).ToUniversalTime();

            // Interpret Wind
            var windNode = document.Descendants(XName.Get("wind", YahooNamespace)).First();
            weatherData.WindChill = int.Parse(windNode.Attribute("chill").Value);
            weatherData.WindDirection = int.Parse(windNode.Attribute("direction").Value);
            weatherData.WindSpeed = int.Parse(windNode.Attribute("speed").Value);

            // Interpret Atmosphere
            var atmosNode = document.Descendants(XName.Get("atmosphere", YahooNamespace)).First();
            weatherData.Humidity = int.Parse(atmosNode.Attribute("humidity").Value);
            weatherData.Visibility = int.Parse(atmosNode.Attribute("visibility").Value);
            weatherData.Pressure = double.Parse(atmosNode.Attribute("pressure").Value);
            weatherData.Rising = int.Parse(atmosNode.Attribute("rising").Value);

            // Interpret Astronomy
            var astroNode = document.Descendants(XName.Get("astronomy", YahooNamespace)).First();
            weatherData.Sunrise = astroNode.Attribute("sunrise").Value;
            weatherData.Sunset = astroNode.Attribute("sunset").Value;

            // Parse the forecast elements into forecast objects
            var forecasts = document.Descendants(XName.Get("forecast", YahooNamespace));
            weatherData.Forecasts = new List<WeatherForecast>();
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

                forecast.Description = forecastElement.Attribute("text").Value;

                weatherData.Forecasts.Add(forecast);
            }

            return weatherData;
        }
    }
}