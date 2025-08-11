using Android.OS;
using Android.Views;
using Android.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Mobile.Components.Carnage;
using Microsoft.EntityFrameworkCore;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class HomeFragment : Fragment
    {
        private MacroDonutView dailyDonut;
        private TableLayout tableDailyMacros;

        private MacroDonutView weeklyDonut;
        private TableLayout tableWeeklyMacros;
        private TextView weeklySummaryText;

        private LinearLayout miniLogContainer;

        private UserProfile currentUserProfile;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            dailyDonut = view.FindViewById<MacroDonutView>(Resource.Id.dailyDonut);
            weeklyDonut = view.FindViewById<MacroDonutView>(Resource.Id.weeklyDonut);
            weeklySummaryText = view.FindViewById<TextView>(Resource.Id.weeklySummaryText);
            miniLogContainer = view.FindViewById<LinearLayout>(Resource.Id.miniLogContainer);

            tableDailyMacros = view.FindViewById<TableLayout>(Resource.Id.tableDailyMacros);
            tableWeeklyMacros = view.FindViewById<TableLayout>(Resource.Id.tableWeeklyMacros);

            currentUserProfile = GetUserProfile();

            LoadData();
        }

        private void LoadData()
        {
            var todayDayName = DateTime.Today.DayOfWeek.ToString();
            var start = DateOnly.FromDateTime(DateTime.Today).AddDays(-6);
            var end = DateOnly.FromDateTime(DateTime.Today);

            var weeklySummaries = LookupFoodMacros(start, end);

            // Today's macros
            weeklySummaries.TryGetValue(todayDayName, out var todaySummary);
            todaySummary ??= new MacroSummary();

            // Sum weekly totals across all days
            var weekSummary = new MacroSummary();
            float weeklyCaloriesActual = 0f;

            foreach (var daySummary in weeklySummaries.Values)
            {
                weekSummary.Calories += daySummary.Calories;
                weekSummary.Protein += daySummary.Protein;
                weekSummary.Carbs += daySummary.Carbs;
                weekSummary.Fats += daySummary.Fats;
                weekSummary.Fiber += daySummary.Fiber;

                weeklyCaloriesActual += daySummary.Calories;
            }

            dailyDonut.SetMacros(todaySummary.Protein, todaySummary.Carbs, todaySummary.Fats, todaySummary.Fiber);
            weeklyDonut.SetMacros(weekSummary.Protein, weekSummary.Carbs, weekSummary.Fats, weekSummary.Fiber);

            float weeklyCaloriesGoal = currentUserProfile.CalorieGoal * 7;
            float diff = weeklyCaloriesActual - weeklyCaloriesGoal;

            weeklySummaryText.Text = diff > 0
                ? $"+{diff:N0} calories over target this week"
                : $"{Math.Abs(diff):N0} calories under target this week";

            // Populate the TableLayouts dynamically
            PopulateMacroTable(tableDailyMacros, todaySummary, 1);
            PopulateMacroTable(tableWeeklyMacros, weekSummary, 7);

            // Clear and populate mini log for today
            miniLogContainer.RemoveAllViews();
            var todayMeals = GetMealsForDate(DateOnly.FromDateTime(DateTime.Today));
            foreach (var meal in todayMeals)
            {
                AddMiniLogItem(meal.MealName, meal.MealType);
            }
        }

        private void PopulateMacroTable(TableLayout table, MacroSummary summary, int weight)
        {
            table.RemoveAllViews();

            AddMacroRow(table, "Calories", summary.Calories);

            table.AddView(new BulkCarnageIQ.Mobile.Components.Carnage.MacroProgressView(Context)
                .Add("Protein", summary.Protein, currentUserProfile.ProteinGoal * weight)
                .Add("Carbs", summary.Carbs, currentUserProfile.CarbsGoal * weight)
                .Add("Fats", summary.Fats, currentUserProfile.FatGoal * weight)
                .Add("Fiber", summary.Fiber, currentUserProfile.FiberGoal * weight));
        }

        private void AddMacroRow(TableLayout table, string label, float value)
        {
            var row = new TableRow(Context);

            var labelView = new TextView(Context)
            {
                Text = label,
                TextSize = 16
            };
            labelView.SetPadding(10, 10, 10, 10);

            var valueView = new TextView(Context)
            {
                Text = value.ToString("N1"),
                TextSize = 16
            };
            valueView.SetPadding(10, 10, 10, 10);

            row.AddView(labelView);
            row.AddView(valueView);

            table.AddView(row);
        }

        private void AddMiniLogItem(string mealName, string mealType)
        {
            var row = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal
            };
            row.SetPadding(10, 10, 10, 10);

            var nameView = new TextView(Context)
            {
                Text = mealName,
                TextSize = 16
            };

            var typeView = new TextView(Context)
            {
                Text = mealType,
                TextSize = 14
            };
            typeView.SetPadding(20, 0, 0, 0);

            row.AddView(nameView);
            row.AddView(typeView);

            miniLogContainer.AddView(row);
        }

        private UserProfile GetUserProfile()
        {
            using var dbContext = CreateDbContext();
            var userService = new UserProfileService(dbContext);

            return userService.GetUserProfile("").Result;
        }

        private List<MealEntry> GetMealsForDate(DateOnly date)
        {
            using var dbContext = CreateDbContext();
            var mealService = new MealEntryService(dbContext);

            return mealService.GetByDateAsync(date, "").Result;
        }

        private Dictionary<string, MacroSummary> LookupFoodMacros(DateOnly start, DateOnly end)
        {
            using var dbContext = CreateDbContext();
            var mealService = new MealEntryService(dbContext);

            return mealService.GetMacroSummariesByDateRangeAsync(start, end, "").Result;
        }

        private AppDbContext CreateDbContext()
        {
            var dbPath = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "bulk_carnage.db"
            );

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new AppDbContext(options);
        }
    }
}
