using System.Net.Http.Json;
using AutoMapper;
using DogHouseService.Api;
using DogHouseService.Api.Mapping;
using DogHouseService.Api.Models;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DogHouseService.Tests.API;

public class DogsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IDogService> _dogServiceMock;
    private readonly IMapper _mapper;

    public DogsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _dogServiceMock = new Mock<IDogService>();
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        _mapper = mappingConfig.CreateMapper();
    }

    [Fact]
    public async Task Ping_ReturnsVersion()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _dogServiceMock.Object);
                services.AddSingleton(_mapper);
            });
        }).CreateClient();

        var response = await client.GetAsync("/dogs/ping");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Be("Dogshouseservice.Version1.0.1");
    }

    [Fact]
    public async Task GetDogs_ReturnsDogs()
    {
        var dogs = new List<DogModel>
        {
            new DogModel { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32 },
            new DogModel { Name = "Jessy", Color = "black & white", TailLength = 7, Weight = 14 }
        };
        var dogDtos = _mapper.Map<IEnumerable<DogDto>>(dogs);

        _dogServiceMock.Setup(service => service.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(dogs);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _dogServiceMock.Object);
                services.AddSingleton(_mapper);
            });
        }).CreateClient();

        var response = await client.GetAsync("/dogs");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DogDto>>();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Neo");
        result.Last().Name.Should().Be("Jessy");
    }

    [Fact]
    public async Task CreateDog_CreatesDog()
    {
        var newDogDto = new DogDto { Name = "Doggy", Color = "red", TailLength = 173, Weight = 33 };
        var newDogModel = _mapper.Map<DogModel>(newDogDto);
        var createdDogModel = new DogModel { Id = 1, Name = "Doggy", Color = "red", TailLength = 173, Weight = 33 };
        var createdDogDto = _mapper.Map<DogDto>(createdDogModel);

        _dogServiceMock.Setup(service => service.CreateDogAsync(It.IsAny<DogModel>()))
            .ReturnsAsync(createdDogModel);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _dogServiceMock.Object);
                services.AddSingleton(_mapper);
            });
        }).CreateClient();

        var response = await client.PostAsJsonAsync("/dogs", newDogDto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DogDto>();
        result.Name.Should().Be("Doggy");
        result.Color.Should().Be("red");
        result.TailLength.Should().Be(173);
        result.Weight.Should().Be(33);
    }

    [Fact]
    public async Task CreateDog_ReturnsBadRequestForDuplicateName()
    {
        var newDogDto = new DogDto { Name = "Neo", Color = "red", TailLength = 173, Weight = 33 };

        _dogServiceMock.Setup(service => service.CreateDogAsync(It.IsAny<DogModel>()))
            .ThrowsAsync(new InvalidOperationException("Duplicate name"));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _dogServiceMock.Object);
                services.AddSingleton(_mapper);
            });
        }).CreateClient();

        var response = await client.PostAsJsonAsync("/dogs", newDogDto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDog_ReturnsBadRequestForNegativeTailLength()
    {
        var newDogDto = new DogDto { Name = "Doggy", Color = "red", TailLength = -1, Weight = 33 };

        _dogServiceMock.Setup(service => service.CreateDogAsync(It.IsAny<DogModel>()))
            .ThrowsAsync(new InvalidOperationException("Negative tail length"));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _dogServiceMock.Object);
                services.AddSingleton(_mapper);
            });
        }).CreateClient();

        var response = await client.PostAsJsonAsync("/dogs", newDogDto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
