using System.Collections.Generic;

namespace SpotYou.Models
{
    public interface ITrack
    {
        string Id { get; set; }
        string Name { get; set; }
        IList<string> Artists { get; set; }
    }
}
