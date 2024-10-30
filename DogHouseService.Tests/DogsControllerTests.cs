﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Models;
using DogHouseService.DAL.Data;
using DogHouseService.DAL.Models;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace DogHouseService.Tests
{
    public class DogsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public DogsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Ping_ReturnsVersion()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    services.AddDbContext<DogHouseContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DogHouseContext>();
                        db.Database.EnsureCreated();
                    }
                });
            }).CreateClient();

            var response = await client.GetAsync("/dogs/ping");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("\"Dogshouseservice.Version1.0.1\"", responseString);
        }

        [Fact]
        public async Task GetDogs_ReturnsDogs()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    services.AddDbContext<DogHouseContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DogHouseContext>();
                        db.Database.EnsureCreated();

                        db.Dogs.Add(new Dog { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32 });
                        db.Dogs.Add(new Dog { Name = "Jessy", Color = "black & white", TailLength = 7, Weight = 14 });
                        db.SaveChanges();
                    }
                });
            }).CreateClient();

            var response = await client.GetAsync("/dogs");
            response.EnsureSuccessStatusCode();

            var dogs = await response.Content.ReadFromJsonAsync<Dog[]>();
            Assert.Equal(2, dogs.Length);
            Assert.Equal("Neo", dogs[0].Name);
            Assert.Equal("Jessy", dogs[1].Name);
        }

        [Fact]
        public async Task CreateDog_CreatesDog()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    services.AddDbContext<DogHouseContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DogHouseContext>();
                        db.Database.EnsureCreated();
                    }
                });
            }).CreateClient();

            var newDog = new Dog { Name = "Doggy", Color = "red", TailLength = 173, Weight = 33 };
            var response = await client.PostAsJsonAsync("/dogs", newDog);
            response.EnsureSuccessStatusCode();

            var createdDog = await response.Content.ReadFromJsonAsync<Dog>();
            Assert.Equal("Doggy", createdDog.Name);
            Assert.Equal("red", createdDog.Color);
            Assert.Equal(173, createdDog.TailLength);
            Assert.Equal(33, createdDog.Weight);
        }

        [Fact]
        public async Task CreateDog_ReturnsBadRequestForDuplicateName()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    services.AddDbContext<DogHouseContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DogHouseContext>();
                        db.Database.EnsureCreated();

                        db.Dogs.Add(new Dog { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32 });
                        db.SaveChanges();
                    }
                });
            }).CreateClient();

            var newDog = new Dog { Name = "Neo", Color = "red", TailLength = 173, Weight = 33 };
            var response = await client.PostAsJsonAsync("/dogs", newDog);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateDog_ReturnsBadRequestForNegativeTailLength()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    services.AddDbContext<DogHouseContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DogHouseContext>();
                        db.Database.EnsureCreated();
                    }
                });
            }).CreateClient();

            var newDog = new Dog { Name = "Doggy", Color = "red", TailLength = -1, Weight = 33 };
            var response = await client.PostAsJsonAsync("/dogs", newDog);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}