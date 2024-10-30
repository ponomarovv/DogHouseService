using DogHouseService.DAL.Data;
using DogHouseService.DAL.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DogHouseService.Tests.DAL
{
    public class DogHouseContextTests
    {
        [Fact]
        public void DogHouseContext_ShouldInitializeDbSet()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DogHouseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Act
            using (var context = new DogHouseContext(options))
            {
                // Assert
                context.Dogs.Should().NotBeNull();
                context.Dogs.Should().BeAssignableTo<DbSet<Dog>>();
            }
        }

        [Fact]
        public void DogHouseContext_ShouldAddDogToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DogHouseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new DogHouseContext(options))
            {
                var dog = new Dog { Name = "TestDog", Color = "Black", TailLength = 10, Weight = 20 };

                // Act
                context.Dogs.Add(dog);
                context.SaveChanges();

                // Assert
                var savedDog = context.Dogs.FirstOrDefault(d => d.Name == "TestDog");
                savedDog.Should().NotBeNull();
                savedDog.Name.Should().Be("TestDog");
                savedDog.Color.Should().Be("Black");
                savedDog.TailLength.Should().Be(10);
                savedDog.Weight.Should().Be(20);
            }
        }
    }
}
