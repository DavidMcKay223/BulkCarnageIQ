using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Enums;
using BulkCarnageIQ.Tests.SharedUtils.Constants;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Tests.Carnage
{
    public class UserProfileTests
    {
        private readonly TestDefaults _defaults = new();

        [Fact]
        public void CalculateGoals_ShouldSetCalorieAndMacroGoalsCorrectly()
        {
            // Arrange
            var profile = new UserProfile
            {
                UserName = _defaults.UserName,
                Age = _defaults.Age,
                Sex = _defaults.Sex,
                HeightInches = _defaults.HeightInches,
                WeightPounds = _defaults.WeightPounds,
                ActivityLevel = _defaults.ActivityLevel,
                GoalType = _defaults.GoalType,
                DietType = _defaults.DietType
            };

            // Act
            profile.CalculateGoals();

            // Assert – spot check against known expected values (based on formulas and DietType.None macro split)
            profile.CalorieGoal.ShouldBeInRange(2290f, 2300f);   // More precise range for ~2294 kcal
            profile.ProteinGoal.ShouldBeInRange(140f, 150f);    // Updated from ~170g to ~143.4g (25% of calories)
            profile.FatGoal.ShouldBeInRange(60f, 70f);          // ~63.7g fat (25% of calories) - still within range
            profile.CarbsGoal.ShouldBeInRange(280f, 290f);      // Updated from ~260g to ~286.8g (50% of calories)
            profile.FiberGoal.ShouldBeInRange(30f, 35f);        // ~32.1g fiber - still within range

            // Ensure macro breakdown fits the calorie total
            var totalCaloriesFromMacros =
                (profile.ProteinGoal * 4f) +
                (profile.CarbsGoal * 4f) +
                (profile.FatGoal * 9f);

            // Give a slightly larger tolerance for float comparisons if needed,
            // but 100 kcal is probably fine given the rounding in individual macros.
            totalCaloriesFromMacros.ShouldBeInRange(profile.CalorieGoal - 5f, profile.CalorieGoal + 5f); // Smaller, more precise range for sum
        }

        [Theory]
        [InlineData("Sedentary", 1.2f)]
        [InlineData("lightly active", 1.375f)]
        [InlineData("MODERATELY ACTIVE", 1.55f)]
        [InlineData("very active", 1.725f)]
        [InlineData("super active", 1.9f)]
        [InlineData("unknown", 1.2f)]
        public void ActivityFactor_ShouldMatchExpectedValue(string activityLevel, float expectedFactor)
        {
            var profile = new UserProfile
            {
                UserName = _defaults.UserName,
                Age = _defaults.Age,
                Sex = _defaults.Sex,
                HeightInches = _defaults.HeightInches,
                WeightPounds = _defaults.WeightPounds,
                ActivityLevel = activityLevel,
                GoalType = "maintain"
            };

            var bmr = typeof(UserProfile)
                .GetMethod("CalculateBMR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(profile, null);

            var factor = typeof(UserProfile)
                .GetMethod("GetActivityFactor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(profile, null);

            factor.ShouldBe(expectedFactor);
        }
    }
}
