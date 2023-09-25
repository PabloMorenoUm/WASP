using YoutubeChannelAPI.Core;

namespace YoutubeChannelAPI.Controllers;

public static class YoutubeChannelDtoConverter
{
    public static YoutubeChannel ToYoutubeChannel(this CreateYoutubeChannelDto dto)
    {
        var channelId = Guid.NewGuid();
        return new YoutubeChannel(
            channelId, dto.Name, dto.Link, dto.Videos?.ToYoutubeVideos(channelId) ?? new List<YoutubeVideo>()
        );
    }
    
    public static YoutubeChannel ToYoutubeChannel(this UpdateYoutubeChannelDto dto, Guid channelId)
    {
        return new YoutubeChannel(channelId, dto.Name, dto.Link, new List<YoutubeVideo>());
    }

    public static YoutubeChannelDto ToYoutubeChannelDto(this YoutubeChannel channel) =>
        new(channel.ChannelId, channel.Name, channel.Link, channel.Videos.ToYoutubeVideoDtos());

    public static IEnumerable<YoutubeChannelDto> ToYoutubeChannelDtos(this IEnumerable<YoutubeChannel> channels) =>
        channels.Select(ToYoutubeChannelDto);
}