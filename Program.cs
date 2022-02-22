using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

await CreateHostBuilder(args).Build().RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) => Startup.ConfigureServices(services, hostContext))
        .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json"));