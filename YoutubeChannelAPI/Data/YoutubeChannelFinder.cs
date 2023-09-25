using Microsoft.EntityFrameworkCore;
using YoutubeChannelAPI.Exceptions;

namespace YoutubeChannelAPI.Data;

public class YoutubeChannelFinder: IYoutubeChannelFinder
{
    private readonly YoutubeChannelContext _context;

    public YoutubeChannelFinder(YoutubeChannelContext context)
    {
        _context = context;
    }

    public YoutubeChannelEntity TryToFindByChannelId(Guid channelId)
    {
        var channelEntity =
            _context
                .YoutubeChannelEntities
                .Include(channelEntity => channelEntity.VideoEntities)
                .SingleOrDefault(entity => entity.ChannelId == channelId.ToString());
        if (channelEntity == null)
        {
            throw new NotFoundException(
                $"A YouTube channel from the database with ID: {channelId} could not be found."
            );
        }

        return channelEntity;
    }
}