using Bogus;
using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Tests.SharedUtils.Generators
{
    public static class MealEntryGenerator
    {
        private static readonly string[] MealTypes =
            new[] { "Breakfast", "Lunch", "Dinner", "Snack" };

        private static readonly string[] MeasurementTypes =
            new[] { "Cup", "Part", "Ounces", "Slices", "Pieces", "Tbsp", "Tsp" };

        public static Faker<MealEntry> GetFaker(string userId) =>
            new Faker<MealEntry>()
                .RuleFor(m => m.Id, f => 0) // Typically EF sets this; 0 for new instances
                .RuleFor(m => m.UserId, _ => userId)
                .RuleFor(m => m.Date, f => DateOnly.FromDateTime(f.Date.Recent(30)))
                .RuleFor(m => m.Day, (f, m) => m.Date.DayOfWeek.ToString())
                .RuleFor(m => m.MealType, f => f.PickRandom(MealTypes))
                .RuleFor(m => m.MealName, f => f.Commerce.ProductName())
                .RuleFor(m => m.GroupName, f => f.Random.Bool(0.3f) ? f.Commerce.Department() : null)
                .RuleFor(m => m.PortionEaten, f => f.Random.Float(0.25f, 2f))
                .RuleFor(m => m.MeasurementServings, f => f.Random.Bool(0.8f) ? f.Random.Float(0.1f, 3f) : null)
                .RuleFor(m => m.MeasurementType, (f, m) => m.MeasurementServings.HasValue ? f.PickRandom(MeasurementTypes) : null)
                .RuleFor(m => m.Calories, f => f.Random.Float(50f, 1200f))
                .RuleFor(m => m.Protein, f => f.Random.Float(0f, 60f))
                .RuleFor(m => m.Carbs, f => f.Random.Float(0f, 80f))
                .RuleFor(m => m.Fats, f => f.Random.Float(0f, 40f))
                .RuleFor(m => m.Fiber, f => f.Random.Float(0f, 10f));
    }
}
