using System.ComponentModel.DataAnnotations;

namespace YoutubeChannelAPI.Controllers;

/// <summary>
/// A new YouTube video
/// </summary>
/// <param name="ReleaseDate">Date (without clock time) of video release</param>
/// <param name="Name">Video name</param>
/// <param name="Description">Video description</param>
public record CreateUpdateYoutubeVideoDto(
    [Required] DateOnly ReleaseDate, [StringLength(50, MinimumLength = 4)] string Name, string Description
);