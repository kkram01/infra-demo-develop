using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace demo.Controllers
{
   
    [ApiController]
    [Route("/healthcheck")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;
        private readonly HealthCheckService _healthCheck;

        public HealthCheckController(ILogger<HealthCheckController> logger, HealthCheckService healthCheck)
        {
            _logger = logger;
            _healthCheck = healthCheck;
        }

        [HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            var result = new ObjectResult(HttpStatusCode.Processing);
            try
            {
                _logger.Log(LogLevel.Information, "HEALTH CHECK REQUEST RECIEVED");
                var report = await _healthCheck.CheckHealthAsync();
                result.StatusCode = 200;
                result.Value = report.Status.ToString();
                return result;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Information, "FAILED TO CALL HEALTH CHECK");
                result.StatusCode = 500;
                result.Value = "Error: " + e.Message;
                return result;
            }
        }
    }
}
