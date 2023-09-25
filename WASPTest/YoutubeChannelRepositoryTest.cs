using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NSubstitute;
using WASP.Core;
using WASP.Data;
using WASP.Exceptions;

namespace WASPTest;

public class YoutubeChannelRepositoryTest
{
    private static readonly Guid ChannelId = Guid.Parse("cec167d7-d7dc-4674-987f-6bf86c4787a7");
    private static readonly Guid SecondChannelId = Guid.Parse("85bd4505-28df-41aa-bc96-77d48e894c84");
    private const string Name = "andrena objects ag";
    private const string Link = "https://www.youtube.com/@andrenaobjects";
    private const string NewName = "newName";
    private const string NewLink = "newLink";

    private readonly YoutubeChannel _channel = new(ChannelId, Name, Link, new List<YoutubeVideo>());

    private readonly YoutubeChannelEntity _entity =
        new(0, ChannelId.ToString(), Name, Link, new List<YoutubeVideoEntity>());

    private YoutubeChannelRepository _repository = null!;
    private YoutubeChannelContext _context = null!;
    private IYoutubeChannelFinder _finder = null!;

    private IStateManager _stateManager = null!;
    private IRuntimeEntityType _entityType = null!;

    [SetUp]
    public void SetUp()
    {
        _stateManager = Substitute.For<IStateManager>();
        _entityType = Substitute.For<IRuntimeEntityType>();
        _entityType.EmptyShadowValuesFactory.Returns(() => new MultiSnapshot());
        _entityType.Counts.Returns(new PropertyCounts(1, 1, 1, 1, 1, 1));

        var entities = new List<YoutubeChannelEntity> { _entity };
        var queryable = entities.AsQueryable();

        var dbSet = Substitute.For<DbSet<YoutubeChannelEntity>, IQueryable<YoutubeChannelEntity>>();
        ((IQueryable<YoutubeChannelEntity>)dbSet).Provider.Returns(queryable.Provider);
        ((IQueryable<YoutubeChannelEntity>)dbSet).Expression.Returns(queryable.Expression);
        ((IQueryable<YoutubeChannelEntity>)dbSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<YoutubeChannelEntity>)dbSet).GetEnumerator().Returns(queryable.GetEnumerator());

        _context = Substitute.For<YoutubeChannelContext>();
        _context.YoutubeChannelEntities.Returns(dbSet);
        _finder = Substitute.For<IYoutubeChannelFinder>();
        _finder.TryToFindByChannelId(ChannelId).Returns(_entity);
        _repository = new YoutubeChannelRepository(_context, _finder);
    }

    [Test]
    public void FindAll_ShouldDelegateToContext()
    {
        var result = _repository.FindAll().ToList();

        result.Should().HaveCount(1).And.AllBeEquivalentTo(_channel);
    }

    [Test]
    public void FindByChannelId_ShouldDelegateToFinder()
    {
        var result = _repository.FindByChannelId(ChannelId);

        result.Should().BeEquivalentTo(_channel);
    }

    [Test]
    public void Save_ShouldDelegateToContext()
    {
        var channel = new YoutubeChannel(SecondChannelId, NewName, NewLink, new List<YoutubeVideo>());

        var entry = new InternalEntityEntry(_stateManager, _entityType, channel.ToEntity());
        var entityEntry = new EntityEntry<YoutubeChannelEntity>(entry);

        _context.Add(Arg.Any<YoutubeChannelEntity>()).Returns(entityEntry);
        _repository = new YoutubeChannelRepository(_context, _finder);

        var result = _repository.Save(channel);

        _context.Received(1).Add(Arg.Any<YoutubeChannelEntity>());
        _context.Received(1).SaveChanges();
        result.Should().BeEquivalentTo(channel);
    }

    [TestCase(Name, NewLink)]
    [TestCase(NewName, Link)]
    public void Save_ShouldThrowAlreadyExistsExceptionForSameNameOrLink(string name, string link)
    {
        var channel = new YoutubeChannel(SecondChannelId, name, link, new List<YoutubeVideo>());

        var entry = new InternalEntityEntry(_stateManager, _entityType, channel.ToEntity());
        var entityEntry = new EntityEntry<YoutubeChannelEntity>(entry);

        _context.Add(Arg.Any<YoutubeChannelEntity>()).Returns(entityEntry);
        _repository = new YoutubeChannelRepository(_context, _finder);

        var action = () => _repository.Save(channel);
        action.Should()
            .Throw<AlreadyExistsException>("This channel (name or link or both) already exists in the database.");
    }

    [Test]
    public void Save_ShouldThrowAlreadyExistsExceptionForSameNameAndLink()
    {
        var action = () => _repository.Save(_channel);
        action.Should()
            .Throw<AlreadyExistsException>("This channel (name or link or both) already exists in the database.");
    }

    [Test]
    public void Remove_ShouldDelegateToContextAndFinder()
    {
        _repository.Remove(ChannelId);

        _context.Received(1).Remove(Arg.Any<YoutubeChannelEntity>());
        _context.Received(1).SaveChanges();
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [Test]
    public void Update_ShouldDelegateToContextAndFinder()
    {
        var channel = new YoutubeChannel(ChannelId, NewName, NewLink, new List<YoutubeVideo>());

        var entry = new InternalEntityEntry(_stateManager, _entityType, channel.ToEntity());
        var entityEntry = new EntityEntry<YoutubeChannelEntity>(entry);

        _context.Update(Arg.Any<YoutubeChannelEntity>()).Returns(entityEntry);
        _repository = new YoutubeChannelRepository(_context, _finder);
        
        var result = _repository.Update(ChannelId, _channel);

        result.Should().BeEquivalentTo(channel);
        _finder.Received(1).TryToFindByChannelId(ChannelId);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}