using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpotYou.Services.Youtube
{
    public interface IYoutubeService : IDisposable
    {
        Task Initialize(CancellationToken cancellationToken);
        IAsyncEnumerable<string> QueryLikedMusicVideos(CancellationToken cancellationToken);
    }
}
