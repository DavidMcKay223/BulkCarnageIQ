using Android.Views;
using AndroidX.RecyclerView.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
using BulkCarnageIQ.Core.Contracts;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Mobile.Components.Carnage.Feed;
using CarnageAndroid;
using CarnageAndroid.UI;
using MikePhil.Charting.Highlight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class FoodListFragment : Fragment
    {
        private LinearLayout fixedContentLayout;
        private LinearLayout dynamicContentLayout;

        private UserProfile currentUserProfile;
        private FoodItemService foodItemService;
        private MealEntryService mealEntryService;

        private FoodAdapter adapter;
        private FoodItemFilter filter;

        public FoodListFragment(AppDbContext db, UserProfile userProfile) : base()
        {
            currentUserProfile = userProfile;
            foodItemService = new FoodItemService(db);
            mealEntryService = new MealEntryService(db);
            filter = new FoodItemFilter();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_dynamic, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            fixedContentLayout = view.FindViewById<LinearLayout>(Resource.Id.fixed_content);
            dynamicContentLayout = view.FindViewById<LinearLayout>(Resource.Id.dynamic_content);

            fixedContentLayout.AddView(Context.CarnageTextView("Food List").AsTitle());

            var macroSummary = mealEntryService.GetMacroSummariesByDateRangeAsync(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today), currentUserProfile.UserName).Result.TryGetValue(DateOnly.FromDateTime(DateTime.Today).DayOfWeek.ToString(), out MacroSummary ms) ? ms : new MacroSummary();

            var foodList = filter.ApplyFilters(foodItemService.GetAllAsync().Result)
                .OrderByDescending(f => {
                    // We don't want to see zero-macro foods first.
                    // If a food has no protein, fiber, or calories, it gets a minimum score.
                    if (f.Protein <= 0 && f.Fiber <= 0 && f.TotalCalories <= 0)
                    {
                        return float.MinValue;
                    }

                    // Current macro status
                    float remainingProtein = currentUserProfile.ProteinGoal - macroSummary.Protein;
                    float remainingFiber = currentUserProfile.FiberGoal - macroSummary.Fiber;
                    float remainingCalories = currentUserProfile.CalorieGoal - macroSummary.Calories;
                    float remainingFat = currentUserProfile.FatGoal - macroSummary.Fats;
                    float remainingCarbs = currentUserProfile.CarbsGoal - macroSummary.Carbs;

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

            if (foodList == null || !foodList.Any())
            {
                dynamicContentLayout.AddView(Context.CarnageTextView("No food items found."));
                return;
            }

            // RecyclerView setup
            var recycler = new RecyclerView(Context);
            recycler.SetLayoutManager(new LinearLayoutManager(Context));
            adapter = new FoodAdapter(Context, foodList, macroSummary, currentUserProfile);
            recycler.SetAdapter(adapter);
            dynamicContentLayout.AddView(recycler);

            // Filter binder
            var binder = new FoodFilterBinder(filter, adapter);

            // Add all checkboxes to fixedContentLayout
            AddFilterCheckboxes(fixedContentLayout, binder);
        }

        private void AddFilterCheckboxes(LinearLayout container, FoodFilterBinder binder)
        {
            // Row 1
            var row1 = new LinearLayout(container.Context) { Orientation = Orientation.Horizontal };
            row1.AddView(CreateCheckBox("High Protein", nameof(FoodItemFilter.IsHighProtein), binder));
            row1.AddView(CreateCheckBox("Low Carb", nameof(FoodItemFilter.IsLowCarb), binder));
            row1.AddView(CreateCheckBox("Keto", nameof(FoodItemFilter.IsKeto), binder));
            container.AddView(row1);

            // Row 2
            var row2 = new LinearLayout(container.Context) { Orientation = Orientation.Horizontal };
            row2.AddView(CreateCheckBox("Bulk Meal", nameof(FoodItemFilter.IsBulkMeal), binder));
            row2.AddView(CreateCheckBox("High Carb", nameof(FoodItemFilter.IsHighCarb), binder));
            row2.AddView(CreateCheckBox("High Fiber", nameof(FoodItemFilter.IsHighFiber), binder));
            container.AddView(row2);

            // Row 3
            var row3 = new LinearLayout(container.Context) { Orientation = Orientation.Horizontal };
            row3.AddView(CreateCheckBox("High Fat", nameof(FoodItemFilter.IsHighFat), binder));
            row3.AddView(CreateCheckBox("Balanced Meal", nameof(FoodItemFilter.IsBalancedMeal), binder));
            row3.AddView(CreateCheckBox("Low Protein", nameof(FoodItemFilter.IsLowProtein), binder));
            container.AddView(row3);
        }

        private Android.Widget.CheckBox CreateCheckBox(string text, string propertyName, FoodFilterBinder binder)
        {
            var cb = new Android.Widget.CheckBox(Context) { Text = text };
            cb.SetTextColor(CarnageStyle.OffWhite);
            binder.BindCheckBox(cb, propertyName);

            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            lp.SetMargins(0, 0, 16, 16);
            cb.LayoutParameters = lp;

            return cb;
        }
    }
}
