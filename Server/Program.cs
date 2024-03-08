using Server;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHsts(options => options.IncludeSubDomains = true);

Settings settings = builder.Configuration.Get<Settings>()!;
builder.Services.AddSingleton(new DiscordOauth2(settings));
builder.Services.AddSingleton(settings);
builder.Services.AddScoped<HttpClient>();

WebApplication app = builder.Build();

app.UseHsts();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("/index.html");
app.MapControllers();

app.Run();
