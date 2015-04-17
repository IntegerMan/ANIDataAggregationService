using System;
using System.Collections.Generic;
using System.Linq;
using ANIDataAggregationLibrary.Database;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace ANIDataAggregationLibrary.Weather
{
    public class FrostPredictionAlgorithm
    {
        private readonly BasicNetwork _network;
        private readonly AniEntities _entities;

        private readonly HashSet<int> _hasClouds = new HashSet<int>();
        private readonly HashSet<int> _hasWind = new HashSet<int>();
        private readonly HashSet<int> _hasRain = new HashSet<int>();
        private readonly HashSet<int> _hasStorm = new HashSet<int>();
        private readonly HashSet<int> _hasSnow = new HashSet<int>();

        public int MaxIterations { get; set; }

        public double ErrorRate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrostPredictionAlgorithm"/> class.
        /// </summary>
        public FrostPredictionAlgorithm() : this(new AniEntities())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrostPredictionAlgorithm"/> class.
        /// </summary>
        public FrostPredictionAlgorithm(AniEntities entities)
        {
            _network = CreateNeuralNet();

            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            _entities = entities;
            MaxIterations = 10000;

            GetWeatherCodeInformation();
        }

        private void GetWeatherCodeInformation()
        {
            foreach (WeatherCode code in _entities.WeatherCodes)
            {
                if (code.WC_HasClouds)
                {
                    _hasClouds.Add(code.WC_ID);
                }

                if (code.WC_HasRain)
                {
                    _hasRain.Add(code.WC_ID);
                }

                if (code.WC_HasSnow)
                {
                    _hasSnow.Add(code.WC_ID);
                }

                if (code.WC_HasStorm)
                {
                    _hasStorm.Add(code.WC_ID);
                }

                if (code.WC_HasWind)
                {
                    _hasWind.Add(code.WC_ID);
                }
            }
        }

        const int NumInputs = 8;

        public void TrainNeuralNet()
        {
            var trainingSet = GetTrainingDataSet();

            var train = new ResilientPropagation(_network, trainingSet);
            var epoch = 1;
            var maxError = 0.0;
            var minError = 100.0;
            do
            {
                train.Iteration();
                epoch++;

                minError = Math.Min(train.Error, minError);
                maxError = Math.Max(train.Error, maxError);

            } while ((epoch < MaxIterations) && (train.Error > 0.001));

            /*
            Console.WriteLine("Neural Network Results: Err: {0}, Iterations: {1}, Max: {2}", train.Error, epoch, maxError);
            foreach (var pair in trainingSet)
            {
                var output = _network.Compute(pair.Input);

                Console.WriteLine("{0}, actual={1},ideal={2}", pair.Input, output[0], pair.Ideal[0]);
            }
            */

            this.Iterations = train.IterationNumber;
            this.ErrorRate = train.Error;
        }

        public int Iterations { get; set; }

        /// <summary>
        /// Gets the training data set from the database (based on recorded frost observations)
        /// </summary>
        /// <returns>The training data set.</returns>
        private BasicNeuralDataSet GetTrainingDataSet()
        {
            var inputRows = new List<List<double>>();
            var idealRows = new List<List<double>>();

            foreach (var row in _entities.FrostPredictionDataViews)
            {
                var inputs = new List<double>(NumInputs)
                {
                    GetDoubleFromExpectedRange(row.Low, -50, 100),
                    GetDoubleFromExpectedRange(row.High, -50, 100),
                    GetDoubleFromExpectedRange(row.WeatherCode, 0, 50),
                    GetDoubleFromBoolean(row.HasRain),
                    GetDoubleFromBoolean(row.HasClouds),
                    GetDoubleFromBoolean(row.HasSnow),
                    GetDoubleFromBoolean(row.HasStorm),
                    GetDoubleFromBoolean(row.HasWind)
                };

                inputRows.Add(inputs);

                var ideals = new List<double>(1) {GetMinutesToDefrostNeuralNetValue(row.MinutesToDefrost)};
                idealRows.Add(ideals);
            }

            var trainInputs = inputRows.Select(r => r.ToArray()).ToArray();
            var trainIdeals = idealRows.Select(r => r.ToArray()).ToArray();

            var trainingSet = new BasicNeuralDataSet(trainInputs, trainIdeals);
            return trainingSet;
        }

        private static double GetMinutesToDefrostNeuralNetValue(double value)
        {
            if (value <= 0)
            {
                return -1;
            }

            return value;
        }

        /// <summary>
        /// Normalizes a double value based on an expected range of values
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="low">The low.</param>
        /// <param name="high">The high.</param>
        /// <returns>System.Double.</returns>
        private static double GetDoubleFromExpectedRange(double value, double low, double high)
        {
            // Calculate the range of the items
            var range = high - low;

            // Prevent division by zero
            if (Math.Abs(range) < Double.Epsilon)
            {
                return 0;
            }

            // Normalize our value
            if (low < 0)
            {
                value += -low;
            }
            else
            {
                value -= low;
            }

            // Calculate percentage and offset from -1
            var calculated = -1 + (2 * (value/range));

            // Respect our ranges
            return Math.Max(-1, Math.Min(1, calculated));
        }

        /// <summary>
        /// Gets a double value from a boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A double value for the boolean</returns>
        private static double GetDoubleFromBoolean(bool value)
        {
            return value ? 1 : -1;
        }

        /// <summary>
        /// Creates the neural network structure and primes it for usage.
        /// </summary>
        /// <returns>A primed neural network</returns>
        private static BasicNetwork CreateNeuralNet()
        {
            var network = new BasicNetwork();

            network.AddLayer(new BasicLayer(null, true, NumInputs));
            network.AddLayer(new BasicLayer(null, true, NumInputs * 3));
            network.AddLayer(new BasicLayer(null, true, NumInputs * 3));
            network.AddLayer(new BasicLayer(null, false, 1));

            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }

        public double GetExpectedFrostInMinutes(WeatherForecast prediction)
        {
            return GetExpectedFrostInMinutes(prediction.Code, prediction.Low, prediction.High);
        }

        public double GetExpectedFrostInMinutes(int weatherCode, double low, double high)
        {

            var inputs = new List<double>(NumInputs)
            {
                GetDoubleFromExpectedRange(low, -50, 100),
                GetDoubleFromExpectedRange(high, -50, 100),
                GetDoubleFromExpectedRange(weatherCode, 0, 50),
                GetDoubleFromBoolean(_hasRain.Contains(weatherCode)),
                GetDoubleFromBoolean(_hasClouds.Contains(weatherCode)),
                GetDoubleFromBoolean(_hasSnow.Contains(weatherCode)),
                GetDoubleFromBoolean(_hasStorm.Contains(weatherCode)),
                GetDoubleFromBoolean(_hasWind.Contains(weatherCode))
            };

            var outputs = new List<double>(1) {0}.ToArray();

            _network.Compute(inputs.ToArray(), outputs);

            return GetMinutesToDefrostNeuralNetValue(outputs[0]);
        }
    }
}