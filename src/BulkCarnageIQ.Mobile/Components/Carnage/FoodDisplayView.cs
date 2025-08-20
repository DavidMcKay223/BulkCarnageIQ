using Android.Content;
using Android.Content.Res;
using Android.Hardware.Lights;
using Android.Net;
using AndroidX.CardView.Widget;
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
using BulkCarnageIQ.Core.Contracts;
using BulkCarnageIQ.Infrastructure.Persistence;
using BulkCarnageIQ.Infrastructure.Repositories;
using CarnageAndroid;
using CarnageAndroid.UI;
using Google.Android.Material.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.CaseMap;

namespace BulkCarnageIQ.Mobile.Components.Carnage
{
    public class FoodDisplayView : MaterialCardView
    {
        private FoodItem _currentFood;
        private UserProfile _currentUserProfile;
        private MacroSummary _macroSummary;

        public FoodDisplayView(Context context, MacroSummary macroSummary, UserProfile currentUserProfile) : base(context)
        {
            _currentFood = null;
            _currentUserProfile = currentUserProfile;
            _macroSummary = macroSummary;

            Radius = Context.DpToPx(8);
            CardElevation = Context.DpToPx(4);

            this.WithMargins(0, 0, 0, CarnageStyle.PaddingLarge);

            SetBackgroundColor(CarnageStyle.MidnightBlue);
            SetCardBackgroundColor(CarnageStyle.DarkCharcoal);

            Bind(_currentFood);
        }

        public void Bind(FoodItem item)
        {
            RemoveAllViews();
            _currentFood = item;

            if (_currentFood == null) return;

            var container = new LinearLayout(Context) { Orientation = Android.Widget.Orientation.Vertical };
            container.WithPadding(CarnageStyle.PaddingMedium, 0, CarnageStyle.PaddingMedium, CarnageStyle.PaddingMedium);
            container.SetBackgroundColor(CarnageStyle.DarkCharcoal);

            AddView(container);

            container.AddView(Context.CarnageTextView(_currentFood.RecipeName).AsTitle().WithMargins(0, CarnageStyle.PaddingSmall, 0, CarnageStyle.PaddingSmall));

            container.AddView(Context.CarnageTextView($"{_currentFood.BrandType} - {_currentFood.GroupName}"));
            container.AddView(Context.CarnageTextView($"{_currentFood.Servings} x {_currentFood.MeasurementServings} {_currentFood.MeasurementType}"));

            container.AddView(new BulkCarnageIQ.Mobile.Components.Carnage.MacroProgressView(Context)
                .Add("Calories", _currentFood.TotalCalories, _currentUserProfile.CalorieGoal - _macroSummary.Calories, "")
                .Add("Protein", _currentFood.Protein, _currentUserProfile.ProteinGoal - _macroSummary.Protein)
                .Add("Carbs", _currentFood.Carbs, _currentUserProfile.CarbsGoal - _macroSummary.Carbs)
                .Add("Fats", _currentFood.Fats, _currentUserProfile.FatGoal - _macroSummary.Fats)
                .Add("Fiber", _currentFood.Fiber, _currentUserProfile.FiberGoal - _macroSummary.Fiber));
        }
    }
}
