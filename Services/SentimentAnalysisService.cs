using Microsoft.ML;
using SentimentAnalysisTool.Models;
using System;
using System.IO;
using System.Reflection;

namespace SentimentAnalysisTool.Services
{
    public class SentimentAnalysisService
    {
        private static readonly string _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sentiment_model.zip");
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public SentimentAnalysisService()
        {
            _mlContext = new MLContext(seed: 0);
            
            // Check if model exists, if not, train it
            if (!File.Exists(_modelPath))
            {
                TrainModel();
            }
            else
            {
                LoadModel();
            }
        }

        private void LoadModel()
        {
            _model = _mlContext.Model.Load(_modelPath, out _);
        }

        private void TrainModel()
        {
            // Load training data from embedded resource
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "SentimentAnalysisTool.Data.sentiment_data.csv";
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new FileNotFoundException($"Could not find embedded training data: {resourceName}");
            
            // Create a temporary file to load the data
            string tempFile = Path.GetTempFileName();
            try
            {
                using (var fileStream = File.Create(tempFile))
                {
                    stream.CopyTo(fileStream);
                }
                
                var dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(
                    tempFile,
                    hasHeader: true,
                    separatorChar: ',');
                    
                // Define data processing pipeline
                var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features", 
                    inputColumnName: nameof(SentimentData.Text))
                    .Append(_mlContext.BinaryClassification.Trainers.FastTree(
                        numberOfLeaves: 50, 
                        numberOfTrees: 50, 
                        minimumExampleCountPerLeaf: 20));

                // Train the model
                _model = pipeline.Fit(dataView);

                // Save the model
                _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        public SentimentPrediction PredictSentiment(string text)
        {
            // Create prediction engine
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);

            // Make prediction
            var prediction = predictionEngine.Predict(new SentimentData { Text = text });
            return prediction;
        }

        public string GetSentimentCategory(SentimentPrediction prediction)
        {
            if (prediction.Score > 0.7)
                return "Positive";
            else if (prediction.Score < 0.3)
                return "Negative";
            else
                return "Neutral";
        }
    }
} 