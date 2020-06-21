using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SpotYou.Services.Spotify
{
    public sealed class SpotifyKeyProvider : ISpotifyKeyProvider
    {
        private readonly ILogger<SpotifyKeyProvider> _logger;
        private readonly IConfiguration _configuration;

        public SpotifyKeyProvider(ILogger<SpotifyKeyProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string GetSpotifyOAuthClientId()
        {
            _logger.LogDebug("Spotify OAuth Client Id requested!");
            return _configuration.GetValue<string>(Constants.SpotifyOAuthClientIdPath);
        }

        public string GetSpotifyOAuthClientSecret()
        {
            _logger.LogDebug("Spotify OAuth Client Secret requested!");
            return _configuration.GetValue<string>(Constants.SpotifyOAuthClientSecretPath);
        }
    }
}
