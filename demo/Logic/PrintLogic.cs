using demo.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace demo.Logic
{
    public class PrintLogic : IPrintLogic
    {
        private readonly ILogger<PrintLogic> _logger;
        private readonly HttpClient _client;

        public PrintLogic(ILogger<PrintLogic> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<IActionResult> PrintHelloWorld()
        {
            try
            {
                _logger.Log(LogLevel.Information, "PAYLOAD SENDING....");
                var response = await _client.GetAsync("https://sandbox.api.service.nhs.uk/hello-world/hello/world");
                _logger.Log(LogLevel.Information, "RESPONSE RECIEVED");

                var result = new ObjectResult(await response.Content.ReadAsStringAsync());
                result.StatusCode = (int)response.StatusCode;

                return result;
            }
            catch(Exception e)
            {
                _logger.Log(LogLevel.Error, "ERROR: " + e.Message);
                throw;
            }
        }
    }
}
