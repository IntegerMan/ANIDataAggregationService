using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ANIDataAggregationService
{
    public class WeatherForecastRecordingProcessor
    {
        const string YahooNamespace = "http://xml.weather.yahoo.com/ns/rss/1.0";

        public string ZipCode { get; set; }

        public WeatherForecastRecordingProcessor(string zipCode = "43035")
        {
            this.ZipCode = zipCode;
        }

        /// <summary>
        /// Gets tomorrow's weather prediction for this zip code and records it in the database.
        /// </summary>
        public void RecordTomorrowsWeatherPrediction()
        {
            var forecasts = GetWeatherForecast();

        }

        /// <summary>
        /// Gets the weather forecasts from a web service and converts them into domain objects which are then yielded.
        /// </summary>
        /// <returns>A yielded enumerable of WeatherForecast objects representing forecasts for the next 5 days including today.</returns>
        private IEnumerable<WeatherForecast> GetWeatherForecast()
        {

            // Grab the data from a GET request based on our zip code.
            var uriString = string.Format("http://xml.weather.yahoo.com/forecastrss/{0}_f.xml", this.ZipCode);
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
                    var forecast = new WeatherForecast();

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