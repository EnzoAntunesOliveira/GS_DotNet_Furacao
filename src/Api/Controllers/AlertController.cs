using Api.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertPredictionService _predictionService;

        public AlertController(IAlertPredictionService predictionService)
        {
            _predictionService = predictionService;
        }
        
        [HttpPost("predict")]
        public ActionResult<float> Predict([FromBody] AlertPredictionDto dto)
        {
            var severity = _predictionService.PredictSeverity(dto.Feature1, dto.Feature2, dto.Feature3);
            return Ok(severity);
        }
    }
}