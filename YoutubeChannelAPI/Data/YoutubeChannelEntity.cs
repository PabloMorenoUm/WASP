using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeChannelAPI.Data;

public class YoutubeChannelEntity
{
    public YoutubeChannelEntity()
    {
    }

    public YoutubeChannelEntity(int id, string channelId, string name, string link,
        ICollection<YoutubeVideoEntity> videoEntities)
    {
        Id = id;
        ChannelId = channelId;
        Name = name;
        Link = link;
        VideoEntities = videoEntities;
    }

    public int Id { get; set; }
    public string ChannelId { get; set; } = "";
    [StringLength(30, MinimumLength = 4)]
    public string Name { get; set; } = "";
    [StringLength(1000, MinimumLength = 1)]
    public string Link { get; set; } = "";
    public ICollection<YoutubeVideoEntity> VideoEntities { get; set; } = new List<YoutubeVideoEntity>();
}