using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using YoutubeChannelAPI.Controllers;
using YoutubeChannelAPI.Data;

namespace YoutubeChannelAPITest;

public sealed class YoutubeVideoIntegrationTest : IDisposable
{
    private const string ChannelId = "84a3b875-cbd5-4021-8c95-fff99435be44";
    private const string SecondChannelId = "2fe7dd6e-f926-478c-9582-4a9496874cfd";

    private const string VideoId = "cec167d7-d7dc-4674-987f-6bf86c4787a7";
    private const string SecondVideoId = "85bd4505-28df-41aa-bc96-77d48e894c84";
    private const string VideoName = "andrena objects ag is great!!!";
    private const string VideoDescription = "This week, we learn that andrena is the best company ever!!!";
    private readonly DateOnly _releaseDate = new(2023, 8, 31);
    private const string NewVideoName = "new video";
    private const string NewVideoDescription = "new video description";
    private readonly DateOnly _newReleaseDate = new(2023, 10, 12);

    private YoutubeChannelEntity _channelEntity = null!;
    private YoutubeVideoEntity _videoEntity = null!;
    private YoutubeVideoDto _videoDto = null!;

    private const string GoodPath = $"/api/youtubechannels/{ChannelId}/videos";
    private const string BadPath = $"/api/youtubechannels/{SecondChannelId}/videos";
    private IntegrationTestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetup() => _factory = new IntegrationTestWebApplicationFactory();

    [SetUp]
    public void SetUp()
    {
        _client = _factory.CreateClient();

        const int channelDatabaseId = 0;
        _channelEntity = new YoutubeChannelEntity(
            channelDatabaseId, ChannelId, "andrena objects ag", "https://www.youtube.com/@andrenaobjects", 
            new List<YoutubeVideoEntity>()
        );
        _videoEntity = new YoutubeVideoEntity(
            0, VideoId, _releaseDate, VideoName, VideoDescription, channelDatabaseId, _channelEntity
        );
        _channelEntity.VideoEntities.Add(_videoEntity);
        _factory.SaveChannelInDatabase(_channelEntity);

        _videoDto = new YoutubeVideoDto(
            Guid.Parse(VideoId), _releaseDate, VideoName, VideoDescription, Guid.Parse(ChannelId)
        );
    }

    public void Dispose()
    {
        _factory.Dispose();
        // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
    }

    [Test]
    public async Task GetAllByChannelId_ShouldReturnSavedVideo()
    {
        var result = await _client.GetFromJsonAsync<List<YoutubeVideoDto>>(GoodPath);

        result.Should().HaveCount(1).And.Equal(_videoDto);
    }

    [Test]
    public async Task GetAllByChannelId_ShouldReturnNotFoundResult()
    {
        var result = await _client.GetAsync(BadPath);

        result.Should().HaveStatusCode(HttpStatusCode.NotFound, "there is no such channel");
    }

    [Test]
    public async Task GetByChannelIdAndVideoId_ShouldReturnSavedVideo()
    {
        var result = await _client.GetFromJsonAsync<YoutubeVideoDto>($"{GoodPath}/{VideoId}");

        result.Should().Be(_videoDto);
    }

    [TestCase(GoodPath, SecondVideoId)]
    [TestCase(BadPath, VideoId)]
    public async Task GetByChannelIdAndVideoId_ShouldReturnNotFoundResult(string path, string videoId)
    {
        var result = await _client.GetAsync($"{path}/{videoId}");

        result.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Post_ShouldReturnSavedVideo()
    {
        var releaseDate = new DateOnly(2023, 10, 12);
        var createdVideoDto = new CreateUpdateYoutubeVideoDto(releaseDate, NewVideoName, NewVideoDescription);

        var result = await _client.PostAsJsonAsync(GoodPath, createdVideoDto);
        var json = await result.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<YoutubeVideoDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().HaveStatusCode(HttpStatusCode.Created);
        dto.Should().BeEquivalentTo(createdVideoDto);
        dto?.VideoId.Should().NotBeEmpty();
        dto?.ChannelId.Should().Be(ChannelId);
    }

    [TestCase(VideoName, NewVideoDescription)]
    [TestCase(NewVideoName, VideoDescription)]
    [TestCase(VideoName, VideoDescription)]
    [TestCase("", NewVideoDescription)]
    public async Task Post_ShouldReturnBadRequestResult(string name, string description)
    {
        var createdVideoDto = new CreateUpdateYoutubeVideoDto(_newReleaseDate, name, description);
        
        var result = await _client.PostAsJsonAsync(GoodPath, createdVideoDto);
        result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Delete_ShouldReturnNoContentResult()
    {
        var result = await _client.DeleteAsync($"{GoodPath}/{VideoId}");
        result.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [TestCase(GoodPath, SecondVideoId)]
    [TestCase(BadPath, VideoId)]
    public async Task Delete_ShouldReturnNotFoundResult(string path, string videoId)
    {
        var result = await _client.DeleteAsync($"{path}/{videoId}");
        result.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Update_ShouldReturnSavedVideo()
    {
        var updatedVideoDto = new CreateUpdateYoutubeVideoDto(_newReleaseDate, NewVideoName, NewVideoDescription);
        var result = await _client.PutAsJsonAsync($"{GoodPath}/{VideoId}", updatedVideoDto);
        var json = await result.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<YoutubeVideoDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.Should().HaveStatusCode(HttpStatusCode.OK);
        dto.Should().BeEquivalentTo(updatedVideoDto);
        dto?.VideoId.Should().Be(VideoId);
        dto?.ChannelId.Should().Be(ChannelId);
    }
    
    [TestCase(GoodPath, SecondVideoId)]
    [TestCase(BadPath, VideoId)]
    public async Task Update_ShouldReturnNotFoundResult(string path, string videoId)
    {
        var updatedVideoDto = new CreateUpdateYoutubeVideoDto(_newReleaseDate, NewVideoName, NewVideoDescription);
        var result = await _client.PutAsJsonAsync($"{path}/{videoId}", updatedVideoDto);
        result.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [TearDown]
    public void TearDown()
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<YoutubeChannelContext>();

        db.YoutubeChannelEntities.ExecuteDelete();
        db.SaveChanges();
    }
}