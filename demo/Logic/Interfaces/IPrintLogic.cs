using Microsoft.AspNetCore.Mvc;

namespace demo.Logic.Interfaces
{
    public interface IPrintLogic
    {
        public Task<IActionResult> PrintHelloWorld();
    }
}
