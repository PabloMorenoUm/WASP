using YoutubeChannelAPI.Core;

namespace YoutubeChannelAPI.Controllers;

public static class YoutubeVideoDtoConverter
{
    public static YoutubeVideoDto ToYoutubeVideoDto(this YoutubeVideo video) =>
        new(video.VideoId, video.ReleaseDate, video.Name, video.Description, video.ChannelId);

    public static IEnumerable<YoutubeVideoDto> ToYoutubeVideoDtos(this IEnumerable<YoutubeVideo> videos) =>
        videos.Select(ToYoutubeVideoDto);

    public static YoutubeVideo ToYoutubeVideo(this CreateUpdateYoutubeVideoDto dto, Guid channelId) =>
        new(Guid.NewGuid(), dto.ReleaseDate, dto.Name, dto.Description, channelId);

    public static YoutubeVideo ToYoutubeVideo(this CreateUpdateYoutubeVideoDto dto, Guid channelId, Guid videoId) =>
        new(videoId, dto.ReleaseDate, dto.Name, dto.Description, channelId);

    public static IEnumerable<YoutubeVideo> ToYoutubeVideos(
        this IEnumerable<CreateUpdateYoutubeVideoDto> dtos, Guid channelId
    ) => dtos.Select(dto => dto.ToYoutubeVideo(channelId));
}