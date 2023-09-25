namespace YoutubeChannelAPI.Core;

public interface IYoutubeVideoService
{
    IEnumerable<YoutubeVideo> FindAllByChannelId(Guid channelId);
    YoutubeVideo FindByChannelIdAndVideoId(Guid channelId, Guid videoId);
    YoutubeVideo SaveByChannelId(Guid channelId, YoutubeVideo video);
    void DeleteByChannelIdAndVideoId(Guid channelId, Guid videoId);
    YoutubeVideo UpdateByChannelIdAndVideoId(Guid channelId, Guid videoId, YoutubeVideo video);
}