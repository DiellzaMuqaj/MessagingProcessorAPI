using MessagingProcessor.Middleware;
using MessagingProcessor.Persistence;
using MessagingProcessor.Queue;
using MessagingProcessor.Services;
using MessagingProcessor.Simulators;
using SQLitePCL;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Models;
using System.Data;
using Serilog;
using System.Text.Json.Serialization;
using Dapper;
using FluentValidation.AspNetCore;
using MessagingProcessor.Validators;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

// Serilog setup
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));


// Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Messaging API", Version = "v1" });
});

// DI registrations
builder.Services.AddSingleton<IMessageQueue, InMemoryMessageQueue>();
builder.Services.AddSingleton<IMetricsService, MetricsService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IExternalApiSimulator, ExternalApiSimulator>();

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<MessageRequestValidator>();

// SQLite connection
builder.Services.AddSingleton<IDbConnection>(_ =>
    new SqliteConnection(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        policy.WithOrigins("https://localhost:7185") // URL of your dashboard project
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});




Batteries.Init();

var app = builder.Build();

using (var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()))
{
    var logger = loggerFactory.CreateLogger<GuidTypeHandler>();
    SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHandler(logger));
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDbConnection>();
    DatabaseInitializer.Initialize(db);
}
app.UseCors("AllowDashboard");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

