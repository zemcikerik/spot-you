using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpotYou.Services.Spotify
{
    public interface ISpotifyService : IDisposable
    {
        Task Initialize(CancellationToken cancellationToken);
        Task CreatePlaylist(string name, CancellationToken cancellationToken);
    }
}
