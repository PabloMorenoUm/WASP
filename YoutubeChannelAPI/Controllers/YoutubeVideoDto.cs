namespace YoutubeChannelAPI.Controllers;

/// <summary>
/// A YouTube video
/// </summary>
/// <param name="VideoId">Video id as a UUID</param>
/// <param name="ReleaseDate">Date (without clock time) of video release</param>
/// <param name="Name">Video name</param>
/// <param name="Description">Video description</param>
/// <param name="ChannelId">UUID of the channel that this video belongs to</param>
public record YoutubeVideoDto(Guid VideoId, DateOnly ReleaseDate, string Name, string Description, Guid ChannelId);