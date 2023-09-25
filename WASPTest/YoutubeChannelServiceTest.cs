using FluentAssertions;
using NSubstitute;
using WASP.Core;

namespace WASPTest;

public class YoutubeChannelServiceTest
{
    private static readonly Guid ChannelId = Guid.Parse("cec167d7-d7dc-4674-987f-6bf86c4787a7");
    private const string Name = "andrena objects ag";
    private const string Link = "https://www.youtube.com/@andrenaobjects";

    private readonly YoutubeChannel _channel = new(ChannelId, Name, Link, new List<YoutubeVideo>());

    private YoutubeChannelService _service = null!;
    private IYoutubeChannelRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IYoutubeChannelRepository>();
    }

    [Test]
    public void FindAll_ShouldDelegateToRepository()
    {
        var channels = new List<YoutubeChannel>{_channel};
        _repository.FindAll().Returns(channels);
        _service = new YoutubeChannelService(_repository);

        var result = _service.FindAll().ToList();

        result.Should().HaveCount(1).And.Equal(_channel);
    }

    [Test]
    public void FindByChannelId_ShouldDelegateToRepository()
    {
        _repository.FindByChannelId(ChannelId).Returns(_channel);
        _service = new YoutubeChannelService(_repository);

        var result = _service.FindByChannelId(ChannelId);

        result.Should().Be(_channel);
    }

    [Test]
    public void Save_ShouldDelegateToRepository()
    {
        _repository.Save(_channel).Returns(_channel);
        _service = new YoutubeChannelService(_repository);

        var result = _service.Save(_channel);

        result.Should().Be(_channel);
    }

    [Test]
    public void Delete_ShouldDelegateToRepository()
    {
        _service = new YoutubeChannelService(_repository);

        _service.Delete(ChannelId);

        _repository.Received().Remove(ChannelId);
    }

    [Test]
    public void Update_ShouldDelegateToRepository()
    {
        _repository.Update(ChannelId, _channel).Returns(_channel);
        _service = new YoutubeChannelService(_repository);

        var result = _service.Update(ChannelId, _channel);

        result.Should().Be(_channel);
    }
}