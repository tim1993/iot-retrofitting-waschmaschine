using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

await CreateHostBuilder(args).RunConsoleAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) => Startup.ConfigureServices(services, hostContext))
        .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json").AddJsonFile("appsettings.dev.json", true));