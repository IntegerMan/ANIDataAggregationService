using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ANIDataAggregationLibrary.Weather
{
    public class WeatherData
    {
        public double Lat { get; set; }

        public double Lng { get; set; }
        public IList<WeatherForecast> Forecasts { get; set; }
        public string Description { get; set; }
        public int WeatherCode { get; set; }
        public int Temperature { get; set; }
        public DateTime RecordDateUTC { get; set; }
        public int WindChill { get; set; }
        public int WindDirection { get; set; }
        public int WindSpeed { get; set; }
        public int Humidity { get; set; }
        public int Visibility { get; set; }
        public int Rising { get; set; }
        public int ZipCode { get; set; }
        public double Pressure { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
    }
}