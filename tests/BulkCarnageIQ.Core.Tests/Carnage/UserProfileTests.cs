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
                GoalType = _defaults.GoalType
            };

            // Act
            profile.CalculateGoals();

            // Assert – spot check against known expected values (based on formulas)
            profile.CalorieGoal.ShouldBeInRange(2200f, 2400f);  // ~2294 kcal
            profile.ProteinGoal.ShouldBeInRange(165f, 175f);    // ~170g protein
            profile.FatGoal.ShouldBeInRange(60f, 70f);          // ~63.7g fat
            profile.CarbsGoal.ShouldBeInRange(250f, 270f);      // ~260g carbs
            profile.FiberGoal.ShouldBeInRange(30f, 35f);        // ~32g fiber

            // Ensure macro breakdown fits the calorie total
            var totalCaloriesFromMacros =
                (profile.ProteinGoal * 4f) +
                (profile.CarbsGoal * 4f) +
                (profile.FatGoal * 9f);

            totalCaloriesFromMacros.ShouldBeInRange(profile.CalorieGoal - 100, profile.CalorieGoal + 100);
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
