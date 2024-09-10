using demo.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace demo.Controllers
{
    [ApiController]
    [Route("helloworld")]
    public class HelloWorldController : ControllerBase
    {
        private readonly ILogger<HelloWorldController> _logger;
        private readonly IPrintLogic _printLogic;
        private readonly IPostgresLogic _postgresLogic;

        public HelloWorldController(ILogger<HelloWorldController> logger, IPrintLogic printLogic, IPostgresLogic postGresLogic)
        {
            _logger = logger;
            _printLogic  = printLogic;
            _postgresLogic = postGresLogic;
        }

        [HttpGet("/print")]
        public async Task<IActionResult> PrintHelloWorldAsync()
        {
            _logger.Log(LogLevel.Information, "HELLO WORLD REQUEST RECIEVED");
            var response = await _printLogic.PrintHelloWorld();
            return response;
        }

        [HttpGet("/connect")]
        public async Task<IActionResult> ConnectToPostGres()
        {
            _logger.Log(LogLevel.Information, "POSTGRESS CONNECT REQUEST RECIEVED");
            var result = await _postgresLogic.HealthCheck();
            return result;
        }
    }
}