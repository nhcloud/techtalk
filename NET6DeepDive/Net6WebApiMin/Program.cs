using Microsoft.AspNetCore.Mvc;
using Net6Lib;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IUserService, UserService>();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", ([FromServices] IUserService userService) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapGet("/users", async ([FromServices] IUserService userService) => await GetUsersAsync(userService));
app.MapGet("/sayhello", SayHello);
app.MapGet("/getdataasync", GetDataAsync);
app.MapGet("/getdatasync", GetData);
app.Run();

static string SayHello()
{
    return "Hello, World!";
}
static async IAsyncEnumerable<int> GetDataAsync()
{
    var max = 10;
    for (var counter = 0; counter < 10; counter++)
    {
        await Task.Delay(1000);
        yield return counter;
    }
}
static IEnumerable<int> GetData()
{
    var max = 10;
    for (var counter = 0; counter < max; counter++)
    {
        Task.Delay(1000);
        yield return counter;
    }
}
async Task<ApiResponse<List<User>>> GetUsersAsync(IUserService userService)
{
    return await userService.Get();
}
internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}