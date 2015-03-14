using System;
using System.Collections.Generic;

namespace ANIDataAggregationService.Traffic
{
    public class TrafficIncident
    {
        public long IncidentID { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double ToLatitude { get; set; }

        public double ToLongitude { get; set; }

        public string Congestion { get; set; }

        public string Description { get; set; }

        public string Detour { get; set; }

        public DateTime StartTimeUTC { get; set; }

        public DateTime EndTimeUTC { get; set; }

        public DateTime ModifiedTimeUTC { get; set; }

        public string Lane { get; set; }

        public bool IsClosed { get; set; }

        public int Severity { get; set; }

        public int IncidentType { get; set; }

        public List<String> LocationCodes { get; set; } 

        public bool IsVerified { get; set; }

    }
}