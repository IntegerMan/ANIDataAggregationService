//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ANIDataAggregationLibrary.Database
{
    using System;
    
    public partial class LatestWeatherEntrySelect_Result
    {
        public int WeatherCode { get; set; }
        public string WeatherCodeName { get; set; }
        public int Temperature { get; set; }
        public string Description { get; set; }
        public System.DateTime RecordTimeUTC { get; set; }
        public System.DateTime CreatedDateUTC { get; set; }
        public string Sunrise { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public int WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public int WindChill { get; set; }
        public int Rising { get; set; }
        public double Pressure { get; set; }
        public int Visibility { get; set; }
        public int Humidity { get; set; }
        public string Sunset { get; set; }
        public int ZipCode { get; set; }
        public int SeverityID { get; set; }
        public string SeverityName { get; set; }
        public bool HasRain { get; set; }
        public bool HasClouds { get; set; }
        public bool HasStorm { get; set; }
        public bool HasWind { get; set; }
        public bool HasSnow { get; set; }
        public string IconClass { get; set; }
        public string ImageUrl { get; set; }
    }
}
