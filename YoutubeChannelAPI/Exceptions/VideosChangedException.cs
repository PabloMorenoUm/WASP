namespace YoutubeChannelAPI.Exceptions;

public class VideosChangedException: CustomException
{
    public VideosChangedException(string message) : base(message)
    {
    }
}