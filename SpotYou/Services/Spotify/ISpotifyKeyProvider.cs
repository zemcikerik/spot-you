namespace SpotYou.Services.Spotify
{
    public interface ISpotifyKeyProvider
    {
        string GetSpotifyOAuthClientId();
        string GetSpotifyOAuthClientSecret();
    }
}
