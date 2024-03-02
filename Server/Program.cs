using API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings = builder.Configuration.Get<Settings>()!;
builder.Services.AddSingleton(settings);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseResponseCaching();
app.UseStaticFiles();
app.MapFallbackToFile("/index.html");
app.MapControllers();

app.Run();
