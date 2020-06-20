using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SpotYou.Services.Youtube
{
    public sealed class YoutubeKeyProvider : IYoutubeKeyProvider
    {
        private readonly ILogger<YoutubeKeyProvider> _logger;
        private readonly IConfiguration _configuration;

        public YoutubeKeyProvider(ILogger<YoutubeKeyProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string GetYoutubeAPIKey()
        {
            _logger.LogDebug("Youtube API key requested!");
            return _configuration.GetValue<string>(Constants.YoutubeAPIKeyPath);
        }
    }
}
