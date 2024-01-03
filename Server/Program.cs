var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();
app
    .UseSwagger()
    .UseSwaggerUI()
	.UseHttpsRedirection()
	.UseAuthorization()
	.UseDefaultFiles()
    .UseResponseCaching()
    .UseStaticFiles();

app.MapFallbackToFile("/index.html");
app.Run();
