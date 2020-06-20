using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpotYou.Services.Youtube;

namespace SpotYou.Services
{
    public sealed class RunnerService : IHostedService
    {
        private readonly ILogger<RunnerService> _logger;
        private readonly IYoutubeService _youtubeService;

        private readonly TaskCompletionSource<bool> _taskCompletionSource;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public RunnerService(ILogger<RunnerService> logger, 
            IYoutubeService youtubeService)
        {
            _logger = logger;
            _youtubeService = youtubeService;

            _taskCompletionSource = new TaskCompletionSource<bool>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: fire a task
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return _taskCompletionSource.Task;
        }
    }
}
