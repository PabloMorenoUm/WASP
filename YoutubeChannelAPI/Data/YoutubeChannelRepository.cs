using Microsoft.EntityFrameworkCore;
using YoutubeChannelAPI.Core;
using YoutubeChannelAPI.Exceptions;

namespace YoutubeChannelAPI.Data;

public class YoutubeChannelRepository : IYoutubeChannelRepository
{
    private readonly YoutubeChannelContext _context;
    private readonly IYoutubeChannelFinder _finder;

    public YoutubeChannelRepository(YoutubeChannelContext context, IYoutubeChannelFinder finder)
    {
        _context = context;
        _finder = finder;
    }

    public IEnumerable<YoutubeChannel> FindAll()
    {
        return _context
            .YoutubeChannelEntities
            .Include(channelEntity => channelEntity.VideoEntities)
            .AsEnumerable()
            .ToYoutubeChannels();
    }

    public YoutubeChannel FindByChannelId(Guid channelId)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);

        return channelEntity.ToYoutubeChannel();
    }

    public YoutubeChannel Save(YoutubeChannel channel)
    {
        var newEntity = channel.ToEntity();
        if (
            _context.YoutubeChannelEntities.Any(
                entity => entity.Name == newEntity.Name || entity.Link == newEntity.Link
            )
        )
        {
            throw new AlreadyExistsException("This channel (name or link or both) already exists.");
        }

        var entityEntry = _context.Add(newEntity);
        _context.SaveChanges();
        return entityEntry.Entity.ToYoutubeChannel();
    }

    public void Remove(Guid channelId)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);

        _context.Remove(channelEntity);
        _context.SaveChanges();
    }

    public YoutubeChannel Update(Guid channelId, YoutubeChannel channel)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);
        channelEntity.Name = channel.Name;
        channelEntity.Link = channel.Link;
        var entityEntry = _context.Update(channelEntity);
        _context.SaveChanges();
        return entityEntry.Entity.ToYoutubeChannel();
    }
}