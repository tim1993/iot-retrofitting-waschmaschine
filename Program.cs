using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await CreateHostBuilder(args).RunConsoleAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) => Startup.ConfigureServices(services, hostContext))
        .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json").AddJsonFile("appsettings.dev.json", true))
        .ConfigureLogging((_, logging) => logging.ClearProviders().AddConsole());