using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WASP.Data;
using WASP.Exceptions;

namespace WASPTest;

public class YoutubeChannelFinderTest
{
    private const string ChannelId = "cec167d7-d7dc-4674-987f-6bf86c4787a7";
    private const string SecondChannelId = "85bd4505-28df-41aa-bc96-77d48e894c84";
    private const string Name = "andrena objects ag";
    private const string Link = "https://www.youtube.com/@andrenaobjects";
    
    private readonly YoutubeChannelEntity _entity =
        new(0, ChannelId, Name, Link, new List<YoutubeVideoEntity>());
    
    private YoutubeChannelFinder _finder = null!;
    private YoutubeChannelContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        var entities = new List<YoutubeChannelEntity> { _entity };
        var queryable = entities.AsQueryable();

        var dbSet = Substitute.For<DbSet<YoutubeChannelEntity>, IQueryable<YoutubeChannelEntity>>();
        ((IQueryable<YoutubeChannelEntity>)dbSet).Provider.Returns(queryable.Provider);
        ((IQueryable<YoutubeChannelEntity>)dbSet).Expression.Returns(queryable.Expression);
        ((IQueryable<YoutubeChannelEntity>)dbSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<YoutubeChannelEntity>)dbSet).GetEnumerator().Returns(queryable.GetEnumerator());

        _context = Substitute.For<YoutubeChannelContext>();
        _context.YoutubeChannelEntities.Returns(dbSet);
        _finder = new YoutubeChannelFinder(_context);
    }

    [Test]
    public void TryToFindByChannelId_ShouldDelegateToContext()
    {
        var result = _finder.TryToFindByChannelId(Guid.Parse(ChannelId));

        result.ChannelId.Should().Be(ChannelId);
        result.Name.Should().Be(Name);
        result.Link.Should().Be(Link);
    }

    [Test]
    public void TryToFindByChannelId_ShouldThrowNotFoundException()
    {
        var action = () => _finder.TryToFindByChannelId(Guid.Parse(SecondChannelId));

        action
            .Should().Throw<NotFoundException>()
            .WithMessage($"A YouTube channel from the database with ID: {SecondChannelId} could not be found.");
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}