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
                "Create a poll where users vote for a movie from a pre-defined list. Additionally, set an expiration date and restrict it to members of a specific Discord server.",
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

var dbContext = app.Services.GetRequiredService<MoviePollsContext>();
dbContext.Database.EnsureCreated();

app.Run();
