using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpotYou.Services.Spotify;
using SpotYou.Services.Youtube;

namespace SpotYou.Services
{
    public sealed class RunnerService : IHostedService, IDisposable
    {
        private readonly ILogger<RunnerService> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IYoutubeService _youtubeService;
        private readonly ISpotifyService _spotifyService;

        private readonly TaskCompletionSource<bool> _taskCompletionSource;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public RunnerService(ILogger<RunnerService> logger, 
            IHostApplicationLifetime applicationLifetime,
            IYoutubeService youtubeService,
            ISpotifyService spotifyService)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _youtubeService = youtubeService;
            _spotifyService = spotifyService;

            _taskCompletionSource = new TaskCompletionSource<bool>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => TryDoWork(_cancellationTokenSource.Token).ConfigureAwait(false));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return _taskCompletionSource.Task;
        }

        private async Task TryDoWork(CancellationToken cancellationToken)
        {
            try
            {
                await DoWork(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Runner service has thrown a critical exception!");
                _applicationLifetime.StopApplication();
            }
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            await _youtubeService.Initialize(cancellationToken);
            await _spotifyService.Initialize(cancellationToken);

            await _spotifyService.CreatePlaylist(Constants.ApplicationName, cancellationToken);
        }

        public void Dispose()
        {
            _youtubeService.Dispose();
            _spotifyService.Dispose();
        }
    }
}
