using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpotYou.Models;

namespace SpotYou.Services.Spotify
{
    public interface ISpotifyService : IDisposable
    {
        Task Initialize(CancellationToken cancellationToken);

        Task<string> CreatePlaylist(string name, CancellationToken cancellationToken);
        Task AddToPlaylist(string playlistId, string trackId, CancellationToken cancellationToken);

        Task<IList<ITrack>> SearchTracks(string name, CancellationToken cancellationToken);
    }
}
