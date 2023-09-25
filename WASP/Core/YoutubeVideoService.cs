namespace WASP.Core;

public class YoutubeVideoService: IYoutubeVideoService
{
    private readonly IYoutubeVideoRepository _repository;

    public YoutubeVideoService(IYoutubeVideoRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<YoutubeVideo> FindAllByChannelId(Guid channelId)
    {
        return _repository.FindAllByChannelId(channelId);
    }

    public YoutubeVideo FindByChannelIdAndVideoId(Guid channelId, Guid videoId)
    {
        return _repository.FindByChannelIdAndVideoId(channelId, videoId);
    }

    public YoutubeVideo SaveByChannelId(Guid channelId, YoutubeVideo video)
    {
        return _repository.SaveByChannelId(channelId, video);
    }

    public void DeleteByChannelIdAndVideoId(Guid channelId, Guid videoId)
    {
        _repository.RemoveByChannelIdAndVideoId(channelId, videoId);
    }

    public YoutubeVideo UpdateByChannelIdAndVideoId(Guid channelId, Guid videoId, YoutubeVideo video)
    {
        return _repository.UpdateByChannelIdAndVideoId(channelId, videoId, video);
    }
}