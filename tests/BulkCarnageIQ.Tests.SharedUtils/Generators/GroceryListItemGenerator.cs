using Bogus;
using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Tests.SharedUtils.Generators
{
    public static class GroceryListItemGenerator
    {
        public static Faker<GroceryListItem> GetFaker(string userId) =>
            new Faker<GroceryListItem>()
                .RuleFor(g => g.UserId, userId)
                .RuleFor(g => g.RecipeName, f => f.Commerce.ProductName())
                .RuleFor(g => g.IsFavorite, f => f.Random.Bool())
                .RuleFor(g => g.IsCompleted, f => f.Random.Bool())
                .RuleFor(g => g.CreatedDate, f => DateOnly.FromDateTime(f.Date.Past(1)))
                .RuleFor(g => g.GroupName, f => f.Commerce.Categories(1).First())
                .RuleFor(g => g.MeasurementType, f => f.PickRandom(new[] { "Cup", "Part", "Ounces", "Slices", "Pieces", "Tbsp", "Tsp" }))
                .RuleFor(g => g.MeasurementServings, f => f.Random.Float(0.1f, 10f));
    }
}
