using System.ComponentModel.DataAnnotations;

namespace YoutubeChannelAPI.Controllers;

/// <summary>
/// A new YouTube channel
/// </summary>
/// <param name="Name">Channel name</param>
/// <param name="Link">URL link of the channel</param>
/// <param name="Videos">List of new videos to be added (optional)</param>
public record CreateYoutubeChannelDto(
    [StringLength(30, MinimumLength = 4)] string Name,
    [StringLength(1000, MinimumLength = 1)] string Link,
    IEnumerable<CreateUpdateYoutubeVideoDto>? Videos
);