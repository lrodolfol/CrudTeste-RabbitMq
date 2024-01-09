using Serilog;
using WorkerConsumer;
using WorkerCreateUserConsumer.Contracts;
using WorkerCreateUserConsumer.Services;

var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", false)
.Build();

Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .WriteTo.Console()
            .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IEmailService>(new EmailService
            (
                configuration.GetValue<string>("ApplicarionConfigs:MyName")!,
                configuration.GetValue<string>("ApplicarionConfigs:MyEmail")!,
                configuration.GetValue<string>("ApplicarionConfigs:MyPassword")!
            )
        );
        services.AddLogging();
        services.AddSingleton(Log.Logger);
    })
    .Build();

host.Run();
