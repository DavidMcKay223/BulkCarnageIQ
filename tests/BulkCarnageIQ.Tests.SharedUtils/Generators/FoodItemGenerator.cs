using Bogus;
using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Tests.SharedUtils.Generators
{
    public static class FoodItemGenerator
    {
        public static Faker<FoodItem> GetFaker() =>
            new Faker<FoodItem>()
                .RuleFor(f => f.RecipeName, f => f.Commerce.ProductName())
                .RuleFor(f => f.Servings, f => f.Random.Float(1, 5))
                .RuleFor(f => f.CaloriesPerServing, f => f.Random.Float(100, 600))
                .RuleFor(f => f.MeasurementServings, f => f.Random.Float(0.1f, 1.0f))
                .RuleFor(f => f.MeasurementType, f => f.PickRandom(new[] { "Cup", "Part", "Ounces", "Slices", "Pieces", "Tbsp", "Tsp" }))
                .RuleFor(f => f.Protein, f => f.Random.Float(5, 40))
                .RuleFor(f => f.Carbs, f => f.Random.Float(10, 100))
                .RuleFor(f => f.Fats, f => f.Random.Float(0, 30))
                .RuleFor(f => f.IsBreakfast, f => f.Random.Bool())
                .RuleFor(f => f.IsLunch, f => f.Random.Bool())
                .RuleFor(f => f.IsDinner, f => f.Random.Bool())
                .RuleFor(f => f.IsSnack, f => f.Random.Bool());
    }
}
