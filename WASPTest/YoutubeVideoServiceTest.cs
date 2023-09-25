using FluentAssertions;
using NSubstitute;
using WASP.Core;

namespace WASPTest;

public class YoutubeVideoServiceTest
{
    private static readonly Guid ChannelId = Guid.Parse("cec167d7-d7dc-4674-987f-6bf86c4787a7");
    private static readonly Guid VideoId = Guid.Parse("cec167d7-d7dc-4674-987f-6bf86c4787a7");
    private static readonly DateOnly ReleaseDate = new(2023, 8, 31);
    private const string Name = "andrena objects";
    private const string Description = "This week, we learn that andrena is the best company ever!!!";
    
    private readonly YoutubeVideo _video = new(VideoId, ReleaseDate, Name, Description, ChannelId);
    
    private YoutubeVideoService _service = null!;
    private IYoutubeVideoRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IYoutubeVideoRepository>();
    }

    [Test]
    public void FindAllByChannelId_ShouldDelegateToRepository()
    {
        _repository.FindAllByChannelId(ChannelId).Returns(new List<YoutubeVideo> { _video });
        _service = new YoutubeVideoService(_repository);
        
        var result = _service.FindAllByChannelId(ChannelId).ToList();
        
        result.Should().HaveCount(1).And.Equal(_video);
    }

    [Test]
    public void FindByChannelIdAndVideoId_ShouldDelegateToRepository()
    {
        _repository.FindByChannelIdAndVideoId(ChannelId, VideoId).Returns(_video);
        _service = new YoutubeVideoService(_repository);
        
        var result = _service.FindByChannelIdAndVideoId(ChannelId, VideoId);
        
        result.Should().Be(_video);
    }
    
    [Test]
    public void SaveByChannelId_ShouldDelegateToRepository()
    {
        _repository.SaveByChannelId(ChannelId, _video).Returns(_video);
        _service = new YoutubeVideoService(_repository);
        
        var result = _service.SaveByChannelId(ChannelId, _video);

        result.Should().Be(_video);
    }
    
    [Test]
    public void DeleteByChannelIdAndVideoId_ShouldDelegateToRepository()
    {
        _service = new YoutubeVideoService(_repository);
        
        _service.DeleteByChannelIdAndVideoId(ChannelId, VideoId);
        
        _repository.Received().RemoveByChannelIdAndVideoId(ChannelId, VideoId);
    }

    [Test]
    public void UpdateByChannelIdAndVideoId_ShouldDelegateToRepository()
    {
        _repository.UpdateByChannelIdAndVideoId(ChannelId, VideoId, _video).Returns(_video);
        _service = new YoutubeVideoService(_repository);
        
        var result = _service.UpdateByChannelIdAndVideoId(ChannelId, VideoId, _video);
        
        result.Should().Be(_video);
    }
}