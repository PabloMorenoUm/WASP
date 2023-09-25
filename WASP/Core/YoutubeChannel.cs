namespace WASP.Core;

public record YoutubeChannel(Guid ChannelId, string Name, string Link, IEnumerable<YoutubeVideo> Videos);