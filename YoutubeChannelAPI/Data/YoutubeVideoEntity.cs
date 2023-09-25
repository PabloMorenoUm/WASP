using System.ComponentModel.DataAnnotations;

namespace YoutubeChannelAPI.Data;

public class YoutubeVideoEntity
{
    public YoutubeVideoEntity()
    {
    }

    public YoutubeVideoEntity(int id, string videoId, DateOnly releaseDate, string name, string description,
        int channelEntityId, YoutubeChannelEntity channelEntity)
    {
        Id = id;
        VideoId = videoId;
        ReleaseDate = releaseDate;
        Name = name;
        Description = description;
        ChannelEntityId = channelEntityId;
        ChannelEntity = channelEntity;
    }

    public int Id { get; init; }
    public string VideoId { get; init; } = "";
    public DateOnly ReleaseDate { get; set; }
    [StringLength(50, MinimumLength = 4)]
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int ChannelEntityId { get; set; }
    public YoutubeChannelEntity ChannelEntity { get; set; } = null!;
}