
using Application.Interfaces;
using Infrastructure.ML;

namespace Application.Services
{
    public class AlertPredictionService : IAlertPredictionService
    {
        private readonly AlertModelBuilder _modelBuilder;

        public AlertPredictionService(AlertModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public float PredictSeverity(float feature1, float feature2, float feature3)
        {
            return _modelBuilder.PredictSeverity(feature1, feature2, feature3);
        }
    }
}