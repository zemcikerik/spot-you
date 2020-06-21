using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Logging;

namespace SpotYou.Services.Youtube
{
    public sealed class YoutubeService : IYoutubeService
    {
        private const string RequestSnippet = "snippet";
        private const int MaxResultsPerPage = 50;
        private const string MusicVideoCategory = "10";

        private readonly ILogger<YoutubeService> _logger;
        private readonly IYoutubeKeyProvider _keyProvider;
        private YouTubeService? _ytService;

        public YoutubeService(ILogger<YoutubeService> logger, IYoutubeKeyProvider keyProvider)
        {
            _logger = logger;
            _keyProvider = keyProvider;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Initializing Youtube Service.");

            var secrets = new ClientSecrets
            {
                ClientId = _keyProvider.GetYoutubeOAuthClientId(),
                ClientSecret = _keyProvider.GetYoutubeOAuthClientSecret()
            };

            const string user = "user";
            var scopes = new[] { YouTubeService.ScopeConstants.YoutubeReadonly };
                
            _logger.LogInformation("Requesting Youtube authorization.");
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, user, cancellationToken);
            _logger.LogInformation("Youtube authorization completed!");

            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = Constants.ApplicationName
            };

            _ytService = new YouTubeService(initializer);

            _logger.LogInformation("Initialized Youtube Service!");
        }

        // TODO: add logging
        public async IAsyncEnumerable<string> QueryLikedMusicVideos([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Debug.Assert(_ytService != null, "Youtube Service is not initialized!");

            var request = _ytService.Videos.List(RequestSnippet);
            request.MyRating = VideosResource.ListRequest.MyRatingEnum.Like;
            request.MaxResults = MaxResultsPerPage;

            do
            {
                var response = await request.ExecuteAsync(cancellationToken);
                var videos = response.Items;

                if (videos.Count == 0)
                    yield break;

                foreach (var video in videos)
                {
                    var snippet = video.Snippet;

                    if (snippet.CategoryId == MusicVideoCategory)
                        yield return snippet.Title;
                }

                request.PageToken = response.NextPageToken;
            } while (request.PageToken != null);
        }

        public void Dispose()
        {
            _ytService?.Dispose();
        }
    }
}
