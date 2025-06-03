using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Infrastructure.ML
{
    public class AlertData
    {
        [LoadColumn(0)] public float Feature1;
        [LoadColumn(1)] public float Feature2;
        [LoadColumn(2)] public float Feature3;
        [LoadColumn(3), ColumnName("Label")] public float Severity;
    }

    public class AlertPrediction
    {
        [ColumnName("Score")] public float PredictedSeverity;
    }

    public class AlertModelBuilder
    {
        private readonly string _modelPath = "MLModels/alertModel.zip";

        public void TrainAndSaveModel(string dataFilePath)
        {
            var mlContext = new MLContext(seed: 0);

            IDataView dataView = mlContext.Data.LoadFromTextFile<AlertData>(
                path: dataFilePath, hasHeader: true, separatorChar: ',');

            var trainTestSplit = mlContext.Data.TrainTestSplit(dataView);

            var pipeline = mlContext.Transforms
                .Concatenate("Features", nameof(AlertData.Feature1),
                                       nameof(AlertData.Feature2),
                                       nameof(AlertData.Feature3))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label"));

            var model = pipeline.Fit(trainTestSplit.TrainSet);

            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
            mlContext.Model.Save(model, dataView.Schema, _modelPath);
        }

        public float PredictSeverity(float f1, float f2, float f3)
        {
            var mlContext = new MLContext();
            ITransformer loadedModel = mlContext.Model.Load(_modelPath, out _);
            var predEngine = mlContext.Model.CreatePredictionEngine<AlertData, AlertPrediction>(loadedModel);

            var input = new AlertData
            {
                Feature1 = f1,
                Feature2 = f2,
                Feature3 = f3
            };
            var prediction = predEngine.Predict(input);
            return prediction.PredictedSeverity;
        }
    }
}
