using System.ComponentModel.DataAnnotations;

namespace WASP.Controllers;
/// <summary>
/// A YouTube channel to be updated
/// </summary>
/// <param name="Name">New channel name</param>
/// <param name="Link">New URL link of the channel</param>
public record UpdateYoutubeChannelDto(
    [StringLength(30, MinimumLength = 4)] string Name, [StringLength(1000, MinimumLength = 1)] string Link
);