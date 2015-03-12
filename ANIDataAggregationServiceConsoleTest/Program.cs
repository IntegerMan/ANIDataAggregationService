using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANIDataAggregationService;

namespace ANIDataAggregationServiceConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var processor = new WeatherForecastRecordingProcessor(1, 43035);
            processor.RecordTomorrowsWeatherPrediction();

        }
    }
}
