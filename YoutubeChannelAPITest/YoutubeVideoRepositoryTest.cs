using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NSubstitute;
using YoutubeChannelAPI.Core;
using YoutubeChannelAPI.Data;
using YoutubeChannelAPI.Exceptions;

namespace YoutubeChannelAPITest;

public class YoutubeVideoRepositoryTest
{
    private static readonly Guid ChannelId = Guid.Parse("cec167d7-d7dc-4674-987f-6bf86c4787a7");
    private static readonly Guid VideoId = Guid.Parse("382b49c6-f39f-4d2c-b7b4-7279b5ef3982");
    private static readonly Guid SecondVideoId = Guid.Parse("85bd4505-28df-41aa-bc96-77d48e894c84");
    private static readonly DateOnly ReleaseDate = new(2023, 8, 31);
    private const string Name = "andrena objects is great!";
    private const string Description = "This week, we learn that andrena is the best company ever!!!";
    private static readonly DateOnly NewReleaseDate = new(2023, 9, 1);
    private const string NewName = "New Name";
    private const string NewDescription = "New Description";

    private readonly YoutubeVideo _video = new(VideoId, ReleaseDate, Name, Description, ChannelId);

    private YoutubeChannelEntity _channelEntity = null!;
    private YoutubeVideoEntity _videoEntity = null!;

    private YoutubeVideoRepository _videoRepository = null!;
    private IYoutubeChannelFinder _finder = null!;
    private YoutubeChannelContext _context = null!;

    private IStateManager _stateManager = null!;
    private IRuntimeEntityType _entityType = null!;

    [SetUp]
    public void SetUp()
    {
        _channelEntity =
            new YoutubeChannelEntity(0, ChannelId.ToString(), "andrena objects ag",
                "https://www.youtube.com/@andrenaobjects", new List<YoutubeVideoEntity>());
        _videoEntity =
            new YoutubeVideoEntity(0, VideoId.ToString(), ReleaseDate, Name, Description,
                _channelEntity.Id, _channelEntity);
        _channelEntity.VideoEntities.Add(_videoEntity);

        _stateManager = Substitute.For<IStateManager>();
        _entityType = Substitute.For<IRuntimeEntityType>();
        _entityType.EmptyShadowValuesFactory.Returns(() => new MultiSnapshot());
        _entityType.Counts.Returns(new PropertyCounts(1, 1, 1, 1, 1, 1));

        var videoEntities = new List<YoutubeVideoEntity> { _videoEntity };
        var queryable = videoEntities.AsQueryable();

        var dbSet = Substitute.For<DbSet<YoutubeVideoEntity>, IQueryable<YoutubeVideoEntity>>();
        ((IQueryable<YoutubeVideoEntity>)dbSet).Provider.Returns(queryable.Provider);
        ((IQueryable<YoutubeVideoEntity>)dbSet).Expression.Returns(queryable.Expression);
        ((IQueryable<YoutubeVideoEntity>)dbSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<YoutubeVideoEntity>)dbSet).GetEnumerator().Returns(queryable.GetEnumerator());

        _context = Substitute.For<YoutubeChannelContext>();
        _context.YoutubeVideoEntities.Returns(dbSet);
        _finder = Substitute.For<IYoutubeChannelFinder>();
        _finder.TryToFindByChannelId(ChannelId).Returns(_channelEntity);
        _videoRepository = new YoutubeVideoRepository(_context, _finder);
    }

    [Test]
    public void FindAllByChannelId_ShouldExtractVideos()
    {
        var result = _videoRepository.FindAllByChannelId(ChannelId).ToList();

        result.Should().HaveCount(1).And.Equal(_video);
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void FindByChannelIdAndVideoId_ShouldExtractVideo()
    {
        var result = _videoRepository.FindByChannelIdAndVideoId(ChannelId, VideoId);

        result.Should().Be(_video);
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void FindByChannelIdAndVideoId_ShouldThrowNotFoundException()
    {
        var action = () => _videoRepository.FindByChannelIdAndVideoId(ChannelId, SecondVideoId);

        action.Should().Throw<NotFoundException>();
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void Save_ShouldDelegateToContext()
    {
        var video = new YoutubeVideo(SecondVideoId, NewReleaseDate, NewName, NewDescription, ChannelId);
        var videoEntity = new YoutubeVideoEntity(
            0, SecondVideoId.ToString(), NewReleaseDate, NewName, NewDescription, 
            _channelEntity.Id, _channelEntity
        );

        var entry = new InternalEntityEntry(_stateManager, _entityType, videoEntity);
        var entityEntry = new EntityEntry<YoutubeVideoEntity>(entry);

        _context.Add(Arg.Any<YoutubeVideoEntity>()).Returns(entityEntry);
        _videoRepository = new YoutubeVideoRepository(_context, _finder);

        var result = _videoRepository.SaveByChannelId(ChannelId, video);

        result.Should().Be(video);
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [TestCase(Name, NewDescription)]
    [TestCase(NewName, Description)]
    public void Save_ShouldThrowAlreadyExistsExceptionForSameNameOrDescription(string name, string description)
    {
        var video = new YoutubeVideo(SecondVideoId, ReleaseDate, name, description, ChannelId);
        var videoEntity = new YoutubeVideoEntity(
            0, SecondVideoId.ToString(), ReleaseDate, name, description, _channelEntity.Id, _channelEntity
        );

        var entry = new InternalEntityEntry(_stateManager, _entityType, videoEntity);
        var entityEntry = new EntityEntry<YoutubeVideoEntity>(entry);

        _context.Add(Arg.Any<YoutubeVideoEntity>()).Returns(entityEntry);
        _videoRepository = new YoutubeVideoRepository(_context, _finder);

        var action = () => _videoRepository.SaveByChannelId(ChannelId, video);

        action.Should()
            .Throw<AlreadyExistsException>();
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void Save_ShouldThrowAlreadyExistsExceptionForSameNameAndDescription()
    {
        var action = () => _videoRepository.SaveByChannelId(ChannelId, _video);

        action.Should()
            .Throw<AlreadyExistsException>();
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void Remove_ShouldDelegateToContext()
    {
        _videoRepository.RemoveByChannelIdAndVideoId(ChannelId, VideoId);

        _context.Received(1).Remove(_videoEntity);
        _context.Received(1).SaveChanges();
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void Remove_ShouldThrowNotFoundException()
    {
        var action = () => _videoRepository.RemoveByChannelIdAndVideoId(ChannelId, SecondVideoId);
        action.Should().Throw<NotFoundException>();
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void Update_ShouldDelegateToContext()
    {
        var video = new YoutubeVideo(VideoId, NewReleaseDate, NewName, NewDescription, ChannelId);
        var videoEntity = new YoutubeVideoEntity(
            0, VideoId.ToString(), NewReleaseDate, NewName, NewDescription, 
            _channelEntity.Id, _channelEntity
        );
        
        var entry = new InternalEntityEntry(_stateManager, _entityType, videoEntity);
        var entityEntry = new EntityEntry<YoutubeVideoEntity>(entry);
        
        _context.Update(Arg.Any<YoutubeVideoEntity>()).Returns(entityEntry);
        _videoRepository = new YoutubeVideoRepository(_context, _finder);
        
        var result = _videoRepository.UpdateByChannelIdAndVideoId(ChannelId, VideoId, video);
        
        result.Should().Be(video);
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void Update_ShouldThrowNotFoundException()
    {
        var action = () => _videoRepository.UpdateByChannelIdAndVideoId(ChannelId, SecondVideoId, _video);
        
        action.Should().Throw<NotFoundException>();
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}