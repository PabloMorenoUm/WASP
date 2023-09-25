namespace YoutubeChannelAPI.Data;

public interface IYoutubeChannelFinder
{
    YoutubeChannelEntity TryToFindByChannelId(Guid channelId);
}