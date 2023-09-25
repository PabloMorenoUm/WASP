namespace WASP.Core;

public interface IYoutubeChannelService
{
    IEnumerable<YoutubeChannel> FindAll();
    YoutubeChannel FindByChannelId(Guid channelId);
    YoutubeChannel Save(YoutubeChannel channel);
    void Delete(Guid channelId);
    YoutubeChannel Update(Guid channelId, YoutubeChannel channel);
}