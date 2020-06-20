using System.Threading;
using System.Threading.Tasks;

namespace SpotYou.Services.Youtube
{
    public interface IYoutubeService
    {
        Task Initialize(CancellationToken cancellationToken);
    }
}
