using CreateUserConsumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

ServiceCollection serviceCollection = new ServiceCollection();
IConfigurationRoot configuration = new ConfigurationBuilder().Build();

serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
var hostBuilder = new HostBuilder().ConfigureServices(services =>
     services.AddHostedService<UserConsumer>());
