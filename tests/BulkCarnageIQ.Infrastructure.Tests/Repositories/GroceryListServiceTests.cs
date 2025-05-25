using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Contracts;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Tests.SharedUtils.DB;
using BulkCarnageIQ.Tests.SharedUtils.Generators;
using BulkCarnageIQ.Tests.SharedUtils.Seed;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Infrastructure.Tests.Repositories
{
    public class GroceryListServiceTests
    {
        private readonly AppDbContext _dbContext;
        private readonly IGroceryListService _service;

        public GroceryListServiceTests()
        {
            _dbContext = TestDbContextFactory.CreateSqliteDb();
            _service = new GroceryListService(_dbContext);
        }

        [Fact]
        public async Task AutoGenerateFromRecentMealsAsync_AddsMissingItems()
        {
            var userId = Guid.NewGuid().ToString();
            var daysBack = 7;

            var mealEntries = SampleDataSeeder.GenerateMealEntries(userId, 3);
            mealEntries[0].MealName = "MealOne";
            mealEntries[0].Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            mealEntries[0].Day = mealEntries[0].Date.DayOfWeek.ToString();
            mealEntries[0].MeasurementType = "Cup";
            mealEntries[0].MeasurementServings = 1.5f;

            mealEntries[1].MealName = "MealTwo";
            mealEntries[1].Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));
            mealEntries[1].Day = mealEntries[1].Date.DayOfWeek.ToString();
            mealEntries[1].MeasurementType = "Ounces";
            mealEntries[1].MeasurementServings = 2f;

            mealEntries[2].MealName = "MealThree";
            mealEntries[2].Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)); // outside daysBack
            mealEntries[2].Day = mealEntries[2].Date.DayOfWeek.ToString();

            await _dbContext.MealEntries.AddRangeAsync(mealEntries);

            var foodItems = SampleDataSeeder.GenerateFoodItems(2);
            foodItems[0].RecipeName = "MealOne";
            foodItems[0].GroupName = "GroupA";
            foodItems[1].RecipeName = "MealThree";
            foodItems[1].GroupName = "GroupB";
            await _dbContext.FoodItems.AddRangeAsync(foodItems);

            var existingGroceryItem = SampleDataSeeder.GenerateGroceryListItems(userId, 1).First();
            existingGroceryItem.RecipeName = "MealTwo";
            existingGroceryItem.CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            existingGroceryItem.IsFavorite = false;
            existingGroceryItem.IsCompleted = false;
            await _dbContext.GroceryListItems.AddAsync(existingGroceryItem);

            await _dbContext.SaveChangesAsync();

            // Act
            await _service.AutoGenerateFromRecentMealsAsync(userId, daysBack);

            // Assert
            var groceryItems = await _service.GetListForUserAsync(userId);

            groceryItems.ShouldContain(x => x.RecipeName == "MealOne");
            groceryItems.ShouldContain(x => x.RecipeName == "MealTwo");
            groceryItems.ShouldNotContain(x => x.RecipeName == "MealThree");
            groceryItems.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetListForUserAsync_ReturnsOnlyUsersItems()
        {
            var userId = Guid.NewGuid().ToString();
            var otherUserId = Guid.NewGuid().ToString();

            await _dbContext.GroceryListItems.AddRangeAsync(SampleDataSeeder.GenerateGroceryListItems(userId, 2));
            await _dbContext.GroceryListItems.AddRangeAsync(SampleDataSeeder.GenerateGroceryListItems(otherUserId, 2));
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetListForUserAsync(userId);

            result.Count.ShouldBe(2);
            result.All(x => x.UserId == userId).ShouldBeTrue();
        }

        [Fact]
        public async Task AddItemAsync_AddsItemToDatabase()
        {
            var userId = Guid.NewGuid().ToString();
            var item = new GroceryListItem
            {
                RecipeName = "TestRecipe",
                UserId = userId,
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var added = await _service.AddItemAsync(item);
            var dbItem = await _dbContext.GroceryListItems.FindAsync(added.Id);

            dbItem.ShouldNotBeNull();
            dbItem.RecipeName.ShouldBe("TestRecipe");
            dbItem.UserId.ShouldBe(userId);
        }

        [Fact]
        public async Task RemoveItemAsync_RemovesItem()
        {
            var userId = Guid.NewGuid().ToString();
            var item = SampleDataSeeder.GenerateGroceryListItems(userId, 1).First();
            await _dbContext.GroceryListItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            await _service.RemoveItemAsync(item.Id, userId);

            var dbItem = await _dbContext.GroceryListItems.FindAsync(item.Id);
            dbItem.ShouldBeNull();
        }

        [Fact]
        public async Task ToggleCompletedAsync_TogglesCompletionStatus()
        {
            var userId = Guid.NewGuid().ToString();
            var item = SampleDataSeeder.GenerateGroceryListItems(userId, 1).First();
            item.IsCompleted = false;
            await _dbContext.GroceryListItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            await _service.ToggleCompletedAsync(item.Id, userId);

            var dbItem = await _dbContext.GroceryListItems.FindAsync(item.Id);
            dbItem.IsCompleted.ShouldBeTrue();

            await _service.ToggleCompletedAsync(item.Id, userId);
            dbItem = await _dbContext.GroceryListItems.FindAsync(item.Id);
            dbItem.IsCompleted.ShouldBeFalse();
        }

        [Fact]
        public async Task ToggleFavoriteAsync_TogglesFavoriteStatus()
        {
            var userId = Guid.NewGuid().ToString();
            var item = SampleDataSeeder.GenerateGroceryListItems(userId, 1).First();
            item.IsFavorite = false;
            await _dbContext.GroceryListItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            await _service.ToggleFavoriteAsync(item.Id, userId);

            var dbItem = await _dbContext.GroceryListItems.FindAsync(item.Id);
            dbItem.IsFavorite.ShouldBeTrue();

            await _service.ToggleFavoriteAsync(item.Id, userId);
            dbItem = await _dbContext.GroceryListItems.FindAsync(item.Id);
            dbItem.IsFavorite.ShouldBeFalse();
        }

        [Fact]
        public async Task ClearCompletedAsync_RemovesOnlyCompletedItems()
        {
            var userId = Guid.NewGuid().ToString();
            var completed = SampleDataSeeder.GenerateGroceryListItems(userId, 2).ToList();
            completed.ForEach(x => x.IsCompleted = true);

            var notCompleted = SampleDataSeeder.GenerateGroceryListItems(userId, 2).ToList();
            notCompleted.ForEach(x => x.IsCompleted = false);

            await _dbContext.GroceryListItems.AddRangeAsync(completed);
            await _dbContext.GroceryListItems.AddRangeAsync(notCompleted);
            await _dbContext.SaveChangesAsync();

            await _service.ClearCompletedAsync(userId);

            var remaining = await _service.GetListForUserAsync(userId);
            remaining.Count.ShouldBe(2);
            remaining.All(x => x.IsCompleted == false).ShouldBeTrue();
        }
    }
}
