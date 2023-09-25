using Microsoft.Extensions.DependencyInjection;
using WASP.Data;

namespace WASPTest;

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