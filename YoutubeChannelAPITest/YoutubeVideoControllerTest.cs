using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using YoutubeChannelAPI.Controllers;
using YoutubeChannelAPI.Core;

namespace YoutubeChannelAPITest;

public class YoutubeVideoControllerTest
{
    private static readonly Guid VideoId = Guid.Parse("cec167d7-d7dc-4674-987f-6bf86c4787a7");
    private static readonly Guid ChannelId = Guid.Parse("4ebab376-58d0-4130-b6c1-cbb9bd3a7b45");
    private static readonly DateOnly ReleaseDate = new(2023, 8, 31);
    private const string Name = "andrena objects ag is great!!!";
    private const string Description = "This week, we learn that andrena is the best company ever!!!";

    private readonly YoutubeVideo _video = new(VideoId, ReleaseDate, Name, Description, ChannelId);
    private readonly YoutubeVideoDto _videoDto = new(VideoId, ReleaseDate, Name, Description, ChannelId);
    private readonly CreateUpdateYoutubeVideoDto _createUpdateYoutubeVideoDto = new(ReleaseDate, Name, Description);

    private YoutubeVideoController _controller = null!;
    private IYoutubeVideoService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IYoutubeVideoService>();
    }
    
    [Test]
    public void GetAllByChannelId_ShouldDelegateToService()
    {
        _service.FindAllByChannelId(ChannelId).Returns(new List<YoutubeVideo> { _video });
        _controller = new YoutubeVideoController(_service);
        
        var result = _controller.GetAllByChannelId(ChannelId).ToList();

        result.Should().HaveCount(1).And.Equal(_videoDto);
    }

    [Test]
    public void GetByChannelIdAndVideoId_ShouldDelegateToService()
    {
        _service.FindByChannelIdAndVideoId(ChannelId, VideoId).Returns(_video);
        _controller = new YoutubeVideoController(_service);

        var result = _controller.GetByChannelIdAndVideoId(ChannelId, VideoId).Value;

        result.Should().Be(_videoDto);
    }

    [Test]
    public void Post_ShouldDelegateToService()
    {
        _service.SaveByChannelId(ChannelId, Arg.Any<YoutubeVideo>()).Returns(_video);
        _controller = new YoutubeVideoController(_service);

        var result = _controller.Post(ChannelId, _createUpdateYoutubeVideoDto).Result;

        _service.Received().SaveByChannelId(ChannelId, Arg.Any<YoutubeVideo>());
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public void Delete_ShouldDelegateToService()
    {
        _controller = new YoutubeVideoController(_service);
        
        _controller.Delete(ChannelId, VideoId);
        
        _service.Received().DeleteByChannelIdAndVideoId(ChannelId, VideoId);
    }

    [Test]
    public void Put_ShouldDelegateToService()
    {
        _service.UpdateByChannelIdAndVideoId(ChannelId, VideoId, Arg.Any<YoutubeVideo>()).Returns(_video);
        _controller = new YoutubeVideoController(_service);
        
        var result = _controller.Put(ChannelId, VideoId, _createUpdateYoutubeVideoDto).Value;
        
        _service.Received().UpdateByChannelIdAndVideoId(ChannelId, VideoId, Arg.Any<YoutubeVideo>());
        result.Should().BeEquivalentTo(_createUpdateYoutubeVideoDto);
    }
}