using AutoMapper;
using DogHouseService.Api.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DogHouseService.Tests.API;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void InstallMappers_ShouldAddMapperToServiceCollection()
    {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.InstallMappers();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IMapper>();

            mapper.Should().NotBeNull();
            mapper.ConfigurationProvider.Should().NotBeNull();
        }
}