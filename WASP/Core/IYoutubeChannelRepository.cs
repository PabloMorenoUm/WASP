namespace WASP.Core;

public interface IYoutubeChannelRepository
{
    IEnumerable<YoutubeChannel> FindAll();
    YoutubeChannel FindByChannelId(Guid channelId);
    YoutubeChannel Save(YoutubeChannel channel);
    void Remove(Guid channelId);
    YoutubeChannel Update(Guid channelId, YoutubeChannel channel);
}