using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotYou.Services;
using SpotYou.Services.Youtube;

namespace SpotYou
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<RunnerService>();
                    services.AddSingleton<IYoutubeService, YoutubeService>();
                });
        }
    }
}
