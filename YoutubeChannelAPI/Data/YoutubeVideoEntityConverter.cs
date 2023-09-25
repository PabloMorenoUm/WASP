using System.Collections;
using YoutubeChannelAPI.Core;

namespace YoutubeChannelAPI.Data;

public static class YoutubeVideoEntityConverter
{
    public static YoutubeVideo ToYoutubeVideo(this YoutubeVideoEntity entity) =>
        new(Guid.Parse(entity.VideoId), entity.ReleaseDate, entity.Name, entity.Description, 
            Guid.Parse(entity.ChannelEntity.ChannelId));

    public static IEnumerable<YoutubeVideo> ToYoutubeVideos(this IEnumerable<YoutubeVideoEntity> entities) =>
        entities.Select(ToYoutubeVideo);

    public static YoutubeVideoEntity ToEntity(this YoutubeVideo video) =>
        new(0, video.VideoId.ToString(), video.ReleaseDate, 
            video.Name, video.Description, 0, null!);
    
    public static ICollection<YoutubeVideoEntity> ToYoutubeVideoEntities(this IEnumerable<YoutubeVideo> videos)
    {
        return videos.Select(video => video.ToEntity()).ToList();
    }
}