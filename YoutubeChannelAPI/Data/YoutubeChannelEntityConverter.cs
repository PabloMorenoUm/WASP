using YoutubeChannelAPI.Core;

namespace YoutubeChannelAPI.Data;

public static class YoutubeChannelEntityConverter
{
    public static YoutubeChannel ToYoutubeChannel(this YoutubeChannelEntity entity) =>
        new(Guid.Parse(entity.ChannelId), entity.Name, entity.Link, entity.VideoEntities.ToYoutubeVideos());

    public static IEnumerable<YoutubeChannel> ToYoutubeChannels(this IEnumerable<YoutubeChannelEntity> entities) =>
        entities.Select(ToYoutubeChannel);

    public static YoutubeChannelEntity ToEntity(this YoutubeChannel channel)
    {
        return new YoutubeChannelEntity(0, channel.ChannelId.ToString(), channel.Name, channel.Link,
            channel.Videos.ToYoutubeVideoEntities());
    }
}