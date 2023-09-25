namespace WASP.Core;

public record YoutubeVideo(Guid VideoId, DateOnly ReleaseDate, string Name, string Description, Guid ChannelId);