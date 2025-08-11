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

            PopulateMacroTable($"Today Macros:", $"{DateTime.Today.ToShortDateString()}", tableDailyMacros, todaySummary, 1);
            PopulateMacroTable($"Weekly Summary:", $"{DateTime.Today.AddDays(-6).ToShortDateString()} - {DateTime.Today.ToShortDateString()}", tableWeeklyMacros, weekSummary, 7);
        }

        private void PopulateMacroTable(String Title, String DateStr, TableLayout table, MacroSummary summary, int weight)
        {
            table.RemoveAllViews();

            table.AddView(new CarnageAndroid.CarnageTextView(Context)
                .WithStyle(CarnageAndroid.CarnageTextViewStyle.Title)
                .WithText(Title));

            table.AddView(new CarnageAndroid.CarnageTextView(Context)
                .WithStyle(CarnageAndroid.CarnageTextViewStyle.Secondary)
                .WithText(DateStr));

            table.AddView(new BulkCarnageIQ.Mobile.Components.Carnage.MacroProgressView(Context)
                .Add("Calories", summary.Calories, currentUserProfile.CalorieGoal * weight, "")
                .Add("Protein", summary.Protein, currentUserProfile.ProteinGoal * weight)
                .Add("Carbs", summary.Carbs, currentUserProfile.CarbsGoal * weight)
                .Add("Fats", summary.Fats, currentUserProfile.FatGoal * weight)
                .Add("Fiber", summary.Fiber, currentUserProfile.FiberGoal * weight));
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
