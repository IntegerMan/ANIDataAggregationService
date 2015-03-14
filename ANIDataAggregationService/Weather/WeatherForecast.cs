using System;

namespace ANIDataAggregationService
{
    /// <summary>
    /// Represents a weather forecast as it relates to Yahoo Weather forecasts.
    /// </summary>
    public struct WeatherForecast
    {
        /// <summary>
        /// Gets or sets the date of the entry.
        /// </summary>
        /// <value>The date of the entry.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the low temperature in imperial units.
        /// </summary>
        /// <value>The low temperature.</value>
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the high temperature in imperial units.
        /// </summary>
        /// <value>The high temperature.</value>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the Yahoo weather code value.
        /// </summary>
        /// <value>The weather code.</value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the zip code for the location this prediction is centered around.
        /// </summary>
        /// <value>The zip code for the location this prediction is centered around.</value>
        public int ZipCode { get; set; }
    }
}