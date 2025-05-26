using BulkCarnageIQ.Core.Carnage.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage.Report
{
    public class MacroSplit
    {
        public float ProteinRatio { get; private set; } // As a decimal, e.g., 0.30 for 30%
        public float CarbRatio { get; private set; }   // As a decimal, e.g., 0.40 for 40%
        public float FatRatio { get; private set; }    // As a decimal, e.g., 0.30 for 30%
        public float FiberPer1000Calories { get; private set; } // Grams of fiber per 1000 calories

        private MacroSplit(float protein, float carbs, float fat, float fiberPer1000)
        {
            ProteinRatio = protein;
            CarbRatio = carbs;
            FatRatio = fat;
            FiberPer1000Calories = fiberPer1000;
        }

        public static MacroSplit FromDiet(DietType dietType)
        {
            switch (dietType)
            {
                case DietType.Balanced:
                    return new MacroSplit(0.30f, 0.40f, 0.30f, 14.0f); // 30% protein, 40% carbs, 30% fat
                case DietType.Keto:
                    return new MacroSplit(0.20f, 0.05f, 0.75f, 14.0f); // 20% protein, 5% carbs, 75% fat
                case DietType.Paleo:
                    return new MacroSplit(0.30f, 0.30f, 0.40f, 14.0f); // Example: More fat, moderate protein/carbs
                case DietType.Whole30:
                    return new MacroSplit(0.35f, 0.30f, 0.35f, 14.0f); // Example
                case DietType.Vegan:
                    return new MacroSplit(0.25f, 0.50f, 0.25f, 20.0f); // Higher carbs/fiber
                case DietType.Vegetarian:
                    return new MacroSplit(0.25f, 0.50f, 0.25f, 18.0f); // Similar to vegan, adjust as needed
                case DietType.Mediterranean:
                    return new MacroSplit(0.20f, 0.50f, 0.30f, 16.0f); // Higher healthy fats, moderate carbs
                case DietType.HighCarb:
                    return new MacroSplit(0.20f, 0.60f, 0.20f, 14.0f);
                case DietType.LowCarb:
                    return new MacroSplit(0.30f, 0.20f, 0.50f, 14.0f);
                case DietType.BodybuilderBulk:
                    return new MacroSplit(0.35f, 0.45f, 0.20f, 14.0f); // Higher protein/carbs
                case DietType.Cutting:
                    return new MacroSplit(0.40f, 0.30f, 0.30f, 14.0f); // Higher protein, lower carbs/fat
                case DietType.Recomp:
                    return new MacroSplit(0.35f, 0.35f, 0.30f, 14.0f); // Balanced but with good protein
                case DietType.IntermittentFasting:
                    // Macro split for IF is typically balanced, but calorie timing differs.
                    // This assumes a standard macro split for the eating window.
                    return new MacroSplit(0.30f, 0.40f, 0.30f, 14.0f);
                case DietType.None:
                default:
                    return new MacroSplit(0.25f, 0.50f, 0.25f, 14.0f); // Default or balanced
            }
        }
    }
}
