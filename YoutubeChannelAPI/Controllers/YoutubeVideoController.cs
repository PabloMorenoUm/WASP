using Microsoft.AspNetCore.Mvc;
using YoutubeChannelAPI.Core;
using YoutubeChannelAPI.Models;

namespace YoutubeChannelAPI.Controllers;

[ApiController]
[Route("api/youtubechannels/{channelId:guid}/videos")]
[Produces("application/json")]
public class YoutubeVideoController : ControllerBase
{
    private readonly IYoutubeVideoService _service;

    public YoutubeVideoController(IYoutubeVideoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all videos from a channel
    /// </summary>
    /// <param name="channelId">ID of the channel (UUID, required)</param>
    /// <returns></returns>
    /// <response code="200">Returns all available videos from a channel</response>
    /// <response code="404">If the channel does not exist</response>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    public IEnumerable<YoutubeVideoDto> GetAllByChannelId([FromRoute] Guid channelId)
    {
        return _service.FindAllByChannelId(channelId).ToYoutubeVideoDtos();
    }

    /// <summary>
    /// Gets a video from a channel
    /// </summary>
    /// <param name="channelId">ID of the channel (UUID, required)</param>
    /// <param name="videoId">ID of the video (UUID, required)</param>
    /// <returns></returns>
    /// <response code="200">Returns a video</response>
    /// <response code="404">If the video or the channel does not exist</response>
    [HttpGet]
    [Route(("{videoId:guid}"))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    public ActionResult<YoutubeVideoDto> GetByChannelIdAndVideoId([FromRoute] Guid channelId, [FromRoute] Guid videoId)
    {
        var video = _service.FindByChannelIdAndVideoId(channelId, videoId);
        return video.ToYoutubeVideoDto();
    }

    /// <summary>
    /// Creates a new video
    /// </summary>
    /// <param name="channelId">ID (UUID, required) of the channel that is supposed to have the video</param>
    /// <param name="createdVideoDto">Details of the new video. Be aware of the format of the release date. If the video name has less than four or more than 50 characters, this request will fail.</param>
    /// <returns></returns>
    /// <response code="201">Returns the newly created video</response>
    /// <response code="400">If the video is invalid or if it already exists</response>
    /// <response code="404">If the channel does not exist</response>
    [HttpPost]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    [Consumes("application/json")]
    public ActionResult<YoutubeVideoDto> Post(
        [FromRoute] Guid channelId, [FromBody] CreateUpdateYoutubeVideoDto createdVideoDto
    )
    {
        // does not work:
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var video = _service.SaveByChannelId(channelId, createdVideoDto.ToYoutubeVideo(channelId));
        var dto = video.ToYoutubeVideoDto();
        return CreatedAtAction(
            nameof(GetByChannelIdAndVideoId), new { channelId, videoId = dto.VideoId }, dto
        );
    }

    /// <summary>
    /// Deletes a video
    /// </summary>
    /// <param name="channelId">ID of the channel (UUID, required)</param>
    /// <param name="videoId">ID (UUID, required) of the video that is to be deleted</param>
    /// <returns></returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the video or the channel does not exist</response>
    [HttpDelete]
    [Route("{videoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    public ActionResult Delete([FromRoute] Guid channelId, [FromRoute] Guid videoId)
    {
        _service.DeleteByChannelIdAndVideoId(channelId, videoId);

        return NoContent();
    }

    /// <summary>
    /// Updates a video
    /// </summary>
    /// <param name="channelId">ID of the channel (UUID, required)</param>
    /// <param name="videoId">ID (UUID, required) of the video that is to be updated</param>
    /// <param name="updatedVideoDto">Details of the updated video. All fields must be filled. Be aware of the format of the release date. If the video name has less than four or more than 50 characters, this request will fail.</param>
    /// <returns></returns>
    /// <response code="200">Returns the updated video</response>
    /// <response code="404">If the video or the channel does not exist</response>
    [HttpPut]
    [Route("{videoId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    public ActionResult<YoutubeVideoDto> Put(
        [FromRoute] Guid channelId, [FromRoute] Guid videoId, [FromBody] CreateUpdateYoutubeVideoDto updatedVideoDto
    )
    {
        var video = _service.UpdateByChannelIdAndVideoId(channelId, videoId, updatedVideoDto.ToYoutubeVideo(channelId, videoId));
        return video.ToYoutubeVideoDto();
    }
}