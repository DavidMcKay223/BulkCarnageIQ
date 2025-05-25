using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Tests.SharedUtils.DB;
using BulkCarnageIQ.Tests.SharedUtils.Extensions;
using BulkCarnageIQ.Tests.SharedUtils.Seed;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Infrastructure.Tests.Repositories
{
    public class FoodItemServiceTests
    {
        private readonly List<FoodItem> _sampleFoodItems;

        public FoodItemServiceTests()
        {
            _sampleFoodItems = SampleDataSeeder.GenerateFoodItems(10);
        }

        [Fact]
        public async Task GetFoodItemByName_ShouldReturnCorrectFoodItem_WhenExists()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateInMemoryDb();
            await context.SeedFoodItemsAsync(_sampleFoodItems);
            var service = new FoodItemService(context);

            var expected = _sampleFoodItems[0];

            // Act
            var result = await service.GetFoodItemByName(expected.RecipeName);

            // Assert
            result.ShouldNotBeNull();
            result.RecipeName.ShouldBe(expected.RecipeName);
            result.CaloriesPerServing.ShouldBe(expected.CaloriesPerServing);
        }

        [Fact]
        public async Task GetFoodItemByName_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateInMemoryDb();
            await context.SeedFoodItemsAsync(_sampleFoodItems);
            var service = new FoodItemService(context);

            // Act
            var result = await service.GetFoodItemByName("Non-Existing-Recipe");

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllFoodItems()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateInMemoryDb();
            await context.SeedFoodItemsAsync(_sampleFoodItems);
            var service = new FoodItemService(context);

            // Act
            var results = await service.GetAllAsync();

            // Assert
            results.ShouldNotBeNull();
            results.Count.ShouldBe(_sampleFoodItems.Count);
            results.Select(f => f.RecipeName).ShouldBe(_sampleFoodItems.Select(f => f.RecipeName));
        }

        [Fact]
        public async Task GetAllDictionaryAsync_ShouldReturnEmptyDictionary_WhenInputIsNullOrEmpty()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateInMemoryDb();
            await context.SeedFoodItemsAsync(_sampleFoodItems);
            var service = new FoodItemService(context);

            // Act
            var nullResult = await service.GetAllDictionaryAsync(null);
            var emptyResult = await service.GetAllDictionaryAsync(new List<string>());

            // Assert
            nullResult.ShouldNotBeNull();
            nullResult.ShouldBeEmpty();

            emptyResult.ShouldNotBeNull();
            emptyResult.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAllDictionaryAsync_ShouldReturnCorrectDictionary_WhenRecipeNamesProvided()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateInMemoryDb();
            await context.SeedFoodItemsAsync(_sampleFoodItems);
            var service = new FoodItemService(context);

            var namesToQuery = _sampleFoodItems.Take(3).Select(f => f.RecipeName).ToList();

            // Act
            var result = await service.GetAllDictionaryAsync(namesToQuery);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(namesToQuery.Count);
            foreach (var name in namesToQuery)
            {
                result.ShouldContainKey(name);
                result[name].RecipeName.ShouldBe(name);
            }
        }
    }
}
