using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace demo.Logic.Interfaces
{
    public interface IPostgresLogic
    {
        public string? dbConnectionString { get; set; }

        public void CreateConnectionString();
        public Task<IActionResult> HealthCheck();
    }
}
