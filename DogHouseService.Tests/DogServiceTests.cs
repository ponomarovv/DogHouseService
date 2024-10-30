using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DogHouseService.Api.Mapping;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Models;
using DogHouseService.BLL.Services;
using DogHouseService.DAL.Data;
using DogHouseService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DogHouseService.Tests
{
    public class DogServiceTests
    {
        private readonly Mock<DogHouseContext> _contextMock;
        private readonly IMapper _mapper;
        private readonly IDogService _dogService;

        public DogServiceTests()
        {
            _contextMock = new Mock<DogHouseContext>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();
            _dogService = new DogService(_contextMock.Object, _mapper);
        }

        [Fact]
        public async Task GetDogsAsync_ReturnsDogs()
        {
            // Arrange
            var dogs = new List<Dog>
            {
                new Dog { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32 },
                new Dog { Name = "Jessy", Color = "black & white", TailLength = 7, Weight = 14 }
            };
            var dogsQueryable = dogs.AsQueryable();
            _contextMock.Setup(c => c.Dogs).Returns((DbSet<Dog>)dogsQueryable);

            // Act
            var result = await _dogService.GetDogsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Neo", result.First().Name);
            Assert.Equal("Jessy", result.Last().Name);
        }

        [Fact]
        public async Task CreateDogAsync_CreatesDog()
        {
            // Arrange
            var newDog = new DogModel { Name = "Doggy", Color = "red", TailLength = 173, Weight = 33 };
            _contextMock.Setup(c => c.Dogs.Add(It.IsAny<Dog>())).Callback<Dog>(dog => dog.Id = 1);

            // Act
            var result = await _dogService.CreateDogAsync(newDog);

            // Assert
            Assert.Equal("Doggy", result.Name);
            Assert.Equal("red", result.Color);
            Assert.Equal(173, result.TailLength);
            Assert.Equal(33, result.Weight);
            _contextMock.Verify(c => c.Dogs.Add(It.IsAny<Dog>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDogAsync_ThrowsExceptionForDuplicateName()
        {
            // Arrange
            var newDog = new DogModel { Name = "Neo", Color = "red", TailLength = 173, Weight = 33 };
            _contextMock.Setup(c => c.Dogs.AnyAsync(It.IsAny<Dog>())).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _dogService.CreateDogAsync(newDog));
        }

        [Fact]
        public async Task CreateDogAsync_ThrowsExceptionForNegativeTailLength()
        {
            // Arrange
            var newDog = new DogModel { Name = "Doggy", Color = "red", TailLength = -1, Weight = 33 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _dogService.CreateDogAsync(newDog));
        }
    }
}
