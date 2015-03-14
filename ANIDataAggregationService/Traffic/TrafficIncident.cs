using System;
using System.Collections.Generic;

namespace ANIDataAggregationService.Traffic
{
    /// <summary>
    /// Represents a traffic incident or construction item from the Bing Maps Traffic API.
    /// </summary>
    public class TrafficIncident
    {
        /// <summary>
        /// Gets or sets the Bing Maps identifier for this incident.
        /// </summary>
        /// <value>The incident identifier.</value>
        public long IncidentId { get; set; }

        /// <summary>
        /// Gets or sets the latitude for the start of this incident.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude for the start of this incident.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the latitude for the end of this incident. This is optional.
        /// </summary>
        /// <value>Ending latitude.</value>
        public double? ToLatitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude for the end of this incident. This is optional.
        /// </summary>
        /// <value>Ending longitude.</value>
        public double? ToLongitude { get; set; }

        /// <summary>
        /// Gets or sets the congestion.
        /// </summary>
        /// <value>The congestion.</value>
        public string Congestion { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the detour.
        /// </summary>
        /// <value>The detour.</value>
        public string Detour { get; set; }

        /// <summary>
        /// Gets or sets the start time in UTC.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the end time in UTC.
        /// </summary>
        /// <value>The end time UTC.</value>
        public DateTime EndTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the modified time in UTC.
        /// </summary>
        /// <value>The modified time UTC.</value>
        public DateTime ModifiedTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the lane.
        /// </summary>
        /// <value>The lane.</value>
        public string Lane { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this road is closed.
        /// </summary>
        /// <value><c>true</c> if this road is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>The severity.</value>
        public int Severity { get; set; }

        /// <summary>
        /// Gets or sets the type of the incident.
        /// </summary>
        /// <value>The type of the incident.</value>
        public int IncidentType { get; set; }

        /// <summary>
        /// Gets or sets the location codes.
        /// </summary>
        /// <value>The location codes.</value>
        public List<String> LocationCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is verified.
        /// </summary>
        /// <value><c>true</c> if this instance is verified; otherwise, <c>false</c>.</value>
        public bool IsVerified { get; set; }

    }
}