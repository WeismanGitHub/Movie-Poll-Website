var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var settings = config.GetSection("Settings").Get<Settings>();
var app = builder.Build();

app
    .UseSwagger()
    .UseSwaggerUI()
    .UseSwaggerGen()
    .UseHttpsRedirection()
    .UseAuthorization()
    .MapControllers()
	.UseDefaultFiles()
    .UseResponseCaching()
    .UseStaticFiles();

app.MapFallbackToFile("/index.html");
app.Run();