using ANIDataAggregationLibrary.Traffic;

namespace ANIDataAggregationServiceConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var zipCodes = AreaMonitor.GetWatchedZipCodes();
            var processor = new WeatherForecastRecordingProcessor(1, null, zipCodes);
            processor.RecordWeatherForecasts();
            */

            var processor = new TrafficRecordingProcessor(1, null);

            // Focus on the Columbus area
            const double West = 40.198316;
            const double North = -83.194528; 
            const double East = 39.853391; 
            const double South = -82.840906;

            processor.RecordTrafficIncidents(West, North, East, South);
        }
    }
}
