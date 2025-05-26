using BulkCarnageIQ.Core.Carnage.Enums;
using BulkCarnageIQ.Core.Carnage.Report;
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
        public Sex Sex { get; set; }
        public float HeightInches { get; set; }
        public float WeightPounds { get; set; }
        public required string ActivityLevel { get; set; }
        public required string GoalType { get; set; }
        public DietType DietType { get; set; } = DietType.None;

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

            var macroSplit = MacroSplit.FromDiet(DietType);

            ProteinGoal = (CalorieGoal * macroSplit.ProteinRatio) / 4f;
            FatGoal = (CalorieGoal * macroSplit.FatRatio) / 9f;
            CarbsGoal = (CalorieGoal * macroSplit.CarbRatio) / 4f;
            FiberGoal = (CalorieGoal / 1000f) * macroSplit.FiberPer1000Calories;
        }

        private float CalculateBMR()
        {
            float weightKg = PoundsToKg(WeightPounds);
            float heightCm = InchesToCm(HeightInches);

            float bmr;

            if (Sex == Sex.Male)
            {
                // Mifflin-St Jeor for males
                bmr = 10f * weightKg + 6.25f * heightCm - 5f * Age + 5f;
            }
            else
            {
                // Mifflin-St Jeor for females
                bmr = 10f * weightKg + 6.25f * heightCm - 5f * Age - 161f;
            }

            return bmr;
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
                default: return 1.2f; // Default to sedentary if level is unknown
            }
        }

        private float ApplyGoalAdjustment(float tdee)
        {
            switch (GoalType.ToLower())
            {
                case "maintain":
                    return tdee;
                case "lose0.5": // Lose 0.5 lbs/week (approx 250 kcal deficit/day)
                    return tdee - 250f;
                case "lose1": // Lose 1 lbs/week (approx 500 kcal deficit/day)
                    return tdee - 500f;
                case "lose2": // Lose 2 lbs/week (approx 1000 kcal deficit/day)
                    return tdee - 1000f;
                case "gain0.5": // Gain 0.5 lbs/week (approx 250 kcal surplus/day)
                    return tdee + 250f;
                case "gain1": // Gain 1 lbs/week (approx 500 kcal surplus/day)
                    return tdee + 500f;
                default:
                    return tdee; // Default to maintain if goal is unknown
            }
        }

        private float PoundsToKg(float pounds) => pounds * 0.453592f;
        private float InchesToCm(float inches) => inches * 2.54f;
    }
}
