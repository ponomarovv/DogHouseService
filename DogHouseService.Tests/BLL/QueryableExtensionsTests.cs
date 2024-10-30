using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using FluentAssertions;
using DogHouseService.BLL.Extensions;

namespace DogHouseService.Tests.Extensions
{
    public class QueryableExtensionsTests
    {
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private readonly List<TestEntity> _testEntities = new()
        {
            new TestEntity { Id = 3, Name = "Charlie" },
            new TestEntity { Id = 1, Name = "Alpha" },
            new TestEntity { Id = 2, Name = "Bravo" }
        };

        [Fact]
        public void OrderBy_ShouldSortAscending_WhenAscendingIsTrue()
        {
            // Arrange
            var queryable = _testEntities.AsQueryable();

            // Act
            var result = queryable.OrderBy("Id", ascending: true).ToList();

            // Assert
            result.Should().BeInAscendingOrder(e => e.Id);
            result.Select(e => e.Id).Should().Equal(1, 2, 3);
        }

        [Fact]
        public void OrderBy_ShouldSortDescending_WhenAscendingIsFalse()
        {
            // Arrange
            var queryable = _testEntities.AsQueryable();

            // Act
            var result = queryable.OrderBy("Id", ascending: false).ToList();

            // Assert
            result.Should().BeInDescendingOrder(e => e.Id);
            result.Select(e => e.Id).Should().Equal(3, 2, 1);
        }

        [Fact]
        public void OrderBy_ShouldSortByNameAscending_WhenPropertyNameIsName()
        {
            // Arrange
            var queryable = _testEntities.AsQueryable();

            // Act
            var result = queryable.OrderBy("Name", ascending: true).ToList();

            // Assert
            result.Select(e => e.Name).Should().Equal("Alpha", "Bravo", "Charlie");
        }

        [Fact]
        public void OrderBy_ShouldSortByNameDescending_WhenPropertyNameIsName()
        {
            // Arrange
            var queryable = _testEntities.AsQueryable();

            // Act
            var result = queryable.OrderBy("Name", ascending: false).ToList();

            // Assert
            result.Select(e => e.Name).Should().Equal("Charlie", "Bravo", "Alpha");
        }

        [Fact]
        public void OrderBy_ShouldThrowArgumentException_WhenPropertyNameIsInvalid()
        {
            // Arrange
            var queryable = _testEntities.AsQueryable();
            var invalidPropertyName = "NonExistentProperty";

            // Act
            Action act = () => queryable.OrderBy(invalidPropertyName, ascending: true).ToList();

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage($"No property '{invalidPropertyName}' found on 'TestEntity'");
        }
    }
}
