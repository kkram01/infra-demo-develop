using demo;
using demo.Logic;
using demo.Logic.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<IPrintLogic, PrintLogic>();
builder.Services.AddScoped<IPostgresLogic, PostgresLogic>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
// Configured Swagger UI to load as default path.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;

});

//Keep this Save JSON so it will support Dev Platform UI
app.Services.SaveSwaggerJson();
app.UseAuthorization();

app.MapControllers();

string? PORT = Environment.GetEnvironmentVariable("PORT");
if (string.IsNullOrEmpty(PORT))
{
    app.Run();
} else
{
    //The following PORT binding is needed for Cloud Run.
    app.Run($"http://0.0.0.0:{PORT}");
}
