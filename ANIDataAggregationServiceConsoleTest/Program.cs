using System;
using System.Collections.Generic;
using ANIDataAggregationLibrary.Database;
using ANIDataAggregationLibrary.Traffic;
using ANIDataAggregationLibrary.Util;
using ANIDataAggregationLibrary.Weather;

namespace ANIDataAggregationServiceConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var processor = new WeatherForecastRecordingProcessor(1, null, new AniEntities())
            {
                ZipCodes = new List<int> {43035}
            };
            processor.RecordWeatherForecasts();

            /*
            var processor = new TrafficRecordingProcessor(1, null);

            // Focus on the Columbus area
            const double West = 40.198316;
            const double North = -83.194528; 
            const double East = 39.853391; 
            const double South = -82.840906;

            processor.RecordTrafficIncidents(West, North, East, South);
            */

            /*
            var algorithm = new FrostPredictionAlgorithm();
            algorithm.TrainNeuralNet();
            var expectedFrost = algorithm.GetExpectedFrostInMinutes(32, 42, 57);
            Console.WriteLine("Expected Frost: " + expectedFrost);
            */
        }
    }
}
