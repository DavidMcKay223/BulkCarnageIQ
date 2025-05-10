using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class UserProfile
    {
        [Key]
        [Required]
        public required string UserName { get; set; }

        public float Age { get; set; }
        public float HeightInches { get; set; } // in inches
        public float WeightPounds { get; set; } // in pounds
        public required string ActivityLevel { get; set; } // Sedentary, Lightly Active, etc.
        public required string GoalType { get; set; }

        // Calculated Goals
        public float CalorieGoal { get; set; }
        public float ProteinGoal { get; set; }
        public float CarbsGoal { get; set; }
        public float FatGoal { get; set; }
        public float FiberGoal { get; set; }

        public void CalculateGoals()
        {
            float bmr = CalculateBMR();
            float tdee = bmr * GetActivityFactor();
            float adjustedCalories = ApplyGoalAdjustment(tdee);

            CalorieGoal = adjustedCalories;

            float weightKg = PoundsToKg(WeightPounds);
            ProteinGoal = weightKg * 2.0f;
            FatGoal = CalorieGoal * 0.25f / 9f;
            CarbsGoal = (CalorieGoal - (ProteinGoal * 4f + FatGoal * 9f)) / 4f;
            FiberGoal = (CalorieGoal / 1000.0f) * 14.0f;
        }

        private float CalculateBMR()
        {
            float weightKg = PoundsToKg(WeightPounds);
            float heightCm = InchesToCm(HeightInches);
            return 10f * weightKg + 6.25f * heightCm - 5f * Age + 5f;
        }

        private float GetActivityFactor()
        {
            switch (ActivityLevel.ToLower())
            {
                case "sedentary": return 1.2f;
                case "lightly active": return 1.375f;
                case "moderately active": return 1.55f;
                case "very active": return 1.725f;
                case "super active": return 1.9f;
                default: return 1.2f;
            }
        }

        private float ApplyGoalAdjustment(float tdee)
        {
            switch (GoalType.ToLower())
            {
                case "maintain":
                    return tdee;
                case "lose0.5":
                    return tdee - 250f;
                case "lose1":
                    return tdee - 500f;
                case "lose2":
                    return tdee - 1000f;
                case "gain0.5":
                    return tdee + 250f;
                case "gain1":
                    return tdee + 500f;
                default:
                    return tdee;
            }
        }

        private float PoundsToKg(float pounds) => pounds * 0.453592f;
        private float InchesToCm(float inches) => inches * 2.54f;
    }
}
