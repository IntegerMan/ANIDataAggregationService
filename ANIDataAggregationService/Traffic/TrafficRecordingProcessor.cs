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
            var url = string.Format("http://dev.virtualearth.net/REST/v1/Traffic/Incidents/{0},{1},{2},{3}?key={4}",
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

            if (!string.IsNullOrWhiteSpace(responseData))
            {
                var trafficData = JObject.Parse(responseData);
                var resources = trafficData["resourceSets"].First["resources"];

                foreach (var resource in resources)
                {
                    int i = 42;                    
                }

            }
        }
    }
}