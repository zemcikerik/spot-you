using System.Collections.Generic;

namespace SpotYou.Models
{
    public sealed class Track : ITrack
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Artists { get; set; }

        public Track(string id, string name, IList<string> artists)
        {
            Id = id;
            Name = name;
            Artists = artists;
        }
    }
}
