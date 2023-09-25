namespace YoutubeChannelAPI.Core;

public interface IYoutubeVideoRepository
{
    IEnumerable<YoutubeVideo> FindAllByChannelId(Guid channelId);
    YoutubeVideo FindByChannelIdAndVideoId(Guid channelId, Guid videoId);
    YoutubeVideo SaveByChannelId(Guid channelId, YoutubeVideo video);
    void RemoveByChannelIdAndVideoId(Guid channelId, Guid videoId);
    YoutubeVideo UpdateByChannelIdAndVideoId(Guid channelId, Guid videoId, YoutubeVideo video);
}