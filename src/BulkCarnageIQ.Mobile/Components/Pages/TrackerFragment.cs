using Android.Content.Res;
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
using Google.Android.Material.Card;
using Google.Android.Material.DatePicker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;
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

            txtFoodName.SetHintTextColor(CarnageAndroid.CarnageStyle.OffWhite);
            txtFoodName.SetTextColor(CarnageAndroid.CarnageStyle.OffWhite);
            txtFoodName.BackgroundTintList = ColorStateList.ValueOf(CarnageAndroid.CarnageStyle.OffWhite);

            txtFoodName.Adapter = adapter;

            CarnageButton btnDate = null;

            btnDate = Context.CarnageButton(DateTime.Today.ToString("MM/dd/yyyy"))
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

            List<string> mealTypes = new List<string> { "Breakfast", "Lunch", "Dinner", "Snack" };

            var now = DateTime.Now.TimeOfDay;

            string MealType =
                (now >= TimeSpan.FromHours(0) && now < TimeSpan.FromHours(6)) ? "Snack" :
                (now >= TimeSpan.FromHours(6) && now < TimeSpan.FromHours(11)) ? "Breakfast" :
                (now >= TimeSpan.FromHours(11) && now < TimeSpan.FromHours(15)) ? "Lunch" :
                (now >= TimeSpan.FromHours(15) && now < TimeSpan.FromHours(20)) ? "Dinner" :
                "Snack";

            var mealTypeSpinner = Context.CarnageSpinner(mealTypes, MealType);

            var btnAddMeal = Context.CarnageButton("Add")
                .OnClick(() =>
                    {
                        string foodName = txtFoodName.Text.Trim();

                        if (string.IsNullOrWhiteSpace(foodName))
                        {
                            Toast.MakeText(Context, "Enter a food name", ToastLength.Short).Show();
                            return;
                        }

                        AddMealByName(foodName, DateOnly.Parse(btnDate.Text), foodPickerView.Progress, mealTypeSpinner.Text);
                        txtFoodName.Text = string.Empty;
                        foodPickerView.UpdateFoodSelection(null);
                        foodPickerView.Progress = 2;

                        HideKeyboard();
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
            var lp = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            lp.SetMargins(0, Context.DpToPx(8), 0, Context.DpToPx(8));

            // Create the card
            var card = new MaterialCardView(Context)
            {
                LayoutParameters = lp,
                Radius = Context.DpToPx(8),
                CardElevation = Context.DpToPx(4),
            };
            card.SetCardBackgroundColor(CarnageStyle.DarkCharcoal);
            card.SetPadding(CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium);

            // Inner vertical layout inside the card
            var contentLayout = new LinearLayout(Context)
            {
                Orientation = Android.Widget.Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            contentLayout.SetPadding(CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium);

            // Top layout: Name on top, MealType underneath
            var topColumn = new LinearLayout(Context)
            {
                Orientation = Android.Widget.Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 3f) // give most space to name+mealType
            };

            var nameView = Context.CarnageTextView(name).AsTitle();
            var mealTypeView = Context.CarnageTextView(mealType);

            topColumn.AddView(nameView);
            topColumn.AddView(mealTypeView);

            // Delete button stays to the right
            var deleteBtnTop = Context.CarnageButtonIcon(CarnageIcon.Trash, "")
                .OnClick(() =>
                {
                    tableMeals.RemoveView(card);
                    DeleteMealEntry(Id);
                });
            deleteBtnTop.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            // Top row now contains: vertical column + delete button
            var topRow = new LinearLayout(Context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };

            topRow.AddView(topColumn);
            topRow.AddView(deleteBtnTop);

            // Bottom row: Servings | Calories | Optional Button
            var bottomRow = new LinearLayout(Context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };

            var servingsView = Context.CarnageTextView($"Servings: {measurementServings * portions:N1} {measurementType}");
            servingsView.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f);

            var caloriesView = Context.CarnageTextView($"Calories: {calories:N0}");
            caloriesView.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f);

            bottomRow.AddView(servingsView);
            bottomRow.AddView(caloriesView);
            
            // Add rows to the content layout
            contentLayout.AddView(topRow);
            contentLayout.AddView(bottomRow);

            // Add content layout to the card
            card.AddView(contentLayout);

            // Add card to the table
            tableMeals.AddView(card);
        }

        private async Task<List<MealEntry>> LoadMealsFromDb(string userName, DateOnly date)
        {
            return await mealEntryService.GetByDateAsync(date, userName);
        }

        private void AddMealByName(string foodName, DateOnly date, float servings, string mealType)
        {
            var macros = LookupFoodMacros(foodName);

            if (macros == null)
            {
                Toast.MakeText(Context, $"Food '{foodName}' not found", ToastLength.Short).Show();
                return;
            }

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
                MealType = mealType,
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
                mealType
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

        void HideKeyboard()
        {
            if (Activity == null)
                return;

            var currentFocus = Activity.CurrentFocus;
            if (currentFocus != null)
            {
                var imm = (InputMethodManager)Activity.GetSystemService(Android.Content.Context.InputMethodService);
                imm?.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                currentFocus.ClearFocus();
            }
        }
    }
}
