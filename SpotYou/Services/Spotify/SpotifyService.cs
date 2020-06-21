using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace SpotYou.Services.Spotify
{
    // TODO: logging
    public sealed class SpotifyService : ISpotifyService
    {
        private readonly ILogger<SpotifyService> _logger;
        private readonly ISpotifyKeyProvider _keyProvider;

        private Token? _token;
        private SpotifyWebAPI? _spotifyAPI;
        private AuthorizationCodeAuth? _auth;

        private PrivateProfile? _profile;

        public SpotifyService(ILogger<SpotifyService> logger, ISpotifyKeyProvider keyProvider)
        {
            _logger = logger;
            _keyProvider = keyProvider;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            const string serverUri = "http://localhost:26452";
            const Scope scope = Scope.PlaylistReadPrivate | Scope.PlaylistModifyPrivate;

            var clientId = _keyProvider.GetSpotifyOAuthClientId();
            var clientSecret = _keyProvider.GetSpotifyOAuthClientSecret();

            _auth = new AuthorizationCodeAuth(clientId, clientSecret, serverUri, serverUri, scope);
            var authFinishedTaskSource = new TaskCompletionSource<string>();

            _auth.AuthReceived += (sender, payload) => authFinishedTaskSource.SetResult(payload.Code);
            _auth.Start();
            _auth.OpenBrowser();

            var payloadCode = await authFinishedTaskSource.Task;
            _auth.Stop(500);

            _token = await _auth.ExchangeCode(payloadCode);

            _spotifyAPI = new SpotifyWebAPI
            {
                TokenType = _token.TokenType,
                AccessToken = _token.AccessToken
            };

            _profile = await _spotifyAPI.GetPrivateProfileAsync();
        }

        private Task RefreshToken()
        {
            Debug.Assert(_token != null, "_token != null");
            return RefreshToken(_token.RefreshToken);
        }

        private async Task RefreshToken(string code)
        {
            Debug.Assert(_auth != null, "_auth != null");
            Debug.Assert(_spotifyAPI != null, "_spotifyAPI != null");

            _token = await _auth.RefreshToken(code);
            _spotifyAPI.TokenType = _token.TokenType;
            _spotifyAPI.AccessToken = _token.AccessToken;
        }

        public async Task CreatePlaylist(string name, CancellationToken cancellationToken)
        {
            Debug.Assert(_spotifyAPI != null, "Spotify Service is not initialized!");

            if (_token!.IsExpired())
                await RefreshToken();

            if (cancellationToken.IsCancellationRequested)
                return;

            await _spotifyAPI.CreatePlaylistAsync(_profile!.Id, name, false);
        }

        public void Dispose()
        {
            _spotifyAPI?.Dispose();
        }
    }
}
