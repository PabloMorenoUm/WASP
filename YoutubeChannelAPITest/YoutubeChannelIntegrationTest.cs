using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using YoutubeChannelAPI.Controllers;
using YoutubeChannelAPI.Data;

namespace YoutubeChannelAPITest;

public sealed class YoutubeChannelIntegrationTest : IDisposable
{
    private const string ChannelId = "84a3b875-cbd5-4021-8c95-fff99435be44";
    private const string SecondChannelId = "2fe7dd6e-f926-478c-9582-4a9496874cfd";
    private const string Name = "andrena objects ag";
    private const string Link = "https://www.youtube.com/@andrenaobjects";
    private const string NewName = "new channel";
    private const string NewLink = "https://www.youtube.com/channel/new";

    private const string Path = "/api/youtubechannels";
    private IntegrationTestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private YoutubeChannelDto _channelDto = null!;

    [OneTimeSetUp]
    public void OneTimeSetup() => _factory = new IntegrationTestWebApplicationFactory();

    [SetUp]
    public void SetUp()
    {
        _channelDto = new YoutubeChannelDto(Guid.Parse(ChannelId), Name, Link, new List<YoutubeVideoDto>());
        
        _client = _factory.CreateClient();

        YoutubeChannelEntity channelEntity = new(0, ChannelId, Name, Link, new List<YoutubeVideoEntity>());
        _factory.SaveChannelInDatabase(channelEntity);
    }

    public void Dispose()
    {
        _factory.Dispose();
        // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
    }

    [Test]
    public async Task GetAll_ShouldReturnSavedChannel()
    {
        var result = await _client.GetFromJsonAsync<List<YoutubeChannelDto>>(Path);

        result.Should().HaveCount(1).And.AllBeEquivalentTo(_channelDto);
    }

    [Test]
    public async Task Post_ShouldReturnSavedChannel()
    {
        var videoDto = new CreateUpdateYoutubeVideoDto(
            new DateOnly(2023, 09, 19), "video name", "video description"
        );
        var channelDto =
            new CreateYoutubeChannelDto(NewName, NewLink, new List<CreateUpdateYoutubeVideoDto> { videoDto });

        var result = await _client.PostAsJsonAsync(Path, channelDto);
        var json = await result.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<YoutubeChannelDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().HaveStatusCode(HttpStatusCode.Created);
        dto.Should().BeEquivalentTo(channelDto);
        dto?.ChannelId.Should().NotBeEmpty();
    }

    [Test]
    public async Task Post_ShouldReturnSavedChannelWithoutVideos()
    {
        var channelDto =
            new CreateYoutubeChannelDto(NewName, NewLink, null);

        var result = await _client.PostAsJsonAsync(Path, channelDto);
        var json = await result.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<YoutubeChannelDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().HaveStatusCode(HttpStatusCode.Created);
        dto?.ChannelId.Should().NotBeEmpty();
        dto?.Name.Should().Be(NewName);
        dto?.Link.Should().Be(NewLink);
        dto?.Videos.Should().BeEmpty();
    }

    [TestCase(Name, NewLink)]
    [TestCase(NewName, Link)]
    [TestCase(Name, Link)]
    [TestCase(NewName, "")]
    [TestCase("", NewLink)]
    public async Task Post_ShouldReturnBadRequestResult(string name, string link)
    {
        var channelDto = new CreateYoutubeChannelDto(name, link, new List<CreateUpdateYoutubeVideoDto>());
        var result = await _client.PostAsJsonAsync(Path, channelDto);
        result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByChannelId_ShouldReturnSavedChannel()
    {
        var result = await _client.GetFromJsonAsync<YoutubeChannelDto>($"{Path}/{ChannelId}");

        result.Should().BeEquivalentTo(_channelDto);
    }

    [Test]
    public async Task GetByChannelId_ShouldReturnNotFoundResult()
    {
        var result = await _client.GetAsync($"{Path}/{SecondChannelId}");

        result.Should().HaveStatusCode(HttpStatusCode.NotFound, "there is no such channel");
    }

    [Test]
    public async Task Delete_ShouldReturnNoContentResult()
    {
        var result = await _client.DeleteAsync($"{Path}/{ChannelId}");

        result.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_ShouldReturnNotFoundResult()
    {
        var result = await _client.DeleteAsync($"{Path}/{SecondChannelId}");

        result.Should().HaveStatusCode(HttpStatusCode.NotFound, "there is no such channel");
    }

    [Test]
    public async Task Update_ShouldReturnSavedChannel()
    {
        var updatedChannelDto = new UpdateYoutubeChannelDto(NewName, NewLink);
        var result = await _client.PutAsJsonAsync($"{Path}/{ChannelId}", updatedChannelDto);
        var json = await result.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<YoutubeChannelDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.Should().HaveStatusCode(HttpStatusCode.OK);
        dto.Should().BeEquivalentTo(updatedChannelDto);
        dto?.ChannelId.Should().Be(ChannelId);
    }

    [Test]
    public async Task Update_ShouldReturnNotFoundResult()
    {
        var updatedChannelDto = new UpdateYoutubeChannelDto(NewName, NewLink);
        var result = await _client.PutAsJsonAsync($"{Path}/{SecondChannelId}", updatedChannelDto);
        result.Should().HaveStatusCode(HttpStatusCode.NotFound, "there is no such channel");
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