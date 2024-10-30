using AutoMapper;
using DogHouseService.BLL.Models;
using DogHouseService.BLL.Services;
using DogHouseService.DAL.Data;
using DogHouseService.DAL.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DogHouseService.Tests.BLL;

public class DogServiceTests : IDisposable
{
    private readonly DogHouseContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DogService _dogService;

    public DogServiceTests()
    {
        var options = new DbContextOptionsBuilder<DogHouseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensures a unique database per test
            .Options;
        _context = new DogHouseContext(options);
        _mapperMock = new Mock<IMapper>();
        _dogService = new DogService(_context, _mapperMock.Object);
    }

    [Fact]
    public async Task GetDogsAsync_ShouldReturnMappedDogs()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new Dog { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 },
            new Dog { Name = "Max", Color = "Black", TailLength = 8, Weight = 15 }
        };
        await _context.Dogs.AddRangeAsync(dogs);
        await _context.SaveChangesAsync();

        _mapperMock.Setup(m => m.Map<IEnumerable<DogModel>>(It.IsAny<IEnumerable<Dog>>()))
            .Returns(dogs.Select(d => new DogModel { 
                Id = d.Id, Name = d.Name, Color = d.Color, TailLength = d.TailLength, Weight = d.Weight 
            }));

        // Act
        var result = await _dogService.GetDogsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Buddy");
        result.First().Color.Should().Be("Brown");
        result.First().Weight.Should().Be(20);
    }

    [Fact]
    public async Task CreateDogAsync_ShouldThrow_WhenDogWithSameNameExists()
    {
        // Arrange
        var dogModel = new DogModel { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 };
        await _context.Dogs.AddAsync(new Dog { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 });
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _dogService.CreateDogAsync(dogModel));
    }

    [Fact]
    public async Task CreateDogAsync_ShouldThrow_WhenTailLengthIsNegative()
    {
        // Arrange
        var dogModel = new DogModel { Name = "Buddy", Color = "Brown", TailLength = -5, Weight = 20 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _dogService.CreateDogAsync(dogModel));
    }

    [Fact]
    public async Task CreateDogAsync_ShouldCreateDog_WhenDataIsValid()
    {
        // Arrange
        var dogModel = new DogModel { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 };
        var dog = new Dog { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 };

        _mapperMock.Setup(m => m.Map<Dog>(It.IsAny<DogModel>())).Returns(dog);
        _mapperMock.Setup(m => m.Map<DogModel>(It.IsAny<Dog>())).Returns(dogModel);

        // Act
        var result = await _dogService.CreateDogAsync(dogModel);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Buddy");
        result.Color.Should().Be("Brown");
        result.Weight.Should().Be(20);
        (await _context.Dogs.CountAsync()).Should().Be(1);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted(); // Clears the database after each test run
        _context.Dispose();
    }
}
