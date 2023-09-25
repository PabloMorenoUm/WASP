using Microsoft.EntityFrameworkCore;
using YoutubeChannelAPI.Core;
using YoutubeChannelAPI.Exceptions;

namespace YoutubeChannelAPI.Data;

public class YoutubeVideoRepository : IYoutubeVideoRepository
{
    private readonly YoutubeChannelContext _context;
    private readonly IYoutubeChannelFinder _finder;

    public YoutubeVideoRepository(YoutubeChannelContext context, IYoutubeChannelFinder finder)
    {
        _context = context;
        _finder = finder;
    }

    public IEnumerable<YoutubeVideo> FindAllByChannelId(Guid channelId)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);
        
        return channelEntity.VideoEntities
            .AsEnumerable()
            .ToYoutubeVideos();
    }

    public YoutubeVideo FindByChannelIdAndVideoId(Guid channelId, Guid videoId)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);

        var videoEntity = TryToFindByVideoId(channelEntity, videoId);

        return videoEntity.ToYoutubeVideo();
    }

    public YoutubeVideo SaveByChannelId(Guid channelId, YoutubeVideo video)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);

        var newEntity = video.ToEntity();
        if (
            channelEntity.VideoEntities.Any(
                videoEntity => videoEntity.Name == newEntity.Name || videoEntity.Description == newEntity.Description
            )
        )
        {
            throw new AlreadyExistsException(
                $"This video (name or description) related to the channel with ID: {channelId} already exists."
            );
        }

        newEntity.ChannelEntityId = channelEntity.Id;
        newEntity.ChannelEntity = channelEntity;
        var entityEntry = _context.Add(newEntity);
        _context.SaveChanges();
        return entityEntry.Entity.ToYoutubeVideo();
    }

    public void RemoveByChannelIdAndVideoId(Guid channelId, Guid videoId)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);
        var videoEntity = TryToFindByVideoId(channelEntity, videoId);
        _context.Remove(videoEntity);
        _context.SaveChanges();
    }

    public YoutubeVideo UpdateByChannelIdAndVideoId(Guid channelId, Guid videoId, YoutubeVideo video)
    {
        var channelEntity = _finder.TryToFindByChannelId(channelId);
        var videoEntity = TryToFindByVideoId(channelEntity, videoId);
        videoEntity.ReleaseDate = video.ReleaseDate;
        videoEntity.Name = video.Name;
        videoEntity.Description = video.Description;
        var entityEntry = _context.Update(videoEntity);
        _context.SaveChanges();
        return entityEntry.Entity.ToYoutubeVideo();
    }

    private static YoutubeVideoEntity TryToFindByVideoId(YoutubeChannelEntity channelEntity, Guid videoId)
    {
        var videoEntity = channelEntity
            .VideoEntities
            .SingleOrDefault(videoEntity => videoEntity.VideoId == videoId.ToString());

        if (videoEntity == null)
        {
            throw new NotFoundException(
                $"A YouTube video from the database with ID: {videoId} from a channel with ID: {channelEntity.ChannelId} could not be found."
            );
        }

        return videoEntity;
    }
}