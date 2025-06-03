namespace Application.Interfaces
{
    public interface IAlertPredictionService
    {
        float PredictSeverity(float feature1, float feature2, float feature3);
    }
}