using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WASP.Controllers;
using WASP.Core;

namespace WASPTest;

public class YoutubeChannelControllerTest
{
    private static readonly Guid ChannelId = Guid.Parse("cec167d7-d7dc-4674-987f-6bf86c4787a7");
    private const string Name = "andrena objects ag";
    private const string Link = "https://www.youtube.com/@andrenaobjects";

    private readonly YoutubeChannel _channel = new(ChannelId, Name, Link, new List<YoutubeVideo>());
    private readonly YoutubeChannelDto _channelDto = new(ChannelId, Name, Link, new List<YoutubeVideoDto>());

    private YoutubeChannelController _controller = null!;
    private IYoutubeChannelService _service = null!;

    [SetUp]
    public void Setup()
    {
        _service = Substitute.For<IYoutubeChannelService>();
    }

    [Test]
    public void GetAll_ShouldDelegateToService()
    {
        var channels = new List<YoutubeChannel> { _channel };
        _service.FindAll().Returns(channels);
        _controller = new YoutubeChannelController(_service);

        var result = _controller.GetAll().ToList();

        result.Should().HaveCount(1).And.AllBeEquivalentTo(_channelDto);
    }

    [Test]
    public void GetByChannelId_ShouldDelegateToService()
    {
        _service.FindByChannelId(ChannelId).Returns(_channel);
        _controller = new YoutubeChannelController(_service);

        var result = _controller.GetByChannelId(ChannelId).Value;

        _service.Received().FindByChannelId(ChannelId);

        result.Should().BeEquivalentTo(_channelDto);
    }

    [Test]
    public void Post_ShouldDelegateToService()
    {
        _service.Save(Arg.Any<YoutubeChannel>()).Returns(_channel);
        _controller = new YoutubeChannelController(_service);

        var result = _controller.Post(
            new CreateYoutubeChannelDto(Name, Link, new List<CreateUpdateYoutubeVideoDto>())
        ).Result;

        _service.Received().Save(Arg.Any<YoutubeChannel>());
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public void Delete_ShouldDelegateToService()
    {
        _controller = new YoutubeChannelController(_service);

        _controller.Delete(ChannelId);

        _service.Received().Delete(ChannelId);
    }

    [Test]
    public void Put_ShouldDelegateToService()
    {
        _service.Update(ChannelId, Arg.Any<YoutubeChannel>()).Returns(_channel);
        _controller = new YoutubeChannelController(_service);

        var result = _controller.Put(ChannelId, new UpdateYoutubeChannelDto(Name, Link)).Value;

        _service.Received().Update(ChannelId, Arg.Any<YoutubeChannel>());
        result.Should().BeEquivalentTo(_channelDto);
    }
}