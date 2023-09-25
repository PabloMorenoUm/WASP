namespace WASP.Core;

public class YoutubeChannelService : IYoutubeChannelService
{
    private readonly IYoutubeChannelRepository _repository;

    public YoutubeChannelService(IYoutubeChannelRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<YoutubeChannel> FindAll()
    {
        return _repository.FindAll();
    }

    public YoutubeChannel FindByChannelId(Guid channelId)
    {
        return _repository.FindByChannelId(channelId);
    }

    public YoutubeChannel Save(YoutubeChannel channel)
    {
        return _repository.Save(channel);
    }

    public void Delete(Guid channelId)
    {
        _repository.Remove(channelId);
    }

    public YoutubeChannel Update(Guid channelId, YoutubeChannel channel)
    {
        return _repository.Update(channelId, channel);
    }
}