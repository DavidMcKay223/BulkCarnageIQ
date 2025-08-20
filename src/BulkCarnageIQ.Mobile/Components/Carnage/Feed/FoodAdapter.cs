using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Carnage.Feed
{
    public class FoodAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly List<FoodItem> _allFoods;
        private List<FoodItem> _filteredFoods;
        private readonly UserProfile _currentUserProfile;
        private readonly MacroSummary _macroSummary;

        public FoodAdapter(Context context, List<FoodItem> foods, MacroSummary macroSummary, UserProfile currentUserProfile)
        {
            _context = context;
            _allFoods = foods;
            _filteredFoods = new List<FoodItem>(_allFoods);
            _currentUserProfile = currentUserProfile;
            _macroSummary = macroSummary;
        }

        public override int ItemCount => _filteredFoods.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is FoodViewHolder vh) vh.Bind(_filteredFoods[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = new FoodDisplayView(_context, _macroSummary, _currentUserProfile);
            return new FoodViewHolder(view);
        }

        public void Filter(Func<FoodItem, bool> predicate)
        {
            _filteredFoods = _allFoods.Where(predicate).ToList();
            NotifyDataSetChanged();
        }

        public void Filter(FoodItemFilter filter)
        {
            _filteredFoods = filter.ApplyFilters(_allFoods)
                .OrderByDescending(f => {
                    // We don't want to see zero-macro foods first.
                    // If a food has no protein, fiber, or calories, it gets a minimum score.
                    if (f.Protein <= 0 && f.Fiber <= 0 && f.TotalCalories <= 0)
                    {
                        return float.MinValue;
                    }

                    // Current macro status
                    float remainingProtein = _currentUserProfile.ProteinGoal - _macroSummary.Protein;
                    float remainingFiber = _currentUserProfile.FiberGoal - _macroSummary.Fiber;
                    float remainingCalories = _currentUserProfile.CalorieGoal - _macroSummary.Calories;
                    float remainingFat = _currentUserProfile.FatGoal - _macroSummary.Fats;
                    float remainingCarbs = _currentUserProfile.CarbsGoal - _macroSummary.Carbs;

                    float score = 0;

                    // Use multipliers to define priority
                    const float proteinMultiplier = 1000000000;
                    const float fiberMultiplier = 1000000000; // Giving fiber top priority in this scenario
                    const float calorieMultiplier = 10000;
                    const float fatMultiplier = 1000;
                    const float carbMultiplier = 100;

                    // 1. Fiber Priority: This is the only macro you're under on, so it gets top priority.
                    if (remainingFiber > 0)
                    {
                        score += Math.Min(f.Fiber, remainingFiber) * fiberMultiplier;
                    }
                    else
                    {
                        // Even if over, we assume fiber is beneficial, so we still reward it.
                        score += f.Fiber * fiberMultiplier;
                    }

                    // 2. Calorie Penalty: You are over on calories, so we heavily penalize high-calorie foods.
                    if (remainingCalories <= 0)
                    {
                        score -= f.TotalCalories * calorieMultiplier;
                    }
                    else
                    {
                        // If you were under, this would be the logic to reward low-calorie foods
                        score -= f.TotalCalories * calorieMultiplier;
                    }

                    // 3. Protein Penalty: You are over on protein.
                    if (remainingProtein <= 0)
                    {
                        score -= f.Protein * proteinMultiplier;
                    }
                    else
                    {
                        score += Math.Min(f.Protein, remainingProtein) * proteinMultiplier;
                    }

                    // 4. Fat Penalty: You are over on fat.
                    if (remainingFat <= 0)
                    {
                        score -= f.Fats * fatMultiplier;
                    }
                    else
                    {
                        score -= f.Fats * fatMultiplier;
                    }

                    // 5. Carb Penalty: You are over on carbs.
                    if (remainingCarbs <= 0)
                    {
                        score -= f.Carbs * carbMultiplier;
                    }
                    else
                    {
                        score -= f.Carbs * carbMultiplier;
                    }

                    return score;
                })
                .ToList();

            NotifyDataSetChanged();
        }

        public class FoodViewHolder : RecyclerView.ViewHolder
        {
            private readonly FoodDisplayView _view;
            public FoodViewHolder(FoodDisplayView view) : base(view) { _view = view; }
            public void Bind(FoodItem item) => _view.Bind(item);
        }
    }
}
