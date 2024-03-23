using Microsoft.OpenApi.Models;
using Server;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
Configuration config = builder.Configuration.Get<Configuration>()!;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHsts(options => options.IncludeSubDomains = true);
builder.Services.AddSingleton(new DiscordOauth2(config));
builder.Services.AddSingleton(config);
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<MoviePollsContext>();
builder.Services.AddSwaggerGen(x =>
    x.SwaggerDoc(
        "v1",
        new OpenApiInfo()
        {
            Title = "Movie Polls API",
            Description =
                "Effortlessly create movie polls and seamlessly integrate them with Discord, restricting polls to server members.",
            Version = "1.0"
        }
    )
);

WebApplication app = builder.Build();

app.UseHsts();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.UseDefaultFiles();
app.MapFallbackToFile("/index.html");
app.MapControllers();
app.UseStaticFiles();

app.Run();
