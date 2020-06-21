using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotYou.Models;

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
                AccessToken = _token.AccessToken,
                UseAutoRetry = true
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

        public async Task<string> CreatePlaylist(string name, CancellationToken cancellationToken)
        {
            await AssertInitializedAndRefresh();

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            var playlist = await _spotifyAPI!.CreatePlaylistAsync(_profile!.Id, name, false);
            return playlist.Id;
        }

        public async Task AddToPlaylist(string playlistId, string trackId, CancellationToken cancellationToken)
        {
            await AssertInitializedAndRefresh();

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            var result = await _spotifyAPI!.AddPlaylistTrackAsync(playlistId, $"spotify:track:{trackId}");

            if (result.HasError())
                throw new Exception(result.Error.Message);
        }

        public async Task<IList<ITrack>> SearchTracks(string name, CancellationToken cancellationToken)
        {
            await AssertInitializedAndRefresh();

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            var searchResult = await _spotifyAPI!.SearchItemsEscapedAsync(name, SearchType.Track, 5);

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            if (searchResult.HasError())
                throw new Exception(searchResult.Error.Message);

            var received = searchResult.Tracks?.Items;

            if (received is null)
                return new List<ITrack>();

            return received.Select(track =>
            {
                var artists = track.Artists
                    .Select(artist => artist.Name)
                    .ToList();

                return new Track(track.Id, track.Name, artists);
            }).Cast<ITrack>().ToList();
        }

        private Task AssertInitializedAndRefresh()
        {
            Debug.Assert(_spotifyAPI != null, "Spotify Service is not initialized!");

            if (_token!.IsExpired())
                return RefreshToken();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _spotifyAPI?.Dispose();
        }
    }
}
