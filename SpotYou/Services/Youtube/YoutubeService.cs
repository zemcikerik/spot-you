using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Logging;

namespace SpotYou.Services.Youtube
{
    public sealed class YoutubeService : IYoutubeService
    {
        private const string RequestAllInfo = "snippet";

        private readonly ILogger<YoutubeService> _logger;
        private readonly IYoutubeKeyProvider _keyProvider;
        private YouTubeService? _ytService;

        public YoutubeService(ILogger<YoutubeService> logger, IYoutubeKeyProvider keyProvider)
        {
            _logger = logger;
            _keyProvider = keyProvider;
        }

        public Task Initialize(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Initializing.");

            var initializer = new BaseClientService.Initializer
            {
                ApplicationName = Constants.ApplicationName, 
                ApiKey = _keyProvider.GetYoutubeAPIKey()
            };

            _ytService = new YouTubeService(initializer);

            _logger.LogInformation("Initialized Youtube Service!");
            return Task.CompletedTask;
        }
    }
}
