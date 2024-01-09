using CreateUser.API.Handlers;
using CreateUser.API.Model.Request;
using CreateUser.Core.Repository;
using CreateUser.Infrastructure;
using CreateUser.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration config = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
   .AddJsonFile("appsettings.json", optional: false)
   .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
   .Build();

Log.Logger = new LoggerConfiguration()
   .Enrich.FromLogContext()
   .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
   .WriteTo.Console()
   .CreateLogger();

builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.AddScoped<IDataBaseConnection, ConMysqlConnection>();
builder.Services.AddScoped<IUserPostRepository, UserPostRepository>();
builder.Services.AddHostedService<RabbitMqConsumerReply>();
builder.Services.AddScoped<PostHandler>();
builder.Services.AddLogging();
builder.Services.AddSingleton(Log.Logger);


var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.MapPost("api/v1/user", async (PostHandler handler, UserRequest request) =>
{
    var viewResult = await handler.Bootstrap(request);

    return viewResult;
})
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status500InternalServerError)
.Produces(StatusCodes.Status400BadRequest)
.WithName("CreateUser")
.WithTags("User");

app.Run();
