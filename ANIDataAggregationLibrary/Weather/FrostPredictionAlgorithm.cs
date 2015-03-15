using System;
using System.Collections.Generic;
using System.Linq;
using ANIDataAggregationLibrary.Database;
using ANIDataAggregationLibrary.Database.AniDataSetTableAdapters;
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

        public int MaxIterations { get; set;  } = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrostPredictionAlgorithm"/> class.
        /// </summary>
        public FrostPredictionAlgorithm()
        {
            _network = CreateNeuralNet();
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

            Console.WriteLine("Neural Network Results: Err: {0}, Iterations: {1}, Max: {2}", train.Error, epoch, maxError);
            foreach (var pair in trainingSet)
            {
                var output = _network.Compute(pair.Input);

                Console.WriteLine("{0}, actual={1},ideal={2}", pair.Input, output[0], pair.Ideal[0]);
            }

        }

        private BasicNeuralDataSet GetTrainingDataSet()
        {
            var adapter = new FrostPredictionDataViewTableAdapter();
            var frostDataTable = adapter.GetData();

            var inputRows = new List<List<double>>();
            var idealRows = new List<List<double>>();

            foreach (AniDataSet.FrostPredictionDataViewRow row in frostDataTable.Rows)
            {
                var inputs = new List<double>(NumInputs);
                var ideals = new List<double>(1);

                inputs.Add(GetDoubleFromBoolean(row.HasRain));
                inputs.Add(GetDoubleFromBoolean(row.HasClouds));
                inputs.Add(GetDoubleFromBoolean(row.HasSnow));
                inputs.Add(GetDoubleFromBoolean(row.HasStorm));
                inputs.Add(GetDoubleFromBoolean(row.HasWind));
                inputs.Add(row.Low);
                inputs.Add(row.High);
                inputs.Add(row.WeatherCode);
                inputRows.Add(inputs);

                ideals.Add(row.MinutesToDefrost);
                idealRows.Add(ideals);
            }

            var trainInputs = inputRows.Select(r => r.ToArray()).ToArray();
            var trainIdeals = idealRows.Select(r => r.ToArray()).ToArray();

            var trainingSet = new BasicNeuralDataSet(trainInputs, trainIdeals);
            return trainingSet;
        }

        private double GetDoubleFromBoolean(bool value)
        {
            return value ? 1 : -1;
        }

        private static BasicNetwork CreateNeuralNet()
        {
            var network = new BasicNetwork();

            network.AddLayer(new BasicLayer(null, true, NumInputs));
            network.AddLayer(new BasicLayer(null, true, NumInputs * 2));
            network.AddLayer(new BasicLayer(null, true, NumInputs * 2));
            network.AddLayer(new BasicLayer(null, false, 1));

            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }
    }
}