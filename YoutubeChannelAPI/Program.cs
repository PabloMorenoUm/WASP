using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using YoutubeChannelAPI.Core;
using YoutubeChannelAPI.Data;
using YoutubeChannelAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

var envConStr = Environment.GetEnvironmentVariable("GITLAB_CONNECTION_STRING");
var connectionString = envConStr ?? builder.Configuration["Postgres:ConnectionString"];

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.MapType<DateOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date"
        });

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Youtube API a.k.a. WASP",
            Description = "An ASP.NET Core Web API for managing Youtube channels and videos"
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
}

builder.Services.AddDbContext<YoutubeChannelContext>(options => { options.UseNpgsql(connectionString); });
builder.Services.AddScoped<IYoutubeChannelService, YoutubeChannelService>();
builder.Services.AddScoped<IYoutubeChannelRepository, YoutubeChannelRepository>();
builder.Services.AddScoped<IYoutubeChannelFinder, YoutubeChannelFinder>();
builder.Services.AddScoped<IYoutubeVideoService, YoutubeVideoService>();
builder.Services.AddScoped<IYoutubeVideoRepository, YoutubeVideoRepository>();
builder.Services.AddTransient<ExceptionMiddleware>();

// generate lowercase URLs
builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.EnableTryItOutByDefault(); });
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<YoutubeChannelContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

app.Run();

public partial class Program
{
}