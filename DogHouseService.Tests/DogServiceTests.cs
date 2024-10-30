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
using FluentAssertions;
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
            var dogsDbSetMock = new Mock<DbSet<Dog>>();
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.Provider).Returns(dogs.AsQueryable().Provider);
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.Expression).Returns(dogs.AsQueryable().Expression);
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.ElementType).Returns(dogs.AsQueryable().ElementType);
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.GetEnumerator()).Returns(dogs.AsQueryable().GetEnumerator());

            _contextMock.Setup(c => c.Dogs).Returns(dogsDbSetMock.Object);

            // Act
            var result = await _dogService.GetDogsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Neo");
            result.Last().Name.Should().Be("Jessy");
        }

        [Fact]
        public async Task CreateDogAsync_CreatesDog()
        {
            // Arrange
            var newDog = new DogModel { Name = "Doggy", Color = "red", TailLength = 173, Weight = 33 };
            var dogsDbSetMock = new Mock<DbSet<Dog>>();
            dogsDbSetMock.Setup(m => m.Add(It.IsAny<Dog>())).Callback<Dog>(dog => dog.Id = 1);

            _contextMock.Setup(c => c.Dogs).Returns(dogsDbSetMock.Object);

            // Act
            var result = await _dogService.CreateDogAsync(newDog);

            // Assert
            result.Name.Should().Be("Doggy");
            result.Color.Should().Be("red");
            result.TailLength.Should().Be(173);
            result.Weight.Should().Be(33);
            _contextMock.Verify(c => c.Dogs.Add(It.IsAny<Dog>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDogAsync_ThrowsExceptionForDuplicateName()
        {
            // Arrange
            var newDog = new DogModel { Name = "Neo", Color = "red", TailLength = 173, Weight = 33 };
            var dogsDbSetMock = new Mock<DbSet<Dog>>();
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.Provider).Returns(new List<Dog> { new Dog { Name = "Neo" } }.AsQueryable().Provider);
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.Expression).Returns(new List<Dog> { new Dog { Name = "Neo" } }.AsQueryable().Expression);
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.ElementType).Returns(new List<Dog> { new Dog { Name = "Neo" } }.AsQueryable().ElementType);
            dogsDbSetMock.As<IQueryable<Dog>>().Setup(m => m.GetEnumerator()).Returns(new List<Dog> { new Dog { Name = "Neo" } }.AsQueryable().GetEnumerator());

            _contextMock.Setup(c => c.Dogs).Returns(dogsDbSetMock.Object);

            // Act & Assert
            Func<Task> act = () => _dogService.CreateDogAsync(newDog);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateDogAsync_ThrowsExceptionForNegativeTailLength()
        {
            // Arrange
            var newDog = new DogModel { Name = "Doggy", Color = "red", TailLength = -1, Weight = 33 };
            var dogsDbSetMock = new Mock<DbSet<Dog>>();

            _contextMock.Setup(c => c.Dogs).Returns(dogsDbSetMock.Object);

            // Act & Assert
            Func<Task> act = () => _dogService.CreateDogAsync(newDog);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
