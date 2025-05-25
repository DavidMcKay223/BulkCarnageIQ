using BulkCarnageIQ.Core.Carnage.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Tests.SharedUtils.Constants
{
    public class TestDefaults
    {
        public string UserName { get; set; } = "test-user";
        public float Age { get; set; } = 25f;
        public Sex Sex { get; set; } = Sex.Male;
        public float HeightInches { get; set; } = 70f; // 5'10"
        public float WeightPounds { get; set; } = 250f; // 250 lbs
        public string ActivityLevel { get; set; } = "Moderate"; // e.g., "Sedentary", "Light", "Moderate", "Active", "Very Active"
        public string GoalType { get; set; } = "lose2"; // e.g., "Lose", "Maintain", "Gain"

        // Calculated properties based on defaults
        public float CalorieGoal { get; set; } = 2500f; // Default calorie goal
        public float ProteinGoal { get; set; } = 175f; // Default protein goal in grams
        public float CarbsGoal { get; set; } = 300f; // Default carbs goal in grams
        public float FatGoal { get; set; } = 70f; // Default fat goal in grams
        public float FiberGoal { get; set; } = 35f; // Default fiber goal in grams
    }
}
