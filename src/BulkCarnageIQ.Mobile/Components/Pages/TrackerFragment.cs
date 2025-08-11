using Android.Graphics;
using Android.Graphics.Text;
using Android.Views;
using Android.Views.InputMethods;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Mobile.Components.Carnage;
using CarnageAndroid;
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
                txtFoodName.Text = string.Empty;

                HideKeyboard();
            };

            // Load meals from DB/service
            var meals = LoadMealsFromDb("").Result;

            foreach (var meal in meals)
                AddMeal(meal.Id, meal.MealName, meal.PortionEaten, meal.Calories, meal.Protein, meal.Carbs, meal.Fats, meal.Fiber, meal.MealType);
        }

        private void AddMeal(int Id, string name, float portions, float calories, float protein, float carbs, float fats, float fiber, string mealType)
        {
            var container = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            container.SetPadding(CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium);

            var nameView = new CarnageTextView(Context)
                .WithText(name)
                .AsTitle();
            container.AddView(nameView);

            var mealTypeView = new CarnageTextView(Context)
                .WithText(mealType);
            container.AddView(mealTypeView);

            var topRow = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };

            var leftContainer = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f)
            };
            leftContainer.SetPadding(CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium);

            var leftColumn = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f)
            };
            leftColumn.AddView(new CarnageTextView(Context).WithText($"Servings: {portions:N1}"));
            leftColumn.AddView(new CarnageTextView(Context).WithText($"Protein: {protein:N1}"));
            leftColumn.AddView(new CarnageTextView(Context).WithText($"Fiber: {fiber:N1}"));

            var rightColumn = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f)
            };
            rightColumn.AddView(new CarnageTextView(Context).WithText($"Calories: {calories:N1}"));
            rightColumn.AddView(new CarnageTextView(Context).WithText($"Carbs: {carbs:N1}"));
            rightColumn.AddView(new CarnageTextView(Context).WithText($"Fats: {fats:N1}"));

            leftContainer.AddView(leftColumn);
            leftContainer.AddView(rightColumn);

            var donut = new MacroDonutView(Context, protein, carbs, fats, fiber)
            {
                LayoutParameters = new LinearLayout.LayoutParams(300, 300)
                {
                    LeftMargin = CarnageStyle.PaddingMedium
                }
            };

            topRow.AddView(leftContainer);
            topRow.AddView(donut);

            var deleteBtn = new CarnageButton(Context)
                .WithText("Delete")
                .SetStyle(CarnageButtonStyle.Danger)
                .OnClick(() =>
                {
                    tableMeals.RemoveView(container);
                    DeleteMealEntry(Id);
                });
            deleteBtn.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            deleteBtn.SetPadding(0, CarnageStyle.PaddingMedium, 0, CarnageStyle.PaddingMedium);

            container.AddView(topRow);
            container.AddView(deleteBtn);

            tableMeals.AddView(container);
        }

        private async Task<List<MealEntry>> LoadMealsFromDb(string userName)
        {
            using var dbContext = CreateDbContext();
            var mealService = new MealEntryService(dbContext);

            var mealEntry = await mealService.GetByDateAsync(DateOnly.FromDateTime(DateTime.Today), userName);

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

            var now = DateTime.Now.TimeOfDay;

            string MealType =
                (now >= TimeSpan.FromHours(0) && now < TimeSpan.FromHours(6)) ? "Snack" :
                (now >= TimeSpan.FromHours(6) && now < TimeSpan.FromHours(12)) ? "Breakfast" :
                (now >= TimeSpan.FromHours(12) && now < TimeSpan.FromHours(15)) ? "Lunch" :
                (now >= TimeSpan.FromHours(15) && now < TimeSpan.FromHours(20)) ? "Dinner" :
                "Snack";

            var mealEntry = new MealEntry
            {
                MealName = macros.RecipeName,
                PortionEaten = macros.Servings,
                Calories = macros.CaloriesPerServing * macros.Servings,
                Protein = macros.Protein * macros.Servings,
                Carbs = macros.Carbs * macros.Servings,
                Fats = macros.Fats * macros.Servings,
                Fiber = macros.Fiber * macros.Servings,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Day = DateTime.Today.DayOfWeek.ToString(),
                MealType = MealType,
                UserId = ""
            };

            SaveMealEntry(mealEntry);

            AddMeal(
                mealEntry.Id,
                macros.RecipeName,
                macros.Servings,
                macros.CaloriesPerServing * macros.Servings,
                macros.Protein * macros.Servings,
                macros.Carbs * macros.Servings,
                macros.Fats * macros.Servings,
                macros.Fiber * macros.Servings,
                MealType
            );
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

        private void SaveMealEntry(MealEntry mealEntry)
        {
            using var dbContext = CreateDbContext();
            var mealService = new MealEntryService(dbContext);

            mealService.AddAsync(mealEntry).Wait();
        }

        private void DeleteMealEntry(int Id)
        {
            using var dbContext = CreateDbContext();
            var mealService = new MealEntryService(dbContext);

            mealService.DeleteAsync(Id).Wait();
        }

        private AppDbContext CreateDbContext()
        {
            var dbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "bulk_carnage.db"
            );

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new AppDbContext(options);
        }

        void HideKeyboard()
        {
            var inputMethodManager = (InputMethodManager)Context.GetSystemService(Android.Content.Context.InputMethodService);
            var token = txtFoodName.WindowToken; // your AutoCompleteTextView instance
            if (token != null)
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
        }
    }
}
