namespace YoutubeChannelAPI.Controllers;

/// <summary>
/// A YouTube channel
/// </summary>
/// <param name="ChannelId">Channel id as a UUID</param>
/// <param name="Name">Channel name</param>
/// <param name="Link">URL link of the channel</param>
/// <param name="Videos">List of all videos of this channel</param>
public record YoutubeChannelDto(Guid ChannelId, string Name, string Link, IEnumerable<YoutubeVideoDto> Videos);