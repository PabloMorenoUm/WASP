using Microsoft.Extensions.DependencyInjection;
using YoutubeChannelAPI.Data;

namespace YoutubeChannelAPITest;

public static class FactoryExtension
{
    public static void SaveChannelInDatabase(
        this IntegrationTestWebApplicationFactory factory, YoutubeChannelEntity channelEntity
    )
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<YoutubeChannelContext>();

        db.YoutubeChannelEntities.Add(channelEntity);
        db.SaveChanges();
    }
}