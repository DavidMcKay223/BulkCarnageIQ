using Android.Views;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class TrackerFragment : Fragment
    {
        private TableLayout tableMeals;
        private AutoCompleteTextView txtFoodName;
        private Button btnAddMeal;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment layout
            return inflater.Inflate(Resource.Layout.fragment_tracker, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            tableMeals = view.FindViewById<TableLayout>(Resource.Id.tableMeals);

            txtFoodName = view.FindViewById<AutoCompleteTextView>(Resource.Id.txtFoodName);
            btnAddMeal = view.FindViewById<Button>(Resource.Id.btnAddMeal);

            var adapter = new ArrayAdapter<string>(
                Context,
                Android.Resource.Layout.SimpleDropDownItem1Line,
                GetFoodItemList()
            );
            
            txtFoodName.Adapter = adapter;

            btnAddMeal.Click += (s, e) =>
            {
                string foodName = txtFoodName.Text.Trim();

                if (string.IsNullOrWhiteSpace(foodName))
                {
                    Toast.MakeText(Context, "Enter a food name", ToastLength.Short).Show();
                    return;
                }

                AddMealByName(foodName);
            };

            // Load meals from DB/service
            var meals = LoadMealsFromDb("").Result;

            if (meals == null || !meals.Any())
            {
                // Optionally, show a message if no meals are found
                var emptyRow = new TableRow(Context);
                emptyRow.AddView(new TextView(Context) { Text = "No meals found." });
                tableMeals.AddView(emptyRow);
                return;
            }

            foreach (var meal in meals)
                AddMeal(meal.MealName, meal.PortionEaten, meal.Calories, meal.Protein, meal.Carbs, meal.Fats, meal.Fiber);
        }

        private void AddMeal(string name, float portions, float calories, float protein, float carbs, float fats, float fiber)
        {
            var row = new TableRow(Context);

            row.AddView(new TextView(Context) { Text = name });
            row.AddView(new TextView(Context) { Text = portions.ToString() });
            row.AddView(new TextView(Context) { Text = calories.ToString() });
            row.AddView(new TextView(Context) { Text = protein.ToString() });
            row.AddView(new TextView(Context) { Text = carbs.ToString() });
            row.AddView(new TextView(Context) { Text = fats.ToString() });
            row.AddView(new TextView(Context) { Text = fiber.ToString() });

            var deleteBtn = new Button(Context) { Text = "Delete" };
            deleteBtn.Click += (s, e) => tableMeals.RemoveView(row);

            row.AddView(deleteBtn);

            tableMeals.AddView(row);
        }

        private async Task<List<MealEntry>> LoadMealsFromDb(string userName)
        {
            using var dbContext = CreateDbContext();
            var mealService = new MealEntryService(dbContext);

            var mealEntry = await mealService.GetAllAsync(userName);

            return mealEntry;
        }

        private void AddMealByName(string foodName)
        {
            var macros = LookupFoodMacros(foodName);

            if (macros == null)
            {
                Toast.MakeText(Context, $"Food '{foodName}' not found", ToastLength.Short).Show();
                return;
            }
        }

        private List<string> GetFoodItemList()
        {
            using var dbContext = CreateDbContext();
            var foodService = new FoodItemService(dbContext);

            return foodService.GetAllAsync().Result.Select(x => x.RecipeName).ToList();
        }

        private FoodItem? LookupFoodMacros(string foodName)
        {
            using var dbContext = CreateDbContext();
            var mealService = new MealEntryService(dbContext);

            return mealService.GetFoodItemByNameAsync(foodName).Result;
        }

        private AppDbContext CreateDbContext()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "bulk_carnage.db"
            );

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new AppDbContext(options);
        }
    }
}
