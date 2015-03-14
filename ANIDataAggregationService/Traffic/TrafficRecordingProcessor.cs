using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ANIDataAggregationService.Traffic
{
    public class TrafficRecordingProcessor
    {
        private readonly string _bingMapsKey;

        public TrafficRecordingProcessor(string bingMapsKey)
        {
            _bingMapsKey = bingMapsKey;
        }

        public void RecordTrafficIncidents(double westLongitude, double northLatitude, double eastLongitude, double southLatitude)
        {

            // Request Traffic from around the area.
            var url = string.Format("http://dev.virtualearth.net/REST/v1/Traffic/Incidents/{0},{1},{2},{3}?includeLocationCodes=true&key={4}",
                eastLongitude,
                northLatitude,
                westLongitude,
                southLatitude,
                _bingMapsKey);

            var request = WebRequest.Create(url);
            var response = request.GetResponse();

            // Read the response and get data
            string responseData = null;
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

            var incidents = new List<TrafficIncident>();

            if (!string.IsNullOrWhiteSpace(responseData))
            {
                var trafficData = JObject.Parse(responseData);
                var resources = trafficData["resourceSets"].First["resources"];

                foreach (var resource in resources)
                {

                    var incident = new TrafficIncident
                    {
                        Latitude = resource["point"]["coordinates"].First.Value<double>(),
                        Longitude = resource["point"]["coordinates"].Last.Value<double>(),
                        IncidentId = resource.Value<long>("incidentId"),
                        Description = resource.Value<string>("description"),
                        IsClosed = resource.Value<bool>("roadClosed"),
                        IsVerified = resource.Value<bool>("verified"),
                        IncidentType = resource.Value<int>("type"),
                        Severity = resource.Value<int>("severity"),
                        Detour = resource.Value<string>("detour"),
                        Lane = resource.Value<string>("lane"),
                        StartTimeUTC = resource.Value<DateTime>("start"),
                        EndTimeUTC = resource.Value<DateTime>("end"),
                        ModifiedTimeUTC = resource.Value<DateTime>("lastModified"),
                        Congestion = resource.Value<string>("congestion")
                    };

                    // Set the to location
                    var toPoint = resource["toPoint"];
                    if (toPoint != null)
                    {
                        incident.ToLatitude = toPoint["coordinates"].First.Value<double>();
                        incident.ToLongitude = toPoint["coordinates"].Last.Value<double>();
                    }

                    incidents.Add(incident);
                }

            }

            int i = 42;                    

        }
    }
}