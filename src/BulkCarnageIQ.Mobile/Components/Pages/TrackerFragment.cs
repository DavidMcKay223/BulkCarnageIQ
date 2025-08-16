using Android.Graphics;
using Android.Graphics.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using BulkCarnageIQ.Mobile.Components.Carnage;
using CarnageAndroid;
using CarnageAndroid.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using static Android.InputMethodServices.Keyboard;

namespace BulkCarnageIQ.Mobile.Components.Pages
{
    public class TrackerFragment : Fragment
    {
        private TableLayout tableMeals;
        private TableLayout tableAdd;

        private UserProfile currentUserProfile;
        private MealEntryService mealEntryService;
        private FoodItemService foodItemService;

        public TrackerFragment(AppDbContext db, UserProfile userProfile) : base()
        {
            currentUserProfile = userProfile;
            mealEntryService = new MealEntryService(db);
            foodItemService = new FoodItemService(db);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment layout
            return inflater.Inflate(Resource.Layout.fragment_tracker, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var row = new TableRow(Context)
            {
                LayoutParameters = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };

            tableMeals = view.FindViewById<TableLayout>(Resource.Id.tableMeals);
            tableAdd = view.FindViewById<TableLayout>(Resource.Id.addTable);

            var txtFoodName = new AutoCompleteTextView(Context)
            {
                LayoutParameters = new TableRow.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Hint = "Search for food",
            };

            var adapter = new ArrayAdapter<string>(
                Context,
                Android.Resource.Layout.SimpleDropDownItem1Line,
                GetFoodItemList()
            );
            
            txtFoodName.Adapter = adapter;

            CarnageButton btnDate = null;

            btnDate = Context.CarnageButton(CarnageButtonStyle.Secondary, DateTime.Today.ToString("MM/dd/yyyy"))
                .OnClick(() =>
                {
                    var dpd = new DatePickerDialog(Context, (sender, e) =>
                    {
                        btnDate.Text = e.Date.ToString("MM/dd/yyyy");

                        tableMeals.RemoveAllViews();

                        var meals = LoadMealsFromDb(currentUserProfile.UserName, DateOnly.Parse(btnDate.Text)).Result;

                        foreach (var meal in meals)
                            AddMeal(meal.Id, meal.MealName, meal.MeasurementServings, meal.MeasurementType, meal.PortionEaten, meal.Calories, meal.Protein, meal.Carbs, meal.Fats, meal.Fiber, meal.MealType);
                    }, DateTime.Today.Year, DateTime.Today.Month - 1, DateTime.Today.Day);

                    dpd.Show();
                });

            FoodPickerView foodPickerView = new FoodPickerView(Context); ;

            txtFoodName.ItemClick += (s, e) =>
            {
                string selectedFood = txtFoodName.Adapter.GetItem(e.Position).ToString();
                var macros = LookupFoodMacros(selectedFood);
                foodPickerView.UpdateFoodSelection(macros);
            };

            var btnAddMeal = Context.CarnageButton(CarnageButtonStyle.Primary, "Add")
                .OnClick(() =>
                    {
                        string foodName = txtFoodName.Text.Trim();

                        if (string.IsNullOrWhiteSpace(foodName))
                        {
                            Toast.MakeText(Context, "Enter a food name", ToastLength.Short).Show();
                            return;
                        }

                        AddMealByName(foodName, DateOnly.Parse(btnDate.Text), foodPickerView.Progress);
                        txtFoodName.Text = string.Empty;
                        foodPickerView.UpdateFoodSelection(null);
                        foodPickerView.Progress = 2;

                        HideKeyboard(txtFoodName);
                    });

            tableAdd.AddView(btnDate);
            tableAdd.AddView(txtFoodName);
            tableAdd.AddView(foodPickerView);
            tableAdd.AddView(btnAddMeal);

            // Load meals from DB/service
            var meals = LoadMealsFromDb("", DateOnly.Parse(btnDate.Text)).Result;

            foreach (var meal in meals)
                AddMeal(meal.Id, meal.MealName, meal.MeasurementServings, meal.MeasurementType, meal.PortionEaten, meal.Calories, meal.Protein, meal.Carbs, meal.Fats, meal.Fiber, meal.MealType);
        }

        private void AddMeal(int Id, string name, float? measurementServings, string measurementType, float portions, float calories, float protein, float carbs, float fats, float fiber, string mealType)
        {
            var container = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            container.SetPadding(CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium);

            // Food Name (title)
            var nameView = Context.CarnageTextView(CarnageTextViewStyle.Title, name);

            container.AddView(nameView);

            // Meal Type
            var mealTypeView = Context.CarnageTextView(CarnageTextViewStyle.Secondary, text: mealType);

            container.AddView(mealTypeView);

            // Servings + Calories row (horizontal)
            var servingsCaloriesRow = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };

            servingsCaloriesRow.AddView(Context.CarnageTextView(CarnageTextViewStyle.Secondary, text: $"Servings: {measurementServings * portions:N1} {measurementType}"));
            servingsCaloriesRow.AddView(Context.CarnageTextView(CarnageTextViewStyle.Secondary, text: "  |  "));
            servingsCaloriesRow.AddView(Context.CarnageTextView(CarnageTextViewStyle.Secondary, text: $"Calories: {calories:N0}"));

            container.AddView(servingsCaloriesRow);

            // Donut chart below, centered
            var donut = new MacroDonutView(Context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(400, 400)
                {
                    Gravity = Android.Views.GravityFlags.CenterHorizontal,
                    TopMargin = CarnageStyle.PaddingMedium,
                    BottomMargin = CarnageStyle.PaddingMedium
                }
            };
            donut.SetMacros(protein, carbs, fats, fiber);

            container.AddView(donut);

            // Delete button full width below
            var deleteBtn = Context.CarnageButton(CarnageButtonStyle.Danger, "Delete")
                .OnClick(() =>
                {
                    tableMeals.RemoveView(container);
                    DeleteMealEntry(Id);
                });
            deleteBtn.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            deleteBtn.SetPadding(0, CarnageStyle.PaddingMedium, 0, CarnageStyle.PaddingMedium);

            container.AddView(deleteBtn);

            tableMeals.AddView(container);
        }

        private async Task<List<MealEntry>> LoadMealsFromDb(string userName, DateOnly date)
        {
            return await mealEntryService.GetByDateAsync(date, userName);
        }

        private void AddMealByName(string foodName, DateOnly date, float servings)
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
                (now >= TimeSpan.FromHours(6) && now < TimeSpan.FromHours(11)) ? "Breakfast" :
                (now >= TimeSpan.FromHours(11) && now < TimeSpan.FromHours(15)) ? "Lunch" :
                (now >= TimeSpan.FromHours(15) && now < TimeSpan.FromHours(20)) ? "Dinner" :
                "Snack";

            var mealEntry = new MealEntry
            {
                MealName = macros.RecipeName,
                PortionEaten = servings,
                MeasurementServings = macros.MeasurementServings,
                MeasurementType = macros.MeasurementType,
                GroupName = macros.GroupName,
                Calories = macros.CaloriesPerServing * servings,
                Protein = macros.Protein * servings,
                Carbs = macros.Carbs * servings,
                Fats = macros.Fats * servings,
                Fiber = macros.Fiber * servings,
                Date = date,
                Day = date.DayOfWeek.ToString(),
                MealType = MealType,
                UserId = ""
            };

            SaveMealEntry(mealEntry);

            AddMeal(
                mealEntry.Id,
                macros.RecipeName,
                macros.MeasurementServings,
                macros.MeasurementType,
                servings,
                macros.CaloriesPerServing * servings,
                macros.Protein * servings,
                macros.Carbs * servings,
                macros.Fats * servings,
                macros.Fiber * servings,
                MealType
            );
        }

        private List<string> GetFoodItemList()
        {
            return foodItemService.GetAllAsync().Result.Select(x => x.RecipeName).ToList();
        }

        private FoodItem? LookupFoodMacros(string foodName)
        {
            return mealEntryService.GetFoodItemByNameAsync(foodName).Result;
        }

        private void SaveMealEntry(MealEntry mealEntry)
        {
            mealEntryService.AddAsync(mealEntry).Wait();
        }

        private void DeleteMealEntry(int Id)
        {
            mealEntryService.DeleteAsync(Id).Wait();
        }

        void HideKeyboard(AutoCompleteTextView control)
        {
            var inputMethodManager = (InputMethodManager)Context.GetSystemService(Android.Content.Context.InputMethodService);
            var token = control.WindowToken;
            if (token != null)
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
        }
    }
}
