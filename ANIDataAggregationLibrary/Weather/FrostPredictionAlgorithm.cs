using System;
using System.Collections.Generic;
using System.Linq;
using ANIDataAggregationLibrary.Database;
using ANIDataAggregationLibrary.Database.AniDataSetTableAdapters;
using Encog.ML.Data;
using Encog.ML.Data.Versatile;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace ANIDataAggregationLibrary.Weather
{
    public class FrostPredictionAlgorithm
    {
        private readonly BasicNetwork _network;

        public int MaxIterations { get; set;  } = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrostPredictionAlgorithm"/> class.
        /// </summary>
        public FrostPredictionAlgorithm()
        {
            _network = CreateNeuralNet();
        }

        const int NumInputs = 3;

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

            Console.WriteLine("Neural Network Results: Err: {0}, Iterations: {1}, Max: {2}", train.Error, epoch, maxError);
            foreach (var pair in trainingSet)
            {
                var output = _network.Compute(pair.Input);

                Console.WriteLine("{0}, actual={1},ideal={2}", pair.Input, output[0], pair.Ideal[0]);
            }

        }

        /// <summary>
        /// Gets the training data set from the database (based on recorded frost observations)
        /// </summary>
        /// <returns>The training data set.</returns>
        private static BasicNeuralDataSet GetTrainingDataSet()
        {
            var adapter = new FrostPredictionDataViewTableAdapter();
            var frostDataTable = adapter.GetData();

            var inputRows = new List<List<double>>();
            var idealRows = new List<List<double>>();

            foreach (AniDataSet.FrostPredictionDataViewRow row in frostDataTable.Rows)
            {
                var inputs = new List<double>(NumInputs)
                {
                    GetDoubleFromExpectedRange(row.Low, -50, 100),
                    GetDoubleFromExpectedRange(row.High, -50, 100),
                    GetDoubleFromExpectedRange(row.WeatherCode, 0, 50)
                };

                /* I - CAN - do this, but this would make me need to know these for predictions
                inputs.Add(GetDoubleFromBoolean(row.HasRain));
                inputs.Add(GetDoubleFromBoolean(row.HasClouds));
                inputs.Add(GetDoubleFromBoolean(row.HasSnow));
                inputs.Add(GetDoubleFromBoolean(row.HasStorm));
                inputs.Add(GetDoubleFromBoolean(row.HasWind));
                */

                inputRows.Add(inputs);

                var ideals = new List<double>(1) {row.MinutesToDefrost};
                idealRows.Add(ideals);
            }

            var trainInputs = inputRows.Select(r => r.ToArray()).ToArray();
            var trainIdeals = idealRows.Select(r => r.ToArray()).ToArray();

            var trainingSet = new BasicNeuralDataSet(trainInputs, trainIdeals);
            return trainingSet;
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
            if (range == 0)
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
            network.AddLayer(new BasicLayer(null, true, NumInputs * 3));
            network.AddLayer(new BasicLayer(null, false, 1));

            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }

        public double GetExpectedFrostInMinutes(double weatherCode, double low, double high)
        {

            var inputs = new List<double>(NumInputs)
            {
                GetDoubleFromExpectedRange(low, -50, 100),
                GetDoubleFromExpectedRange(high, -50, 100),
                GetDoubleFromExpectedRange(weatherCode, 0, 50)
            };

            var outputs = new List<double>(1) {0}.ToArray();

            _network.Compute(inputs.ToArray(), outputs);

            return outputs[0];
        }
    }
}