using Microsoft.AspNetCore.Mvc;
using YoutubeChannelAPI.Core;
using YoutubeChannelAPI.Models;

namespace YoutubeChannelAPI.Controllers;

[ApiController]
[Route("api/[controller]s")]
[Produces("application/json")]
public class YoutubeChannelController : ControllerBase
{
    private readonly IYoutubeChannelService _service;

    public YoutubeChannelController(IYoutubeChannelService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all channels
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Returns all available channels</response>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<YoutubeChannelDto> GetAll()
    {
        return _service.FindAll().ToYoutubeChannelDtos();
    }

    /// <summary>
    /// Gets a channel
    /// </summary>
    /// <param name="channelId">ID of the channel (UUID, required)</param>
    /// <returns></returns>
    /// <response code="200">Returns a channel</response>
    /// <response code="404">If the channel does not exist</response>
    [HttpGet]
    [Route("{channelId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    public ActionResult<YoutubeChannelDto> GetByChannelId([FromRoute] Guid channelId)
    {
        var youtubeChannel = _service.FindByChannelId(channelId);

        return youtubeChannel.ToYoutubeChannelDto();
    }

    /// <summary>
    /// Creates a new channel
    /// </summary>
    /// <param name="createdChannelDto">Details of the new channel.
    /// If the channel name has less than four and more than 30 characters, this request will fail.
    /// Same if the link has less than one and more than 1000 characters.</param>
    /// <returns></returns>
    /// <response code="201">Returns the newly created channel</response>
    /// <response code="400">If the channel is invalid or if it already exists</response>
    [HttpPost]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    public ActionResult<YoutubeChannelDto> Post([FromBody] CreateYoutubeChannelDto createdChannelDto)
    {
        var createdChannel = _service.Save(createdChannelDto.ToYoutubeChannel());

        var youtubeChannelDto = createdChannel.ToYoutubeChannelDto();
        return CreatedAtAction(nameof(GetByChannelId), new { channelId = youtubeChannelDto.ChannelId },
            youtubeChannelDto);
    }

    /// <summary>
    /// Deletes a channel
    /// </summary>
    /// <param name="channelId">ID of the channel (UUID, required)</param>
    /// <returns></returns>
    /// <response code="204">If the channel was deleted</response>
    /// <response code="404">If the channel does not exist</response>
    [HttpDelete]
    [Route("{channelId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    public ActionResult Delete([FromRoute] Guid channelId)
    {
        _service.Delete(channelId);

        return NoContent();
    }

    /// <summary>
    /// Updates a channel
    /// </summary>
    /// <param name="channelId">ID of the channel (UUID, required)</param>
    /// <param name="updatedChannelDto">Details of the updated channel.
    /// If the channel name has less than four and more than 30 characters, this request will fail.
    /// Same if the link has less than one and more than 1000 characters.</param>
    /// <returns></returns>
    /// <response code="200">If the channel was updated</response>
    /// <response code="400">If the channel id was modified in the body</response>
    /// <response code="404">If the channel does not exist</response>
    [HttpPut]
    [Route("{channelId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status404NotFound)]
    public ActionResult<YoutubeChannelDto> Put(
        [FromRoute] Guid channelId, [FromBody] UpdateYoutubeChannelDto updatedChannelDto
    )
    {
        var updatedChannel = _service.Update(
            channelId, YoutubeChannelDtoConverter.ToYoutubeChannel(updatedChannelDto, channelId)
        );

        return updatedChannel.ToYoutubeChannelDto();
    }
}