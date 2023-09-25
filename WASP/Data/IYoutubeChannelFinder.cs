namespace WASP.Data;

public interface IYoutubeChannelFinder
{
    YoutubeChannelEntity TryToFindByChannelId(Guid channelId);
}