namespace SpotYou.Services.Youtube
{
    public interface IYoutubeKeyProvider
    {
        string GetYoutubeOAuthClientId();
        string GetYoutubeOAuthClientSecret();
    }
}
