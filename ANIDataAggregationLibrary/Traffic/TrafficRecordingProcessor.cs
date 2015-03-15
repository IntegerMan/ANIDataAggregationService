using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ANIDataAggregationLibrary.Database;
using ANIDataAggregationLibrary.Properties;
using ANIDataAggregationLibrary.Util;
using Newtonsoft.Json.Linq;

namespace ANIDataAggregationLibrary.Traffic
{
    /// <summary>
    /// A data processor that records traffic incidents
    /// </summary>
    public class TrafficRecordingProcessor
    {
        private readonly ServiceLogger _logger;

        private readonly string _bingMapsKey;
        private readonly int _userNodeId;
        private readonly AniEntities _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrafficRecordingProcessor" /> class.
        /// </summary>
        /// <param name="userNodeId">The user node identifier.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="entities">The entity.</param>
        /// <exception cref="System.ArgumentNullException">entities</exception>
        public TrafficRecordingProcessor(int userNodeId, ServiceLogger logger, AniEntities entities)
        {
            // Ensure logger exists
            _logger = logger ?? new ServiceLogger();

            _userNodeId = userNodeId;

            _bingMapsKey = Settings.Default.BingMapsKey;

            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            _entities = entities;
        }

        public void RecordTrafficIncidents(double westLongitude, double northLatitude, double eastLongitude, double southLatitude)
        {
            try
            {
                var incidents = GetTrafficIncidents(westLongitude, northLatitude, eastLongitude, southLatitude);
                
                foreach (var incident in incidents)
                {
                    _logger.Log(string.Format("Logging incident at {0}, {1}: {2}", incident.Latitude, incident.Longitude, incident.Description));
                    _entities.InsertUpdateTrafficIncident(incident.IncidentId,
                        incident.Description,
                        incident.Congestion,
                        incident.Detour,
                        incident.Lane,
                        incident.IsClosed,
                        incident.IsVerified,
                        DateTime.UtcNow,
                        incident.ModifiedTimeUTC,
                        incident.StartTimeUTC,
                        incident.EndTimeUTC,
                        incident.Latitude,
                        incident.Longitude,
                        incident.ToLatitude,
                        incident.ToLongitude,
                        _userNodeId,
                        incident.Severity,
                        incident.IncidentType);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Problem parsing traffic {0}", ex.Message));
            }
        }

        private IEnumerable<TrafficIncident> GetTrafficIncidents(double westLongitude, double northLatitude, double eastLongitude, double southLatitude)
        {
            // Request Traffic from around the area.
            var url =
                string.Format(
                    "http://dev.virtualearth.net/REST/v1/Traffic/Incidents/{0},{1},{2},{3}?includeLocationCodes=true&key={4}",
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

            // Interpret data received
            if (!string.IsNullOrWhiteSpace(responseData))
            {
                var trafficData = JObject.Parse(responseData);
                var resources = trafficData["resourceSets"].First["resources"];

                foreach (var resource in resources)
                {
                    // Create the traffic incident. This doesn't need to worry if any of these elements aren't present (other than coords, but those are mandatory)
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

                    // Set the to location if it's present
                    var toPoint = resource["toPoint"];
                    if (toPoint != null)
                    {
                        incident.ToLatitude = toPoint["coordinates"].First.Value<double>();
                        incident.ToLongitude = toPoint["coordinates"].Last.Value<double>();
                    }

                    yield return incident;
                }
            }
        }
    }
}